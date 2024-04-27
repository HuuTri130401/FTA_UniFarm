using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class StationService : IStationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAccountRoleService _accountRoleService;
    private readonly UserManager<Account> _userManager;

    public StationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAccountRoleService accountRoleService,
        UserManager<Account> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _accountRoleService = accountRoleService;
        _userManager = userManager;
    }

    public async Task<OperationResult<IEnumerable<StationResponse>>> GetAll(bool? isAscending, string? orderBy = null,
        Expression<Func<Station, bool>>? filter = null,
        Expression<Func<Order, bool>>? filterOrder = null,
        string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<StationResponse>>();
        try
        {
            var stations = await _unitOfWork.StationRepository.FilterAll(isAscending, orderBy, filter,
                includeProperties,
                pageIndex, pageSize).ToListAsync();
            if (!stations.Any())
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var stationResponse = _mapper.Map<IEnumerable<StationResponse>>(stations);
            foreach (var station in stationResponse)
            {
                Expression<Func<Order, bool>> expression2 = order => order.StationId == station.Id;
                if (filterOrder != null)
                {
                    ParameterExpression parameter = Expression.Parameter(typeof(Order), "Order");
                    var combinedExpression = Expression.Lambda<Func<Order, bool>>(
                        Expression.AndAlso(
                            Expression.Invoke(filterOrder, parameter),
                            Expression.Invoke(expression2, parameter)
                        ),
                        parameter
                    );
                    station.TotalOrderPending = _unitOfWork.OrderRepository.GetAllWithoutPaging(false, null,
                        combinedExpression).ToListAsync().Result.Count;
                }
                else
                {
                    station.TotalOrderPending = _unitOfWork.OrderRepository.GetAllWithoutPaging(false, null,
                        x => x.StationId == station.Id
                             && x.DeliveryStatus == EnumConstants.DeliveryStatus.AtCollectedHub.ToString()
                             && x.IsPaid == true).ToListAsync().Result.Count;
                }
            }

            result.Payload = stationResponse;
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get stations error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }

    public Task<OperationResult<IEnumerable<StationResponse>>> GetAllWithoutPaging(bool? isAscending,
        string? orderBy = null, Expression<Func<Station, bool>>? filter = null,
        string[]? includeProperties = null)
    {
        var result = new OperationResult<IEnumerable<StationResponse>>();
        try
        {
            var stations =
                _unitOfWork.StationRepository.GetAllWithoutPaging(isAscending, orderBy, filter, includeProperties);
            if (!stations.Any())
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return Task.FromResult(result);
            }

            result.Payload = _mapper.Map<IEnumerable<StationResponse>>(stations);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get stations error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return Task.FromResult(result);
    }

    public async Task<OperationResult<StationResponse>> GetById(Guid objectId)
    {
        var result = new OperationResult<StationResponse>();
        try
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(objectId);
            if (station == null)
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            result.Payload = _mapper.Map<StationResponse>(station);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get station by Id Error" + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }

    public Task<OperationResult<StationResponse>> GetFilterByExpression(Expression<Func<Station, bool>> filter,
        string[]? includeProperties = null)
    {
        var result = new OperationResult<StationResponse>();
        try
        {
            var station = _unitOfWork.StationRepository.FilterByExpression(filter, null).FirstOrDefaultAsync().Result;
            if (station == null)
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return Task.FromResult(result);
            }

            result.Payload = _mapper.Map<StationResponse>(station);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get station by filter error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return Task.FromResult(result);
    }

    public async Task<OperationResult<StationResponse>> Create(StationRequestCreate objectRequestCreate)
    {
        var result = new OperationResult<StationResponse>();
        try
        {
            var station = _mapper.Map<Station>(objectRequestCreate);
            var stationCreated = await _unitOfWork.StationRepository.AddAsync(station);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.Message = "Create station failed";
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            stationCreated.Area = await _unitOfWork.AreaRepository.GetByIdAsync(objectRequestCreate.AreaId);
            var stationResponse = _mapper.Map<StationResponse>(stationCreated);
            stationResponse.Area = _mapper.Map<Area>(stationCreated.Area);
            result.Payload = stationResponse;
            result.StatusCode = StatusCode.Created;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Create station error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<bool>> Delete(Guid id)
    {
        var result = new OperationResult<bool>();
        try
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
            if (station == null)
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            _unitOfWork.StationRepository.SoftRemove(station);
            var count = _unitOfWork.SaveChangesAsync().Result;
            if (count > 0)
            {
                result.Payload = true;
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.Message = "Delete station failed";
                result.StatusCode = StatusCode.BadRequest;
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Delete station error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<StationResponse.StationResponseSimple>> Update(Guid id,
        StationRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<StationResponse.StationResponseSimple>();
        try
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
            if (station == null)
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            _mapper.Map(objectRequestUpdate, station);
            station.Area = null;
            _unitOfWork.StationRepository.Update(station);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count > 0)
            {
                var stationResponse = _mapper.Map<StationResponse.StationResponseSimple>(station);
                result.Payload = stationResponse;
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.Message = "Update station failed";
                result.StatusCode = StatusCode.BadRequest;
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Update station error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }


    public async Task<OperationResult<IEnumerable<AboutMeResponse.StaffResponse>>>
        GetStationStaffsData(
            Guid id,
            bool? isAscending,
            string? orderBy,
            Expression<Func<Account, bool>>? filter,
            string[]? includeProperties, int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AboutMeResponse.StaffResponse>>();
        try
        {
            var stationHub = await _unitOfWork.StationRepository.GetByIdAsync(id);
            if (stationHub == null)
            {
                result.Message = "StationHub not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }

            var accountRole = await _accountRoleService.GetAllWithoutPaging(isAscending, orderBy, x =>
                x.StationId == id && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
            if (accountRole.Payload == null)
            {
                result.Message = "Does not have any staff in this Station Hub";
                result.StatusCode = StatusCode.Ok;
                return result;
            }

            var accountIds = accountRole.Payload.Select(x => x.AccountId).ToList();
            var stationStaffs = _userManager.Users
                .Where(x => accountIds.Contains(x.Id) && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var stationStaffsResponse =
                _mapper.Map<IEnumerable<AboutMeResponse.StaffResponse>>(stationStaffs);
            result.Payload = stationStaffsResponse;
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get all staffs in stationHub success";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get all staffs in stationHub error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<AboutMeResponse.StaffResponse>>>
        GetStationStaffsNotWorking(
            bool? isAscending,
            string? orderBy,
            Expression<Func<Account, bool>>? filter,
            string[]? includeProperties,
            int pageIndex = 0,
            int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AboutMeResponse.StaffResponse>>();
        try
        {
            var accountRole = await _accountRoleService.GetAllWithoutPaging(
                isAscending,
                orderBy,
                x => x.StationId != null && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);

            var accountRoleIds = accountRole.Payload.Select(x => x.AccountId).ToList();

            var stationStaffs = _userManager.GetUsersInRoleAsync(EnumConstants.RoleEnumString.STATIONSTAFF).Result;
            var stationStaffIds = stationStaffs.Select(x => x.Id).ToList().Except(accountRoleIds).ToList();

            stationStaffs = _userManager.Users
                .Where(x => stationStaffIds.Contains(x.Id) && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var stationStaffsResponse =
                _mapper.Map<IEnumerable<AboutMeResponse.StaffResponse>>(stationStaffs);

            if (!stationStaffsResponse.Any())
            {
                result.Message = "Nobody. All station staffs are working!";
                result.StatusCode = StatusCode.Ok;
                return result;
            }

            result.Payload = stationStaffsResponse;
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get station staff not working success";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get station staff not working error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<StationResponse.StationDashboardResponse?>?> ShowDashboard(DateTime addDays,
        DateTime today, AboutMeResponse.AboutMeRoleAndID defineUserPayload)
    {
        var result = new OperationResult<StationResponse.StationDashboardResponse?>();
        try
        {
            var station =
                await _unitOfWork.StationRepository.GetByIdAsync(Guid.Parse(defineUserPayload.AuthorizationDecision));
            if (station == null)
            {
                result.Message = "Station not found";
                result.StatusCode = StatusCode.NotFound;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Station not found"
                });
                return result;
            }

            var orders = await _unitOfWork.OrderRepository.GetAllWithoutPaging(null, "CreatedAt",
                x => x.StationId == station.Id && x.CreatedAt.Date <= today.Date && x.CreatedAt >= addDays.Date &&
                     x.IsPaid == true, null).ToListAsync();
            var totalOrderOnTheWayToStation = orders.Count(x =>
                x.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString());
            var totalOrderAtStation =
                orders.Count(x => x.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString());
            var totalOrderReadyForPickup = orders.Count(x =>
                x.CustomerStatus == EnumConstants.DeliveryStatus.ReadyForPickup.ToString());
            var totalOrderStationNotReceived = orders.Count(x =>
                x.DeliveryStatus == EnumConstants.DeliveryStatus.StationNotReceived.ToString());
            var totalOrderPickedUp =
                orders.Count(x => x.CustomerStatus == EnumConstants.CustomerStatus.PickedUp.ToString());
            var totalOrderExpired = orders.Count(x =>
                x.CustomerStatus == EnumConstants.CustomerStatus.Expired.ToString() ||
                x.DeliveryStatus == EnumConstants.CustomerStatus.Expired.ToString());
            var totalOrderStaffHandled = orders.Count(x => x.ShipByStationStaffId == defineUserPayload.Id);

            var transfers = await _unitOfWork.TransferRepository.GetAllWithoutPaging(null, null,
                    x => x.StationId == station.Id && x.CreatedAt <= today.Date && x.CreatedAt >= addDays.Date, null)
                .ToListAsync();

            var totalTransferPending = transfers.Count(x =>
                x.Status == EnumConstants.StationUpdateTransfer.Pending.ToString() ||
                x.Status == EnumConstants.StationUpdateTransfer.Resend.ToString());
            var totalTransferReceived =
                transfers.Count(x => x.Status == EnumConstants.StationUpdateTransfer.Received.ToString());
            var totalTransferNotReceived =
                transfers.Count(x => x.Status == EnumConstants.StationUpdateTransfer.NotReceived.ToString());
            var totalTransferProcessed =
                transfers.Count(x => x.Status == EnumConstants.StationUpdateTransfer.Processed.ToString());

            var stationDashboardResponse = new StationResponse.StationDashboardResponse
            {
                TotalOrder = orders.Count,
                TotalOrderOnTheWayToStation = totalOrderOnTheWayToStation,
                TotalOrderAtStation = totalOrderAtStation,
                TotalOrderReadyForPickup = totalOrderReadyForPickup,
                TotalOrderStationNotReceived = totalOrderStationNotReceived,
                TotalOrderPickedUp = totalOrderPickedUp,
                TotalOrderExpired = totalOrderExpired,
                TotalOrderStaffHandled = totalOrderStaffHandled,
                TotalTransferPending = totalTransferPending,
                TotalTransferReceived = totalTransferReceived,
                TotalTransferNotReceived = totalTransferNotReceived,
                TotalTransferProcessed = totalTransferProcessed
            };

            result.Payload = stationDashboardResponse;
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get dashboard success";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get dashboard error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<StationNotificationResponse?>?>> GetNotificationsForStationStaff(
        AboutMeResponse.AboutMeRoleAndID defineUserPayload, int pageIndex, int pageSize)
    {
        var result = new OperationResult<IEnumerable<StationNotificationResponse?>?>();
        try
        {
            var station =
                await _unitOfWork.StationRepository.GetByIdAsync(Guid.Parse(defineUserPayload.AuthorizationDecision));
            if (station == null)
            {
                result.Message = "Station not found";
                result.StatusCode = StatusCode.NotFound;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.NotFound,
                    Message = "Station not found"
                });
                return result;
            }

            var transfer = await _unitOfWork.TransferRepository.FilterAll(false, "CreatedAt",
                x => x.StationId == station.Id 
                     /*&& (x.Status != EnumConstants.StationUpdateTransfer.Received.ToString() ||
                      x.Status != EnumConstants.StationUpdateTransfer.NotReceived.ToString())*/
                ,
                null, pageIndex, pageSize).ToListAsync();

            var response = new List<StationNotificationResponse>();
            foreach (var item in transfer)
            {
                var title = "";
                var content = "";
                var type = EnumConstants.NotificationType.Transfer;
                if (item.UpdatedAt != null || item.ReceivedDate != null)
                {
                    content = item.Status == EnumConstants.StationUpdateTransfer.Pending.ToString()
                        ?
                        "Chuyến hàng " + item.Code + " đang được gửi đến trạm!"
                        : item.Status == EnumConstants.StationUpdateTransfer.Received.ToString()
                            ? "Đã nhận chuyến hàng " + item.Code + " ở trạm!"
                            : item.Status == EnumConstants.StationUpdateTransfer.NotReceived.ToString()
                                ? "Không nhận được chuyến hàng " + item.Code + " !"
                                : item.Status == EnumConstants.StationUpdateTransfer.Resend.ToString()
                                    ? "Chuyến hàng " + item.Code + " đã gửi lại và đang vận chuyển đến trạm!"
                                    : "Chuyến hàng " + item.Code + " đã được xử lý!";
                    title = item.Status == EnumConstants.StationUpdateTransfer.Pending.ToString()
                        ? EnumConstants.NotificationType.Pending.ToString()
                        : item.Status == EnumConstants.StationUpdateTransfer.Resend.ToString()
                            ? EnumConstants.NotificationType.Resend.ToString()
                            : item.Status == EnumConstants.StationUpdateTransfer.Received.ToString()
                                ? EnumConstants.NotificationType.Received.ToString()
                                : item.Status == EnumConstants.StationUpdateTransfer.NotReceived.ToString()
                                    ? EnumConstants.NotificationType.NotReceived.ToString()
                                    : item.Status == EnumConstants.NotificationType.Processed.ToString()
                                        ? EnumConstants.NotificationType.Processed.ToString()
                                        : EnumConstants.NotificationType.Transfer.ToString();
                    var notification = new StationNotificationResponse
                    {
                        StationId = item.StationId,
                        Title = title,
                        Content = content,
                        UpdatedAt = item.UpdatedAt ?? item.ReceivedDate ?? DateTime.Now,
                        Code = item.Code ?? "EMPTY",
                        NotificationType = EnumConstants.NotificationType.Transfer
                    };
                    response.Add(notification);
                }

                if (item.CreatedAt != null)
                {
                    content = "Chuyến hàng mới mã " + item.Code + " đang được vận chuyển đến trạm!";
                    title = EnumConstants.NotificationType.Pending.ToString();
                    var notification = new StationNotificationResponse
                    {
                        StationId = item.StationId,
                        Title = title,
                        Content = content,
                        UpdatedAt = item.CreatedAt ??
                                    item.UpdatedAt ?? item.ReceivedDate ?? item.ExpectedReceiveDate ?? DateTime.Now,
                        Code = item.Code ?? "EMPTY",
                        NotificationType = EnumConstants.NotificationType.Transfer
                    };
                    response.Add(notification);
                }
            }

            result.Payload = response;
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get notifications success";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get notifications error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }

        return result;
    }
}