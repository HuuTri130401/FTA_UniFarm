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
                TotalOrderSuccess =
                    orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()),
                TotalOrderCancelByCustomer = orderList.Count(x =>
                    x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByCustomer.ToString()),
                TotalOrderCancelByFarm = orderList.Count(x =>
                    x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByFarmHub.ToString()),
                TotalOrderCancelBySystem = orderList.Count(x =>
                    x.DeliveryStatus == EnumConstants.DeliveryStatus.CanceledByCollectedHub.ToString()),
                TotalOrderExpired =
                    orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.Expired.ToString()),
                TotalOrderPending =
                    orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.Pending.ToString()),
                TotalOrderConfirmed =
                    orderList.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.Confirmed.ToString())
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

    public async Task<OperationResult<OrderResponse.OrderAndOrderDetailResponse>> GetOrderDetail(Guid businessDayId,
        Guid orderId)
    {
        var result = new OperationResult<OrderResponse.OrderAndOrderDetailResponse>();
        try
        {
            var order = await _unitOfWork.OrderRepository
                .FilterByExpression(x => x.Id == orderId && x.BusinessDayId == businessDayId).FirstOrDefaultAsync();
            if (order == null)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không tìm thấy đơn hàng!"
                });
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
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

            var orderDetails = await _unitOfWork.OrderDetailRepository
                .FilterByExpression(x => x.OrderId == order.Id)
                .ToListAsync();

            var orderDetailResponses = new List<OrderDetailResponse>();
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = "Lấy chi tiết đơn hàng thất bại!"
            });
            result.IsError = true;
            throw;
        }

        return result;
    }


    /// <summary>
    /// - Lấy thống kê doanh thu theo tháng
    /// - Tổng doanh thu
    /// - Tổng tiền đặt cọc
    /// - Tổng tiền rút
    /// - Tổng tiền hoàn
    /// - Tổng lợi nhuận
    /// - Tổng số đơn hàng
    /// - Tổng số đơn hàng thành công
    /// - Tổng số đơn hàng hủy
    /// - Tổng số đơn hàng hết hạn
    /// </summary>
    /// <returns></returns>
    public async Task<OperationResult<IEnumerable<AdminDashboardResponse.RevenueByMonth>>> GetStatisticByMonth()
    {
        var result = new OperationResult<IEnumerable<AdminDashboardResponse.RevenueByMonth>>();
        try
        {
            var orderList = await _unitOfWork.OrderRepository.GetAllWithoutPaging(isAscending: true, EnumConstants.FilterOrder.CreatedAt.ToString())
                .ToListAsync();
            if (orderList.Count == 0)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không có đơn hàng nào!"
                });
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Không có đơn hàng nào!";
                result.IsError = true;
                return result;
            }

            var paymentList = await _unitOfWork.PaymentRepository.GetAllWithoutPaging(null, null).ToListAsync();

            var revenueByMonths = new List<AdminDashboardResponse.RevenueByMonth>();
            var months = orderList.Select(x => x.CreatedAt.Month).Distinct().ToList();
            foreach (var month in months)
            {
                var orders = orderList.Where(x => x.CreatedAt.Month == month).ToList();
                var payments = paymentList.Where(x => x.PaymentDay.Month == month).ToList();
                var revenueByMonth = new AdminDashboardResponse.RevenueByMonth()
                {
                    Month = month.ToString(),
                    TotalRevenue = orders.Sum(x => x.TotalAmount ?? 0),
                    TotalBenefit = orders.Sum(x => x.TotalAmount ?? 0),
                    TotalOrder = orders.Count,
                    TotalOrderSuccess = orders.Count(x =>
                        x.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString()),
                    TotalOrderCancel = orders.Count(x =>
                        x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByCustomer.ToString()
                        || x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByFarmHub.ToString()
                        || x.CustomerStatus == EnumConstants.CustomerStatus.CanceledByCollectedHub.ToString()),
                    TotalOrderExpired = orders.Count(x =>
                        x.CustomerStatus == EnumConstants.CustomerStatus.Expired.ToString())
                };
                revenueByMonth.TotalDepositMoney = payments
                    .Where(x => x.Type == EnumConstants.PaymentType.Deposit.ToString() && x.PaymentDay.Month == month).Sum(x => x.Amount);
                revenueByMonth.TotalWithdrawMoney = payments
                    .Where(x => x.Type == EnumConstants.PaymentType.Withdraw.ToString() && x.PaymentDay.Month == month).Sum(x => x.Amount);
                revenueByMonth.TotalRefundMoney = payments
                    .Where(x => x.Type == EnumConstants.PaymentType.Refund.ToString() && x.PaymentDay.Month == month).Sum(x => x.Amount);
                revenueByMonths.Add(revenueByMonth);
            }

            result.Payload = revenueByMonths;
            result.StatusCode = StatusCode.Ok;
            result.Message = "Lấy thống kê doanh thu theo tháng thành công!";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
            result.StatusCode = StatusCode.ServerError;
            result.Message = "Lấy thống kê doanh thu theo tháng thất bại!";
            result.IsError = true;
            throw;
        }

        return result;
    }

    /// <summary>
    /// - Lấy thống kê sản phẩm bán chạy theo thang
    /// - Tên sản phẩm
    /// - Phần trăm
    /// - Lấy tất cả businessId theo thời gian từ fromDate đến toDate trong BusinessDay
    /// - Lấy tất cả menu theo businessdayId trong Menu
    /// - Lấy tất cả sản phẩm trong ProductItemInMenu theo menuId
    /// - Tính phần trăm bán chạy của sản phẩm theo số lượng bán ra
    /// </summary>
    public async Task<OperationResult<IEnumerable<AdminDashboardResponse.ProductSellingPercent>>> GetProductSellingPercent(DateTime? fromDate, DateTime? toDate)
    {
        var result = new OperationResult<IEnumerable<AdminDashboardResponse.ProductSellingPercent>>();
        try
        {
            var businessDays = await _unitOfWork.BusinessDayRepository.FilterByExpression(x => x.OpenDay >= fromDate && x.OpenDay <= toDate)
                .ToListAsync();
            if (businessDays.Count == 0)
            {
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Không có ngày kinh doanh nào!"
                });
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Không có ngày kinh doanh nào!";
                result.IsError = true;
                return result;
            }

            var menuList = await _unitOfWork.MenuRepository.GetAllWithoutPaging(null, null).ToListAsync();
            var productItemInMenuList = await _unitOfWork.ProductItemInMenuRepository.GetAllWithoutPaging(null, null).ToListAsync();

            var productItemSellingPercentRatios = new List<AdminDashboardResponse.ProductSellingPercent>();
            double totalQuantitySold = 0;
            
            var newProductItemInMenuList = new List<ProductItemInMenu>();

            foreach (var businessDay in businessDays)
            {
                var menu = menuList.FirstOrDefault(x => x.BusinessDayId == businessDay.Id);
                if (menu == null)
                {
                    continue;
                }

                var productItemInMenus = productItemInMenuList.Where(x => x.MenuId == menu.Id).ToList();
                if (productItemInMenus.Count == 0)
                {
                    continue;
                }
                
                newProductItemInMenuList.AddRange(productItemInMenus);
                totalQuantitySold += productItemInMenus.Sum(x => x.Sold ?? 0);
            }

            var newProductItemUnique = newProductItemInMenuList.GroupBy(x => x.ProductItemId)
                .Select(x => new AdminDashboardResponse.ProductSellingPercent()
                {
                    ProductItemId = x.First().ProductItemId,
                    SoldQuantity = x.Sum(y => y.Sold ?? 0),
                    Percent = Math.Round(x.Sum(y => y.Sold ?? 0) / totalQuantitySold * 100, 2)
                }).ToList();
            

            // check duplicate product item then sum percent and get one
            foreach (var item in newProductItemUnique)
            {
                item.ProductName = (await _unitOfWork.ProductItemRepository.GetByIdAsync(item.ProductItemId))!.Title;
            }
            
            
            result.Payload = newProductItemUnique;
            result.StatusCode = StatusCode.Ok;
            result.Message = "Lấy thống kê sản phẩm bán chạy thành công!";
            result.IsError = false;
        }
        catch (Exception e)
        {
            result.Errors.Add(new Error()
            {
                Code = StatusCode.ServerError,
                Message = e.Message
            });
            result.StatusCode = StatusCode.ServerError;
            result.Message = "Lấy thống kê sản phẩm bán chạy thất bại!";
            result.IsError = true;
            throw;
        }

        return result;
    }
}