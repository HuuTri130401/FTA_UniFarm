﻿using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /*
      => Tạo một list rỗng để chứa Order sau mỗi lần tạo thành công sẽ thêm vào
      => Check số lượng từng sản phẩm có đủ trong ProductInMenu => Không đủ thì return
      => Loop qua FarmHubAndProduct table
      => Create order => Create order fail return
      => Duyệt qua từng sản phẩm trong orderDetail thêm vào orderDetail
      => Check số lượng trong lúc lên đơn hàng
      => Trừ số lượng sản phẩm trong productItemInMenu => Không đủ SL để cập nhật return
      => Tạo order detail trong 1 order => Tạo order detail thất bại rollback + return
      =>  Cập nhật số tiền trong ví => Fail rollback + return
      => Tạo transaction bởi orderId và số tiền  ở trên => Fail rolllback and return
     */
    public async Task<OperationResult<OrderRequestCreate>> CreateOrder(OrderRequestCreate orderRequestCreate,
        Guid createdBy)
    {
        orderRequestCreate.paymentStatus = "Pending";
        var result = new OperationResult<OrderRequestCreate>();
        var customer = await _unitOfWork.AccountRepository.GetByIdAsync(createdBy);
        var listNewOrder = new List<Order>();
        if (customer == null)
        {
            result.StatusCode = StatusCode.NotFound;
            result.Message = "Khách hàng không tồn tại vui lòng đăng nhập lại";
            result.IsError = true;
            orderRequestCreate.paymentStatus = "Failure";
            return result;
        }

        var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.AccountId == customer.Id)
            .FirstOrDefaultAsync();
        if (wallet == null)
        {
            result.StatusCode = StatusCode.NotFound;
            result.Message = "Không tìm thấy ví của khách hàng!";
            result.IsError = true;
            orderRequestCreate.paymentStatus = "Failure";
            return result;
        }

        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Check số lượng từng sản phẩm có đủ trong ProductInMenu => Không đủ thì return
            foreach (var item in orderRequestCreate.FamHubAndProduct)
            {
                foreach (var orderDetail in item.orderDetail)
                {
                    var productInMenu = await _unitOfWork.ProductItemInMenuRepository
                        .FilterByExpression(
                            x => x.ProductItemId == orderDetail.ProductItemId
                            /*&& x.Status == "Preparing"*/
                        )
                        .FirstOrDefaultAsync();
                    if (productInMenu == null)
                    {
                        result.Message = "Sản phẩm không tìm thấy hoặc ngừng kinh doanh";
                        result.StatusCode = StatusCode.NotFound;
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }

                    if (productInMenu.Quantity < orderDetail.Quantity)
                    {
                        result.Message = "Không đủ sản phẩm trong menu!";
                        result.StatusCode = StatusCode.BadRequest;
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }
                }
            }

            // 2. Loop qua FarmHubAndProduct table
            foreach (var item in orderRequestCreate.FamHubAndProduct)
            {
                var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(item.farmHubId);
                if (farmHub == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.Message = "Không tìm thấy FarmHub";
                    result.IsError = true;
                    orderRequestCreate.paymentStatus = "Failure";
                    return result;
                }


                // 3. Create order => Create order fail return
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    FarmHubId = item.farmHubId,
                    CustomerId = customer.Id,
                    StationId = orderRequestCreate.stationId,
                    BusinessDayId = orderRequestCreate.businessDayId,
                    TotalAmount = item.totalFarmHubPrice,
                    Code = "Order" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ExpectedReceiveDate = DateTime.Now + TimeSpan.FromDays(1),
                    ShipAddress = orderRequestCreate.shipNote,
                    IsPaid = false
                };

                await _unitOfWork.OrderRepository.AddAsync(order);
                var count = await _unitOfWork.SaveChangesAsync();
                if (count == 0)
                {
                    await transaction.RollbackAsync();
                    result.StatusCode = StatusCode.BadRequest;
                    result.Message = "Tạo order không thành công";
                    result.IsError = true;
                    orderRequestCreate.paymentStatus = "Failure";
                    return result;
                }

                // 4. Duyệt qua từng sản phẩm trong orderDetail thêm vào orderDetail
                foreach (var orderDetail in item.orderDetail)
                {
                    //5. Check số lượng trong lúc lên đơn hàng
                    var productInMenu = await _unitOfWork.ProductItemInMenuRepository
                        .FilterByExpression(
                            x => x.ProductItemId == orderDetail.ProductItemId && x.Status == "Preparing")
                        .FirstOrDefaultAsync();
                    if (productInMenu == null)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Sản phẩm không tìm thấy hoặc ngừng kinh doanh";
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }

                    if (productInMenu.Quantity < orderDetail.Quantity)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Không đủ sản phẩm trong menu!" + orderDetail.ProductItemId + " " +
                                         orderDetail.Quantity;
                        orderRequestCreate.paymentStatus = "Failure";
                        result.IsError = true;
                        return result;
                    }

                    //6. Trừ số lượng sản phẩm trong productItemInMenu => Không đủ SL để cập nhật return
                    productInMenu.Quantity -= orderDetail.Quantity;
                    await _unitOfWork.ProductItemInMenuRepository.UpdateAsync(productInMenu);
                    var updateQuantity = await _unitOfWork.SaveChangesAsync();
                    if (updateQuantity == 0)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Cập nhật số lượng sản phẩm trong menu không thành công";
                        result.IsError = true;
                        return result;
                    }

                    // 7. Tạo order detail trong 1 order => Tạo order detail thất bại rollback + return
                    var orderDetailEntity = new OrderDetail()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductItemId = orderDetail.ProductItemId,
                        Quantity = orderDetail.Quantity,
                        UnitPrice = orderDetail.UnitPrice,
                        Unit = orderDetail.Unit,
                        TotalPrice = orderDetail.TotalPrice
                    };
                    await _unitOfWork.OrderDetailRepository.AddAsync(orderDetailEntity);
                    var countOrderDetail = await _unitOfWork.SaveChangesAsync();
                    if (countOrderDetail == 0)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Tạo order detail không thành công";
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }


                    // 8. Cập nhật số tiền trong ví => Fail rollback + return

                    if (wallet.Balance < orderDetail.TotalPrice)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Ví không đủ tiền. Vui lòng nạp thêm tiền vào ví!";
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }

                    wallet.Balance -= orderDetail.TotalPrice;
                    await _unitOfWork.WalletRepository.UpdateAsync(wallet);
                    var countWallet = await _unitOfWork.SaveChangesAsync();
                    if (countWallet == 0)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Cập nhật ví không thành công!";
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }

                    // 9. Tạo transaction bởi orderId và số tiền  ở trên => Fail rolllback and return
                    var transactionEntity = new Transaction()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        Amount = orderDetail.TotalPrice,
                        PaymentDate = DateTime.Now,
                        Status = "Success",
                        WalletId = wallet.Id
                    };

                    await _unitOfWork.TransactionRepository.AddAsync(transactionEntity);
                    var countTransaction = await _unitOfWork.SaveChangesAsync();
                    if (countTransaction == 0)
                    {
                        await transaction.RollbackAsync();
                        result.StatusCode = StatusCode.BadRequest;
                        result.Message = "Tạo transaction không thành công";
                        result.IsError = true;
                        orderRequestCreate.paymentStatus = "Failure";
                        return result;
                    }

                    order.IsPaid = true;
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    listNewOrder.Add(order);
                }
            }

            result.Message = "Tạp đơn hàng thành công!";
            result.StatusCode = StatusCode.Created;
            result.IsError = false;
            orderRequestCreate.paymentStatus = "Success";
            orderRequestCreate.paymentAmount = orderRequestCreate.totalMoney;
            result.Payload = orderRequestCreate;
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            result.AddError(StatusCode.ServerError, e.Message);
            result.Message = "Tạo order thất bại!";
            result.IsError = true;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff>>> GetAllOrdersOfACollectedHub(
        bool? isAscending, string? orderBy, Expression<Func<Order, bool>>? filter = null, int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff>>();
        try
        {
            var orders = await _unitOfWork.OrderRepository
                .FilterAll(isAscending, orderBy, filter, null, pageIndex, pageSize).ToListAsync();
            if (orders == null)
            {
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Không tìm thấy đơn hàng nào!";
                result.IsError = false;
                return result;
            }

            var orderResponses = new List<OrderResponse.OrderResponseForStaff>();
            foreach (var order in orders)
            {
                
                var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDay = await _unitOfWork.BusinessDayRepository.FilterByExpression(x => x.Id == order.BusinessDayId)
                    .FirstOrDefaultAsync();
                var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                    .FirstOrDefaultAsync();

                var stationResponse = new StationResponse.StationResponseSimple();
                if (station != null)
                {
                    stationResponse.Id = station.Id;
                    stationResponse.Name = station.Name;
                    stationResponse.Address = station.Address;
                    stationResponse.Description = station.Description;
                    stationResponse.Image = station.Image;
                    stationResponse.CreatedAt = station.CreatedAt;
                    stationResponse.UpdatedAt = station.UpdatedAt;
                    stationResponse.Status = station.Status;
                }
                
                var orderDetails = await _unitOfWork.OrderDetailRepository
                    .FilterByExpression(x => x.OrderId == order.Id)
                    .ToListAsync();
                
                var orderDetailResponses = new List<OrderDetailResponse>();

                foreach (var item in orderDetails)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId);
                    var orderDetailResponse = new OrderDetailResponse()
                    {
                        ProductItemId = item.ProductItemId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Unit = item.Unit,
                        TotalPrice = item.TotalPrice,
                        Title = productItem.Title
                    };
                    orderDetailResponses.Add(orderDetailResponse);
                }

                var batchReponse = new BatchResponseSimple();
                if(order.BatchId != null)
                {
                    var batch = await _unitOfWork.BatchesRepository.FilterByExpression(x => x.Id == order.BatchId).FirstOrDefaultAsync();
                    batchReponse.Id = batch.Id;
                    batchReponse.FarmShipDate = batch.FarmShipDate;
                    batchReponse.CollectedHubReceiveDate = batch.CollectedHubReceiveDate;
                    batchReponse.Status = batch.Status;
                }
                
                var transferResponse = new TransferResponse.TransferResponseSimple();

                if (order.TransferId != null)
                {
                    var transfer = await _unitOfWork.TransferRepository.FilterByExpression(x => x.Id == order.TransferId).FirstOrDefaultAsync();
                    transferResponse.Id = transfer.Id;
                    transferResponse.CollectedId = transfer.CollectedId;
                    transferResponse.StationId = transfer.StationId;
                    transferResponse.CreatedAt = transfer.CreatedAt;
                    transferResponse.UpdatedAt = transfer.UpdatedAt;
                    transferResponse.ExpectedReceiveDate = transfer.ExpectedReceiveDate;
                    transferResponse.ReceivedDate = transfer.ReceivedDate;
                    transferResponse.CreatedBy = transfer.CreatedBy;
                    transferResponse.UpdatedBy = transfer.UpdatedBy;
                    transferResponse.NoteSend = transfer.NoteSend;
                    transferResponse.NoteReceived = transfer.NoteReceived;
                    transferResponse.Code = transfer.Code;
                    transferResponse.Status = transfer.Status;
                }
                
                var orderResponse = new OrderResponse.OrderResponseForStaff()
                {
                    Id = order.Id,
                    Code = order.Code,
                    CustomerId = order.CustomerId,
                    FarmHubId = order.FarmHubId,
                    StationId = order.StationId,
                    BusinessDayId = order.BusinessDayId,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    ExpectedReceiveDate = order.ExpectedReceiveDate,
                    ShipAddress = order.ShipAddress,
                    DeliveryStatus = order.DeliveryStatus,
                    CustomerStatus = order.CustomerStatus,
                    FarmHubResponse = farmHubResponse,
                    BusinessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay),
                    StationResponse = stationResponse,
                    OrderDetailResponse = orderDetailResponses,
                    BatchResponse = batchReponse,
                    TransferResponse = transferResponse
                };
                orderResponses.Add(orderResponse);
            }
            
            result.Message = "Lấy danh sách đơn hàng thành công!";
            result.StatusCode =StatusCode.Ok;
            result.IsError = false;
            result.Payload = orderResponses;
        }
        catch (Exception e)
        {
            result.AddError(StatusCode.ServerError, e.Message);
            result.Message = "Lấy danh sách đơn hàng thất bại!";
            result.IsError = true;
            throw;
        }

        return result;
    }
}