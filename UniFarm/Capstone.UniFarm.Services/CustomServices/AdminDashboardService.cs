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

    public AdminDashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
                    Message = "Business day not found"
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
                    Message = "Order not found"
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
}