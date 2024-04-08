using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminDashboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<AdminDashboardResponse.OrderStatisticByBusinessDay>>
        GetOrderStatisticByBusinessDay(Guid? id, DateTime? searchDay)
    {
        var result = new OperationResult<AdminDashboardResponse.OrderStatisticByBusinessDay>();
        try
        {
            var businessDay = new BusinessDay();
            if (id != null)
            {
                businessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(id.Value);
            }

            if (searchDay != null)
            {
                businessDay = await _unitOfWork.BusinessDayRepository.FilterByExpression(x => x.OpenDay == searchDay)
                    .FirstOrDefaultAsync();
            }

            if (businessDay == null)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không tìm thấy ngày kinh doanh!"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var orderList = await _unitOfWork.OrderRepository.FilterByExpression(x => x.BusinessDayId == businessDay.Id)
                .ToListAsync();

            if (orderList.Count == 0)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Ngày kinh doanh không có đơn hàng!"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var orderStatistic = new AdminDashboardResponse.OrderStatisticByBusinessDay()
            {
                TotalRevenue = orderList.Sum(x => x.TotalAmount ?? 0),
                TotalOrder = orderList.Count,
                TotalOrderDelivering = orderList.Count(x =>
                    x.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToCollectedHub.ToString()
                    || x.DeliveryStatus == EnumConstants.DeliveryStatus.AtCollectedHub.ToString()
                    || x.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString()
                    || x.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()
                ),
                TotalOrderSuccess = orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()),
                TotalOrderCancelByCustomer = orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByCustomer.ToString()),
                TotalOrderCancelByFarm = orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByFarmHub.ToString()),
                TotalOrderCancelBySystem = orderList.Count(x => x.DeliveryStatus == EnumConstants.DeliveryStatus.CanceledByCollectedHub.ToString()),
                TotalOrderExpired = orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.Expired.ToString()),
                TotalOrderPending = orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.Pending.ToString()),
                TotalOrderConfirmed = orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.Confirmed.ToString())
            };
            
            result.Payload = orderStatistic;
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
            result.Message = "Lấy thống kê đơn hàng theo ngày kinh doanh thành công!";
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            throw;
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
                    customerResponse= _mapper.Map<AboutMeResponse.CustomerResponseSimple>(customer);
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
                    CustomerResponse = customerResponse
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
}