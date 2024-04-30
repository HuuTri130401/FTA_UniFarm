using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;
using Utils = Capstone.UniFarm.Services.ThirdPartyService.Utils;

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

        var adminWallet = await _unitOfWork.WalletRepository
            .FilterByExpression(x => x.AccountId == Guid.Parse("4A846001-C2C0-4EBA-8FCF-F36A8106813F"))
            .FirstOrDefaultAsync();
        if (adminWallet == null)
        {
            result.StatusCode = StatusCode.NotFound;
            result.Message = "Không tìm thấy ví của admin!";
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

                var totalRevenue = item.totalFarmHubPrice;
                // 3. Create order => Create order fail return
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    FarmHubId = item.farmHubId,
                    CustomerId = customer.Id,
                    StationId = orderRequestCreate.stationId,
                    BusinessDayId = orderRequestCreate.businessDayId,
                    TotalAmount = item.totalFarmHubPrice,
                    Code = "OD" + Guid.NewGuid().ToString().Substring(0, 6),
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
                            x => x.ProductItemId == orderDetail.ProductItemId &&
                                 x.Status == EnumConstants.CommonEnumStatus.Active.ToString())
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
                        PaymentDate = DateTime.UtcNow.AddHours(7),
                        Status = "Success",
                        PayerWalletId = wallet.Id,
                        PayeeWalletId = adminWallet.Id,
                        TransactionType = TransactionEnum.Payment.ToString()
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

                // 10. Cập nhật số tiền trong ví admin
                adminWallet.Balance += totalRevenue;
                await _unitOfWork.WalletRepository.UpdateAsync(adminWallet);
                await _unitOfWork.SaveChangesAsync();
            }

            result.Message = "Tạo đơn hàng thành công!";
            result.StatusCode = StatusCode.Created;
            result.IsError = false;
            orderRequestCreate.paymentStatus = "Success";
            orderRequestCreate.paymentAmount = orderRequestCreate.totalAmount;
            result.Payload = orderRequestCreate;
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            result.AddError(StatusCode.ServerError, e.Message);
            result.Message = "Tạo đơn hàng thất bại!";
            result.IsError = true;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff>>> GetAllOrdersOfStaff(
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
                var businessDay = await _unitOfWork.BusinessDayRepository
                    .FilterByExpression(x => x.Id == order.BusinessDayId)
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
                
                var collectedHubResponse = new CollectedHubResponse();
                if (order.CollectedHubId != null)
                {
                    var collectedHub = await _unitOfWork.CollectedHubRepository
                        .FilterByExpression(x => x.Id == order.CollectedHubId)
                        .FirstOrDefaultAsync();
                    collectedHubResponse = _mapper.Map<CollectedHubResponse>(collectedHub);
                }

                var orderDetails = await _unitOfWork.OrderDetailRepository
                    .FilterByExpression(x => x.OrderId == order.Id)
                    .ToListAsync();

                var orderDetailResponses = new List<OrderDetailResponse>();

                foreach (var item in orderDetails)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId); var productImage = await _unitOfWork.ProductImageRepository.FilterByExpression(x => x.ProductItemId == item.ProductItemId && x.DisplayIndex == 1).FirstOrDefaultAsync();
                    var orderDetailResponse = new OrderDetailResponse()
                    {
                        ProductItemId = item.ProductItemId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Unit = item.Unit,
                        TotalPrice = item.TotalPrice,
                        Title = productItem.Title,
                        ProductImage = productImage?.ImageUrl
                    };
                    orderDetailResponses.Add(orderDetailResponse);
                }

                var batchReponse = new BatchResponseSimple();
                if (order.BatchId != null)
                {
                    var batch = await _unitOfWork.BatchesRepository.FilterByExpression(x => x.Id == order.BatchId)
                        .FirstOrDefaultAsync();
                    batchReponse.Id = batch.Id;
                    batchReponse.FarmShipDate = batch.FarmShipDate;
                    batchReponse.CollectedHubReceiveDate = batch.CollectedHubReceiveDate;
                    batchReponse.Status = batch.Status;
                }

                var transferResponse = new TransferResponse.TransferResponseSimple();

                var customer = await _unitOfWork.AccountRepository.GetByIdAsync(order.CustomerId);

                var customerResponse = new AboutMeResponse.CustomerResponseSimple();
                if (customer != null)
                {
                    customerResponse = _mapper.Map<AboutMeResponse.CustomerResponseSimple>(customer);
                }

                if (order.TransferId != null)
                {
                    var transfer = await _unitOfWork.TransferRepository
                        .FilterByExpression(x => x.Id == order.TransferId).FirstOrDefaultAsync();
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
                    TransferResponse = transferResponse,
                    CustomerResponse = customerResponse,
                    CollectedHubResponse = collectedHubResponse
                };
                orderResponses.Add(orderResponse);
            }

            result.Message = "Lấy danh sách đơn hàng thành công!";
            result.StatusCode = StatusCode.Ok;
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

    // Update order status by station staff
    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff?>?>>
        UpdateOrderStatusByStationStaff(
            UpdateOrderStatus.UpdateOrderStatusByTransfer request, AboutMeResponse.AboutMeRoleAndID defineUserPayload)
    {
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff?>?>();
        try
        {
            var transfer = await _unitOfWork.TransferRepository.FilterByExpression(x => x.Id == request.TransferId)
                .FirstOrDefaultAsync();

            if (transfer == null)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không tìm thấy phiếu chuyển hàng!"
                });
                result.IsError = true;
                return result;
            }

            if (transfer.Status == EnumConstants.StationUpdateTransfer.Processed.ToString() ||
                transfer.Status == EnumConstants.StationUpdateTransfer.NotReceived.ToString())
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Phiếu chuyển hàng đã được xử lý trước đó!"
                });
                result.IsError = true;
                return result;
            }

            var orderResponses = new List<OrderResponse.OrderResponseForStaff>();
            foreach (var orderId in request.OrderIds)
            {
                var order = await _unitOfWork.OrderRepository
                    .FilterByExpression(x =>
                        x.Id == orderId && x.TransferId == request.TransferId &&
                        x.StationId == request.StationId)
                    .FirstOrDefaultAsync();
                if (order == null)
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = "Không tìm thấy đơn hàng!" + orderId
                    });
                    result.IsError = true;
                    return result;
                }

                if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString())
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Đơn hàng đã được lấy rồi!" + orderId
                    });
                    result.IsError = true;
                    return result;
                }
                

                if (request.DeliveryStatus == EnumConstants.StationStaffUpdateOrderStatus.AtStation)
                {
                    order.DeliveryStatus = EnumConstants.DeliveryStatus.AtStation.ToString();
                    order.CustomerStatus = EnumConstants.CustomerStatus.ReadyForPickup.ToString();
                    order.ExpiredDayInStation = DateTime.Now + TimeSpan.FromDays(2);
                    order.UpdatedAt = DateTime.Now;
                }
                else if (request.DeliveryStatus == EnumConstants.StationStaffUpdateOrderStatus.StationNotReceived &&
                         order.DeliveryStatus != EnumConstants.StationStaffUpdateOrderStatus.AtStation.ToString())
                {
                    order.DeliveryStatus = EnumConstants.DeliveryStatus.StationNotReceived.ToString();
                    order.UpdatedAt = DateTime.Now;
                }
                else if (request.DeliveryStatus == EnumConstants.StationStaffUpdateOrderStatus.PickedUp)
                {
                    order.DeliveryStatus = EnumConstants.DeliveryStatus.PickedUp.ToString();
                    order.CustomerStatus = EnumConstants.CustomerStatus.PickedUp.ToString();
                    order.UpdatedAt = DateTime.Now;
                    order.ShipByStationStaffId = defineUserPayload.Id;
                }
                else
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Trạng thái không hợp lệ!"
                    });
                    result.IsError = true;
                    return result;
                }

                await _unitOfWork.OrderRepository.UpdateAsync(order);
                var count = await _unitOfWork.SaveChangesAsync();
                if (count == 0)
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Cập nhật trạng thái đơn hàng không thành công!"
                    });
                    result.IsError = true;
                    return result;
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

                var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDay = await _unitOfWork.BusinessDayRepository
                    .FilterByExpression(x => x.Id == order.BusinessDayId)
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

                var batchResponse = new BatchResponseSimple();
                if (order.BatchId != null)
                {
                    var batch = await _unitOfWork.BatchesRepository.FilterByExpression(x => x.Id == order.BatchId)
                        .FirstOrDefaultAsync();
                    batchResponse.Id = batch.Id;
                    batchResponse.FarmShipDate = batch.FarmShipDate;
                    batchResponse.CollectedHubReceiveDate = batch.CollectedHubReceiveDate;
                    batchResponse.Status = batch.Status;
                }

                var transferResponse = new TransferResponse.TransferResponseSimple();
                if (order.TransferId != null)
                {
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

                var customer = await _unitOfWork.AccountRepository.GetByIdAsync(order.CustomerId);
                var customerResponse = new AboutMeResponse.CustomerResponseSimple();
                if (customer != null)
                {
                    customerResponse = _mapper.Map<AboutMeResponse.CustomerResponseSimple>(customer);
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
                    BatchResponse = batchResponse,
                    TransferResponse = transferResponse,
                    CustomerResponse = customerResponse
                };
                orderResponses.Add(orderResponse);
            }

            await AutoUpdateTransferStatus(request.TransferId);
            result.Message = "Cập nhật trạng thái đơn hàng thành công!";
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
            result.Payload = orderResponses;
        }
        catch (Exception e)
        {
            result.AddError(StatusCode.ServerError, e.Message);
            result.Message = "Cập nhật trạng thái đơn hàng thất bại!";
            result.IsError = true;
            throw;
        }

        return result;
    }

    private async Task AutoUpdateTransferStatus(Guid requestTransferId)
    {
        var orders = _unitOfWork.OrderRepository.FilterByExpression(x => x.TransferId == requestTransferId).ToList();
        var count = 0;
        foreach (var order in orders)
        {
            if (order.DeliveryStatus != EnumConstants.DeliveryStatus.AtStation.ToString()
                || order.DeliveryStatus != EnumConstants.DeliveryStatus.StationNotReceived.ToString())
            {
                count++;
            }
        }

        if (count == 0)
        {
            var transfer = _unitOfWork.TransferRepository.FilterByExpression(x => x.Id == requestTransferId)
                .FirstOrDefault();
            if (transfer != null)
            {
                transfer.Status = EnumConstants.StationUpdateTransfer.Processed.ToString();
                _unitOfWork.TransferRepository.Update(transfer);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }

    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer>>> GetAllOrdersOfCustomer(
        bool? isAscending,
        string? orderBy,
        Expression<Func<Order, bool>>? filter = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer>>();
        try
        {
            var orders = await _unitOfWork.OrderRepository
                .FilterAll(isAscending, orderBy, filter, null, pageIndex, pageSize).ToListAsync();
            if (!orders.Any())
            {
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Không tìm thấy đơn hàng nào!";
                result.IsError = false;
                return result;
            }

            var orderResponses = new List<OrderResponse.OrderResponseForCustomer>();
            foreach (var order in orders)
            {
                var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDay = await _unitOfWork.BusinessDayRepository
                    .FilterByExpression(x => x.Id == order.BusinessDayId)
                    .FirstOrDefaultAsync();
                var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                    .FirstOrDefaultAsync();

                var stationResponse = new StationResponse.StationResponseSimple();
                if (station != null)
                {
                    stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                }

                var orderDetails = await _unitOfWork.OrderDetailRepository
                    .FilterByExpression(x => x.OrderId == order.Id)
                    .ToListAsync();

                var orderDetailResponses = new List<OrderDetailResponseForCustomer>();

                foreach (var item in orderDetails)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId);
                    var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                    var productImage = await _unitOfWork.ProductImageRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id && x.DisplayIndex == 1).FirstOrDefaultAsync();
                    if (productImage != null)
                    {
                        productItemResponse.ImageUrl = productImage.ImageUrl;
                    }

                    var orderDetailResponse = new OrderDetailResponseForCustomer()
                    {
                        Id = item.Id,
                        OrderId = item.OrderId,
                        ProductItemId = item.ProductItemId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Unit = item.Unit,
                        TotalPrice = item.TotalPrice,
                        ProductItemResponse = productItemResponse
                    };
                    var productInMenu = await _unitOfWork.ProductItemInMenuRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id
                             && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE).FirstOrDefaultAsync();
                    if (productInMenu == null)
                    {
                        orderDetailResponse.QuantityInStock = 0;
                    }
                    else
                    {
                        orderDetailResponse.QuantityInStock = productInMenu.Quantity - productInMenu.Sold;
                    }

                    orderDetailResponses.Add(orderDetailResponse);
                }

                var orderResponse = new OrderResponse.OrderResponseForCustomer()
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    FarmHubId = order.FarmHubId,
                    StationId = order.StationId,
                    BusinessDayId = order.BusinessDayId,
                    CreatedAt = order.CreatedAt,
                    Code = order.Code,
                    ShipAddress = order.ShipAddress,
                    TotalAmount = order.TotalAmount,
                    IsPaid = order.IsPaid,
                    FullName = order.FullName,
                    PhoneNumber = order.PhoneNumber,
                    DeliveryStatus = order.DeliveryStatus,
                    CustomerStatus = order.CustomerStatus,
                    FarmHubResponse = farmHubResponse,
                    BusinessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay),
                    StationResponse = stationResponse,
                    OrderDetailResponse = orderDetailResponses
                };
                orderResponses.Add(orderResponse);
            }

            result.Message = "Lấy danh sách đơn hàng thành công!";
            result.StatusCode = StatusCode.Ok;
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


    /// <summary>
    /// Các bước thực hiện thanh toán đơn hàng
    /// - Kiểm tra khách hàng có tồn tại không
    /// - Duyệt qua từng order trong request
    /// - Lấy ra tất cả order và order detail trong đơn hàng.
    /// - So sánh số lượng sản phẩm trong order detail với số lượng sản phẩm trong productItemInMenu
    /// - Nếu không đủ số lượng sản phẩm thì rollback và return
    /// - Trừ số lượng sản phẩm trong productItemInMenu
    /// - Tạo ra một ListNewOrder để chứa order mới
    /// - So sánh order trong db và order trong request có đủ số lượng orderDetailId không
    /// - Nếu không đủ số lượng orderDetailId trong 1 order 
    /// -- Tạo ra order mới và thêm orderDetail vào order mới đó
    /// -- cập nhật lại order cũ với số lượng orderDetail còn lại
    /// -- cập nhật lại totalAmount của order cũ và order mới đó
    /// -- Thêm order mới vào ListNewOrder
    /// 
    /// </summary>
    public async Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>?>> Checkout(
        Guid customerId,
        CreateOrderRequest request)
    {
        var result = new OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>?>();
        var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Kiểm tra khách hàng có tồn tại không
            var customer = await _unitOfWork.AccountRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                result.Errors.Add(
                    new Error
                    {
                        Code = StatusCode.NotFound,
                        Message = "Không tìm thấy khách hàng!"
                    }
                );
                result.Message = "Không tìm thấy khách hàng!";
                result.IsError = true;
                return result;
            }

            var listOrderDb = _unitOfWork.OrderRepository
                .FilterByExpression(x => x.CustomerId == customerId
                                         && x.IsPaid == false
                    /*&& x.CreatedAt.Date == DateTime.Now.Date*/
                );


            var newListOrder = new List<Order>();
            // Duyệt qua từng order trong request
            foreach (var requestOrder in request.Orders)
            {
                var orderDb = await listOrderDb.Where(x => x.Id == requestOrder.OrderId).FirstOrDefaultAsync();

                if (orderDb == null)
                {
                    result.Errors.Add(
                        new Error
                        {
                            Code = StatusCode.NotFound,
                            Message = "Không tìm thấy order!" + requestOrder.OrderId
                        }
                    );
                    result.Message = "Không tìm thấy order!";
                    result.IsError = true;
                    return result;
                }

                // Lấy ra tất cả order detail trong 1 order
                var listOrderDetailDbs = _unitOfWork.OrderDetailRepository
                    .FilterByExpression(x => x.OrderId == requestOrder.OrderId);
                var listOrderDetailDbId = listOrderDetailDbs.Select(x => x.Id).ToList();
                var listOrderDetailRequestId = requestOrder.OrderDetailIds.ToList();

                // So sánh số lượng OrderDetailId trong 2 list của 1 order
                if (listOrderDetailDbId.Count == listOrderDetailRequestId.Count)
                {
                    var listNewOrderDetail = new List<OrderDetail>();
                    foreach (var orderDetail in listOrderDetailRequestId)
                    {
                        var orderDetailDb =
                            await listOrderDetailDbs.Where(x => x.Id == orderDetail).FirstOrDefaultAsync();
                        if (orderDetailDb == null)
                        {
                            result.Errors.Add(
                                new Error
                                {
                                    Code = StatusCode.NotFound,
                                    Message = "Không tìm thấy order detail!" + orderDetail
                                }
                            );
                            result.Message = "Không tìm thấy order detail!";
                            result.IsError = true;
                            return result;
                        }
                        listNewOrderDetail.Add(orderDetailDb);
                    }

                    orderDb.TotalAmount = listOrderDetailDbs.Sum(x => x.TotalPrice);
                    orderDb.FullName = request.FullName;
                    orderDb.PhoneNumber = request.PhoneNumber;
                    orderDb.OrderDetails = listNewOrderDetail;
                    orderDb.CustomerStatus = EnumConstants.CustomerStatus.Pending.ToString();
                    orderDb.DeliveryStatus = EnumConstants.CustomerStatus.Pending.ToString();
                    orderDb.IsPaid = false;
                    newListOrder.Add(orderDb);
                }
                else
                {
                    // Trường hợp số lượng orderDetailId trong 1 order không bằng nhau
                    var listNewOrderDetail = new List<OrderDetail>();
                    foreach (var orderDetailId in listOrderDetailRequestId)
                    {
                        var orderDetailDb =
                            await listOrderDetailDbs.Where(x => x.Id == orderDetailId).FirstOrDefaultAsync();
                        if (orderDetailDb == null)
                        {
                            await transaction.RollbackAsync();
                            result.Errors.Add(
                                new Error
                                {
                                    Code = StatusCode.NotFound,
                                    Message = "Không tìm thấy order detail!" + orderDetailId
                                }
                            );
                            result.Message = "Không tìm thấy order detail!";
                            result.IsError = true;
                            return result;
                        }

                        listNewOrderDetail.Add(orderDetailDb);
                    }

                    // Xóa OrderDetail của 1 order dựa vào listNewOrderDetail 
                    foreach (var orderDetailRemove in listNewOrderDetail)
                    {
                        await _unitOfWork.OrderDetailRepository.RemoveAsync(orderDetailRemove);
                        var count = await _unitOfWork.SaveChangesAsync();
                        if (count == 0)
                        {
                            await transaction.RollbackAsync();
                            result.IsError = true;
                            result.Errors.Add(new Error()
                            {
                                Code = StatusCode.BadRequest,
                                Message = "Remove orderDetail In Order Error" + orderDetailRemove.Id
                            });
                            return result;
                        }
                    }

                    // set lại total amount 
                    var orderDetailListAfterRemove =
                        _unitOfWork.OrderDetailRepository.FilterByExpression(x =>
                            x.OrderId == requestOrder.OrderId);
                    orderDb.TotalAmount = orderDetailListAfterRemove.Sum(x => x.TotalPrice);
                    await _unitOfWork.OrderRepository.UpdateAsync(orderDb);
                    var updateOrderCount = await _unitOfWork.SaveChangesAsync();
                    if (updateOrderCount == 0)
                    {
                        await transaction.RollbackAsync();
                        result.IsError = true;
                        result.Errors.Add(new Error()
                        {
                            Code = StatusCode.BadRequest,
                            Message = "Remove Order Error" + orderDb.Id
                        });
                        return result;
                    }

                    var newId = Guid.NewGuid();
                    listNewOrderDetail.ForEach(x => x.OrderId = newId);
                    var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(orderDb.FarmHubId);
                    var order = new Order
                    {
                        Id = newId,
                        FarmHubId = orderDb.FarmHubId,
                        CustomerId = customerId,
                        StationId = request.StationId,
                        BusinessDayId = orderDb.BusinessDayId,
                        TotalAmount = listNewOrderDetail.Sum(x => x.TotalPrice),
                        Code = Utils.GenerateOrderCode(farmHub?.Code!),
                        ExpectedReceiveDate = DateTime.Now + TimeSpan.FromDays(1),
                        ShipAddress = orderDb.ShipAddress,
                        OrderDetails = listNewOrderDetail,
                        FullName = request.FullName,
                        PhoneNumber = request.PhoneNumber,
                        CustomerStatus = EnumConstants.CustomerStatus.Pending.ToString(),
                        DeliveryStatus = EnumConstants.CustomerStatus.Pending.ToString(),
                        IsPaid = false
                    };
                    newListOrder.Add(order);
                }
            }

            // Check số lượng và tăng số lượng sold trong productItemInMenu
            foreach (var order in newListOrder)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var productInMenu = await _unitOfWork.ProductItemInMenuRepository
                        .FilterByExpression(
                            x => x.ProductItemId == orderDetail.ProductItemId
                                 && x.Status == EnumConstants.CommonEnumStatus.Active.ToString()
                        )
                        .FirstOrDefaultAsync();
                    if (productInMenu == null)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "Sản phẩm không tìm thấy hoặc ngừng kinh doanh";
                        result.StatusCode = StatusCode.NotFound;
                        result.IsError = true;
                        return result;
                    }

                    var checkQuantity = productInMenu.Quantity - productInMenu.Sold - orderDetail.Quantity;

                    if (checkQuantity <= 0)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "Hết hàng hoặc không đủ số lượng sản phẩm trong menu!";
                        result.StatusCode = StatusCode.BadRequest;
                        result.IsError = true;
                        return result;
                    }

                    productInMenu.Sold = productInMenu.Sold + orderDetail.Quantity;
                    await _unitOfWork.ProductItemInMenuRepository.UpdateAsync(productInMenu);
                    var updateQuantity = await _unitOfWork.SaveChangesAsync();
                    if (updateQuantity == 0)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "Cập nhật số lượng sản phẩm trong menu không thành công";
                        result.StatusCode = StatusCode.BadRequest;
                        result.IsError = true;
                        return result;
                    }
                }
            }

            // loop qua listNewOrder
            var wallet = await _unitOfWork.WalletRepository.FilterByExpression(x => x.AccountId == customer.Id)
                .FirstOrDefaultAsync();
            if (wallet == null)
            {
                await transaction.RollbackAsync();
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Ví không tồn tại!"
                });
                return result;
            }
            
            // Kiểm tra ví đủ tiền
            var totalAmount = newListOrder.Sum(x => x.TotalAmount);
            if (wallet.Balance < totalAmount)
            {
                await transaction.RollbackAsync();
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Ví không đủ tiền. Vui lòng nạp thêm tiền vào ví!"
                });
                return result;
            }
            
            foreach (var newOrder in newListOrder)
            {
                var checkExistOrder = await _unitOfWork.OrderRepository.FilterByExpression(x => x.Id == newOrder.Id)
                    .FirstOrDefaultAsync();
                var newTransaction = new Transaction()
                {
                    Id = new Guid(),
                    TransactionType = EnumConstants.TransactionEnum.Payment.ToString(),
                    Amount = newOrder.TotalAmount,
                    PaymentDate = DateTime.Now,
                    Status = EnumConstants.TransactionStatus.Success.ToString(),
                    PayerWalletId = wallet!.Id,
                    PayeeWalletId = Guid.Parse(EnumConstants.AdminWallet.AdminWalletId),
                    OrderId = newOrder.Id
                };

                await _unitOfWork.TransactionRepository.AddAsync(newTransaction);
                var countTransaction = await _unitOfWork.SaveChangesAsync();
                if (countTransaction == 0)
                {
                    await transaction.RollbackAsync();
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Tạo giao dịch thất bại!"
                    });
                    return result;
                }

                if (wallet.Balance < newOrder.TotalAmount)
                {
                    await transaction.RollbackAsync();
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Ví không đủ tiền. Vui lòng nạp thêm tiền vào ví!"
                    });
                    return result;
                }

                wallet.Balance -= newOrder.TotalAmount;
                await _unitOfWork.WalletRepository.UpdateAsync(wallet);
                var countWallet = await _unitOfWork.SaveChangesAsync();
                if (countWallet == 0)
                {
                    await transaction.RollbackAsync();
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Cập nhật ví không thành công!"
                    });
                    return result;
                }

                newOrder.IsPaid = true;
                newOrder.ExpectedReceiveDate = DateTime.Now + TimeSpan.FromDays(1);
                if (checkExistOrder == null)
                {
                    await _unitOfWork.OrderRepository.AddAsync(newOrder);
                    var countInsert = await _unitOfWork.SaveChangesAsync();
                    if (countInsert == 0)
                    {
                        await transaction.RollbackAsync();
                        result.IsError = true;
                        result.Errors.Add(new Error()
                        {
                            Code = StatusCode.BadRequest,
                            Message = "Tạo đơn hàng thất bại!" + newOrder.Id
                        });
                        return result;
                    }
                }
                else
                {
                    await _unitOfWork.OrderRepository.UpdateAsync(newOrder);
                    var countUpdate = await _unitOfWork.SaveChangesAsync();
                    if (countUpdate == 0)
                    {
                        await transaction.RollbackAsync();
                        result.IsError = true;
                        result.Errors.Add(new Error()
                        {
                            Code = StatusCode.BadRequest,
                            Message = "Tạo đơn hàng thất bại!" + newOrder.Id
                        });
                        return result;
                    }
                }
            }

            await transaction.CommitAsync();
            var orderResponses = new List<OrderResponse.OrderResponseForCustomer>();
            foreach (var order in newListOrder)
            {
                var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
                var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
                var businessDay = await _unitOfWork.BusinessDayRepository
                    .FilterByExpression(x => x.Id == order.BusinessDayId)
                    .FirstOrDefaultAsync();
                var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                    .FirstOrDefaultAsync();

                var stationResponse = new StationResponse.StationResponseSimple();
                if (station != null)
                {
                    stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                }

                var orderDetails = await _unitOfWork.OrderDetailRepository
                    .FilterByExpression(x => x.OrderId == order.Id)
                    .ToListAsync();

                var orderDetailResponses = new List<OrderDetailResponseForCustomer>();

                foreach (var item in orderDetails)
                {
                    var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId);
                    var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                    var productImage = await _unitOfWork.ProductImageRepository.FilterByExpression(
                        x => x.ProductItemId == productItem!.Id && x.DisplayIndex == 1).FirstOrDefaultAsync();
                    if (productImage != null)
                    {
                        productItemResponse.ImageUrl = productImage.ImageUrl;
                    }
                    var orderDetailResponse = new OrderDetailResponseForCustomer()
                    {
                        Id = item.Id,
                        OrderId = item.OrderId,
                        ProductItemId = item.ProductItemId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Unit = item.Unit,
                        TotalPrice = item.TotalPrice,
                        ProductItemResponse = productItemResponse
                    };
                    orderDetailResponses.Add(orderDetailResponse);
                }

                var orderResponse = new OrderResponse.OrderResponseForCustomer()
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    BusinessDayId = order.BusinessDayId,
                    FarmHubId = order.FarmHubId,
                    StationId = order.StationId,
                    CreatedAt = order.CreatedAt,
                    Code = order.Code,
                    ShipAddress = order.ShipAddress,
                    TotalAmount = order.TotalAmount,
                    IsPaid = order.IsPaid,
                    FullName = order.FullName,
                    PhoneNumber = order.PhoneNumber,
                    FarmHubResponse = farmHubResponse,
                    BusinessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay),
                    StationResponse = stationResponse,
                    OrderDetailResponse = orderDetailResponses
                };
                orderResponses.Add(orderResponse);
            }

            result.Payload = orderResponses;
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
            result.Message = "Tạo đơn hàng thành công!";
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            result.Errors.Add(
                new Error
                {
                    Code = StatusCode.ServerError,
                    Message = "Thanh toán đơn hàng thất bại!"
                }
            );
            result.Message = "Thanh toán đơn hàng thất bại!";
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<OrderResponse.OrderResponseForCustomer>> CancelOrderByCustomer(Guid orderId,
        Guid payloadId)
    {
        var result = new OperationResult<OrderResponse.OrderResponseForCustomer>();
        try
        {
            var order = await _unitOfWork.OrderRepository.FilterByExpression(
                x => x.Id == orderId
                     && x.CustomerId == payloadId
                     && x.IsPaid == true
            ).FirstOrDefaultAsync();

            if (order == null)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không tìm thấy đơn hàng!"
                });
                result.IsError = true;
                return result;
            }

            if (order.CustomerStatus != EnumConstants.CustomerStatus.Pending.ToString())
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Không thể hủy đơn hàng!"
                });
                result.IsError = true;
                return result;
            }

            // Trả lại số lượng sản phẩm trong productItemInMenu
            var orderDetails = await _unitOfWork.OrderDetailRepository.FilterByExpression(x => x.OrderId == orderId)
                .ToListAsync();
            foreach (var orderDetail in orderDetails)
            {
                var productInMenu = await _unitOfWork.ProductItemInMenuRepository.FilterByExpression(
                    x => x.ProductItemId == orderDetail.ProductItemId
                         && x.Menu.BusinessDayId == order.BusinessDayId).FirstOrDefaultAsync();
                if (productInMenu == null)
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = "Không tìm thấy sản phẩm trong menu!"
                    });
                    result.IsError = true;
                    return result;
                }

                productInMenu.Sold -= orderDetail.Quantity;
                await _unitOfWork.ProductItemInMenuRepository.UpdateAsync(productInMenu);
                var count = await _unitOfWork.SaveChangesAsync();
                if (count == 0)
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Cập nhật số lượng sản phẩm trong menu không thành công!"
                    });
                    result.IsError = true;
                    return result;
                }
            }

            order.CustomerStatus = EnumConstants.CustomerStatus.CanceledByCustomer.ToString();
            order.UpdatedAt = DateTime.Now;
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            var countUpdate = await _unitOfWork.SaveChangesAsync();
            if (countUpdate == 0)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.BadRequest,
                    Message = "Cập nhật trạng thái đơn hàng không thành công!"
                });
                result.IsError = true;
                return result;
            }

            var transaction = await _unitOfWork.TransactionRepository.FilterByExpression(x => x.OrderId == orderId)
                .FirstOrDefaultAsync();
            if (transaction != null)
            {
                transaction.TransactionType = EnumConstants.TransactionEnum.Refund.ToString();
                await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
                var countTransaction = await _unitOfWork.SaveChangesAsync();

                if (countTransaction == 0)
                {
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Cập nhật giao dịch không thành công!"
                    });
                    result.IsError = true;
                    return result;
                }
            }

            var farmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(order.FarmHubId);
            var farmHubResponse = _mapper.Map<FarmHubResponse>(farmHub);
            var businessDay = await _unitOfWork.BusinessDayRepository
                .FilterByExpression(x => x.Id == order.BusinessDayId)
                .FirstOrDefaultAsync();
            var station = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == order.StationId)
                .FirstOrDefaultAsync();

            var stationResponse = new StationResponse.StationResponseSimple();
            if (station != null)
            {
                stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
            }

            var orderDetailResponses = new List<OrderDetailResponseForCustomer>();
            foreach (var item in orderDetails)
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId);
                var productItemResponse = _mapper.Map<ProductItemResponseForCustomer>(productItem);
                var orderDetailResponse = new OrderDetailResponseForCustomer()
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    ProductItemId = item.ProductItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Unit = item.Unit,
                    TotalPrice = item.TotalPrice,
                    ProductItemResponse = productItemResponse
                };
                orderDetailResponses.Add(orderDetailResponse);
            }

            var orderResponse = new OrderResponse.OrderResponseForCustomer()
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                FarmHubId = order.FarmHubId,
                StationId = order.StationId,
                BusinessDayId = order.BusinessDayId,
                CreatedAt = order.CreatedAt,
                Code = order.Code,
                ShipAddress = order.ShipAddress,
                TotalAmount = order.TotalAmount,
                IsPaid = order.IsPaid,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                DeliveryStatus = order.DeliveryStatus,
                CustomerStatus = order.CustomerStatus,
                FarmHubResponse = farmHubResponse,
                BusinessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay),
                StationResponse = stationResponse,
                OrderDetailResponse = orderDetailResponses
            };
            result.Payload = orderResponse;
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
            result.Message = "Hủy đơn hàng thành công!";
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = "Cancel order failure!"
            });
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<TrackingOrderResponse>>> TrackingOrder(Guid orderId, Guid payloadId)
    {
        var result = new OperationResult<IEnumerable<TrackingOrderResponse>>();
        try
        {
            var order = _unitOfWork.OrderRepository
                .FilterByExpression(x => x.Id == orderId && x.CustomerId == payloadId)
                .FirstOrDefault();
            if (order == null)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không tìm thấy đơn hàng!"
                });
                result.IsError = true;
                return result;
            }

            var trackingOrderResponses = HandleTrackingOrder(order);
            result.Payload = trackingOrderResponses;
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
            result.Message = "Lấy thông tin đơn hàng thành công!";
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = "Lấy thông tin đơn hàng thất bại!"
            });
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<Order?> GetOrderById(Guid orderId)
    {
        return await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
    }
    
    private IEnumerable<TrackingOrderResponse>? HandleTrackingOrder(Order order)
    {
        // Đơn hàng đã xác nhận lúc
        var confirmed = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã được duyệt!",
            OrderStatus = EnumConstants.DeliveryStatus.Confirmed,
            UpdatedAt = null
        };

        // Đơn hàng đã bị hủy bởi người bán 
        var canceledByFarmhub = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã bị hủy bởi người bán!",
            OrderStatus = EnumConstants.DeliveryStatus.CanceledByFarmHub,
            UpdatedAt = order.UpdatedAt
        };

        // Đơn hàng đang được vận chuyển đến kho
        var onTheWayToCollectedHub = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đang được vận chuyển đến kho!",
            OrderStatus = EnumConstants.DeliveryStatus.OnTheWayToCollectedHub,
            UpdatedAt = null
        };

        // Đơn hàng đã đến kho
        var atCollectedHub = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã đến kho!",
            OrderStatus = EnumConstants.DeliveryStatus.AtCollectedHub,
            UpdatedAt = null
        };

        var onTheWaytoStation = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đang được vận chuyển đến trạm!",
            OrderStatus = EnumConstants.DeliveryStatus.OnTheWayToStation,
            UpdatedAt = null
        };

        var atStation = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã đến trạm!",
            OrderStatus = EnumConstants.DeliveryStatus.AtStation,
            UpdatedAt = null
        };

        var readyForPickup = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đang chờ lấy!",
            OrderStatus = EnumConstants.DeliveryStatus.ReadyForPickup,
            UpdatedAt = null
        };

        var pickedUp = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã được lấy!",
            OrderStatus = EnumConstants.DeliveryStatus.PickedUp,
            UpdatedAt = null
        };

        var canceledByCollectedHub = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã bị hủy bởi hệ thống!",
            OrderStatus = EnumConstants.DeliveryStatus.CanceledByCollectedHub,
            UpdatedAt = order.UpdatedAt
        };
        
        var canceledByCustomer = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đã bị hủy!",
            OrderStatus = EnumConstants.DeliveryStatus.CanceledByCustomer,
            UpdatedAt = order.UpdatedAt
        };
        
        var pendingOrder = new TrackingOrderResponse()
        {
            Title = "Đơn hàng đang chờ xác nhận!",
            OrderStatus = EnumConstants.DeliveryStatus.Pending,
            UpdatedAt = null
        };
        var batch = new Batch();
        if (order.BatchId != null)
        {
            batch = _unitOfWork.BatchesRepository.FilterByExpression(x => x.Id == order.BatchId)
                .FirstOrDefault();
        }

        var transfer = new Transfer();
        if (order.TransferId != null)
        {
            transfer = _unitOfWork.TransferRepository.FilterByExpression(x => x.Id == order.TransferId)
                .FirstOrDefault();
        }
        
        var trackingOrderResponses = new List<TrackingOrderResponse>();
       
        if(order.CustomerStatus == EnumConstants.DeliveryStatus.Pending.ToString())
        {
            trackingOrderResponses.Add(pendingOrder);
            return trackingOrderResponses;
        }
        
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.CanceledByFarmHub.ToString())
        {
            trackingOrderResponses.Add(canceledByFarmhub);
            return trackingOrderResponses;
        }
        
        if (order.CustomerStatus == EnumConstants.DeliveryStatus.CanceledByCustomer.ToString())
        {
            trackingOrderResponses.Add(canceledByCustomer);
            return trackingOrderResponses;
        }
        
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.CanceledByCollectedHub.ToString())
        {
            trackingOrderResponses.Add(canceledByCollectedHub);
            return trackingOrderResponses;
        }
        
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.CanceledByCustomer.ToString())
        {
            canceledByCollectedHub.UpdatedAt = order.UpdatedAt;
            trackingOrderResponses.Add(canceledByCollectedHub);
            return trackingOrderResponses;
        }
        
        // Đơn hàng đã được xác nhận
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToCollectedHub.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtCollectedHub.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()
           )
        {
            trackingOrderResponses.Add(confirmed);
        }

        //Đơn hàng đang đến kho
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToCollectedHub.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtCollectedHub.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()
           )
        {
            onTheWayToCollectedHub.UpdatedAt = batch?.FarmShipDate;
            //trackingOrderResponses.Add(onTheWayToCollectedHub);
        }

        // Đơn hàng đã đến kho
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtCollectedHub.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()
           )
        {
            if (batch?.Status == EnumConstants.BatchStatus.Received.ToString() ||
                batch?.Status == EnumConstants.BatchStatus.Processed.ToString() &&
                order.CollectedHubId != null)
            {
                atCollectedHub.UpdatedAt = batch?.CollectedHubReceiveDate;
                //trackingOrderResponses.Add(atCollectedHub);
            }
        }

        // Đơn hàng đang đến trạm
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()
           )
        {
            onTheWaytoStation.UpdatedAt = transfer?.CreatedAt;
            //trackingOrderResponses.Add(onTheWaytoStation);
        }
        
        // Đơn hàng đã đến trạm
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()
           )
        {
            atStation.UpdatedAt = transfer?.ReceivedDate;
            //trackingOrderResponses.Add(atStation);
        }
        
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
            || order.DeliveryStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString()
           )
        {
            readyForPickup.UpdatedAt = transfer?.ReceivedDate;
            //trackingOrderResponses.Add(readyForPickup);
        }
        
        if (order.DeliveryStatus == EnumConstants.DeliveryStatus.PickedUp.ToString()
            || order.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()
           )
        {
            pickedUp.UpdatedAt = order?.UpdatedAt;
            //trackingOrderResponses.Add(pickedUp);
        }
        
        trackingOrderResponses.Add(onTheWayToCollectedHub);
        trackingOrderResponses.Add(atCollectedHub);
        trackingOrderResponses.Add(onTheWaytoStation);
        trackingOrderResponses.Add(atStation);
        trackingOrderResponses.Add(readyForPickup);
        trackingOrderResponses.Add(pickedUp);
        return trackingOrderResponses;
    }
}