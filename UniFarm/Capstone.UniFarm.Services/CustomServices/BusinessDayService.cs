using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Capstone.UniFarm.Domain.Enum;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class BusinessDayService : IBusinessDayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusinessDayService> _logger;
        private readonly IMapper _mapper;

        public BusinessDayService(IUnitOfWork unitOfWork, ILogger<BusinessDayService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> CreateBusinessDay(BusinessDayRequest businessDayRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                DateTime endOfRegister = businessDayRequest.OpenDay
                    .Date.AddDays(-1)
                    .AddHours(23).AddMinutes(59);
                DateTime regiterDay = businessDayRequest.OpenDay
                    .AddDays(-3).Date;

                var businessDay = new BusinessDay
                {
                    Name = businessDayRequest.Name,
                    RegiterDay = regiterDay,
                    EndOfRegister = endOfRegister,
                    OpenDay = businessDayRequest.OpenDay.Date,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };

                int checkResult;
                bool isUnique = await _unitOfWork.BusinessDayRepository.IsUniqueOpenDay(businessDay.OpenDay);
                if (!isUnique)
                {
                    _unitOfWork.BusinessDayRepository.Update(businessDay);
                    checkResult = _unitOfWork.Save();
                    result.AddResponseStatusCode(StatusCode.Created, "Add BusinessDay Success!", true);
                    return result;
                }

                await _unitOfWork.BusinessDayRepository.AddAsync(businessDay);
                checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.Created, "Add BusinessDay Success!", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add BusinessDay Failed!");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CreateBusinessDay Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteBusinessDay(Guid businessDayId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var businessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(businessDayId);
                if (businessDay != null && businessDay.Status == "Active")
                {
                    var menus = await _unitOfWork.MenuRepository.GetAllMenuInCurrentBusinessDay(businessDayId);
                    if (menus.Any())
                    {
                        result.AddError(StatusCode.BadRequest,
                            "Cannot delete BusinessDay because it has associated menus. Please delete the menus first.");
                        return result;
                    }

                    businessDay.Status = "Inactive";
                    _unitOfWork.BusinessDayRepository.Update(businessDay);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok,
                            $"Delete BusinessDay have Id: {businessDayId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete BusinessDay Failed!");
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound,
                        $"Can't find or You Can not Delete this BusinessDay have Id: {businessDayId}. Delete Faild!.", false);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeleteBusinessDay Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> StopSellingDay(Guid businessDayId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var businessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(businessDayId);
                if (businessDay != null)
                {
                    if (businessDay.Status == "Active")
                    {
                        businessDay.Status = "StopSellingDay";
                        businessDay.StopSellingDay = DateTime.UtcNow.AddHours(7);
                        _unitOfWork.BusinessDayRepository.Update(businessDay);
                        var checkResult = _unitOfWork.Save();
                        if (checkResult > 0)
                        {
                            var listMenuContainProductItemsSelling = await _unitOfWork.MenuRepository.GetAllMenuInCurrentBusinessDay(businessDayId);
                            if(listMenuContainProductItemsSelling != null)
                            {
                                foreach (var menu in listMenuContainProductItemsSelling)
                                {
                                    menu.Status = "Preparing";
                                    menu.BusinessDayId = null;
                                    _unitOfWork.MenuRepository.Update(menu);
                                    var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuId(menu.Id);
                                    if (productItemsInMenu != null)
                                    {
                                        foreach (var productItemInMenu in productItemsInMenu)
                                        {
                                            var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemInMenu.ProductItemId);
                                            if(productItem != null && productItem.Status == "Selling")
                                            {
                                                productItem.Status = "Registered";
                                                _unitOfWork.ProductItemRepository.Update(productItem);
                                            }
                                        }
                                    }
                                    _unitOfWork.Save();
                                }
                            }
                            result.AddResponseStatusCode(StatusCode.Ok,
                                $"Stop Selling BusinessDay have Id: {businessDayId} Success.", true);
                        }
                        else
                        {
                            result.AddError(StatusCode.BadRequest, "Stop Selling BusinessDay Failed!");
                        }
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest,
                            "Stop Selling in BusinessDay Only Update Status Active!");
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound,
                        $"Can't find BusinessDay have Id: {businessDayId}. Stop Selling BusinessDay Faild!.", false);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in StopSellingDay Service Method");
                throw;
            }
        }

        public async Task<OperationResult<BusinessDayResponse>> FarmHubGetBusinessDayById(Guid farmHubAccountId,
            Guid businessDayId)
        {
            var result = new OperationResult<BusinessDayResponse>();
            try
            {
                var accountRoleInfor =
                    await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
                var businessDay =
                    await _unitOfWork.BusinessDayRepository.FarmHubGetBusinessDayByIdAsync(
                        (Guid)accountRoleInfor.FarmHubId, businessDayId);
                if (businessDay == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                    return result;
                }
                else if (businessDay.Status != "Inactive")
                {
                    var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get BusinessDay by Id: {businessDayId} Success!",
                        businessDayResponse);
                    return result;
                }

                result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error occurred in FarmHubGetBusinessDayById Service Method for category ID: {businessDayId}");
                throw;
            }
        }

        public async Task<OperationResult<List<BusinessDayResponse>>> GetAllBusinessDays()
        {
            var result = new OperationResult<List<BusinessDayResponse>>();
            try
            {
                var listBusinessDays = await _unitOfWork.BusinessDayRepository.GetAllBusinessDay();
                var activeBusinessDays = listBusinessDays.Where(c => c.Status != "Inactive").ToList();
                var businessDayResponses = _mapper.Map<List<BusinessDayResponse>>(activeBusinessDays);

                if (businessDayResponses == null || !businessDayResponses.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List BusinessDays is Empty!", businessDayResponses);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, "Get List BusinessDays Done!", businessDayResponses);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllBusinessDays Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<BusinessDayContainBatchStatistics>>>
            GetAllBusinessDaysContainBatchQuantity(Guid collectedHubId)
        {
            var result = new OperationResult<List<BusinessDayContainBatchStatistics>>();
            try
            {
                var listBusinessDays = await _unitOfWork.BusinessDayRepository
                    .GetAllWithoutPaging(false, "CreatedAt", x => x.Status != EnumConstants.ActiveInactiveEnum.INACTIVE)
                    .ToListAsync();
                var businessDayResponses = new List<BusinessDayContainBatchStatistics>();
                foreach (var businessDay in listBusinessDays)
                {
                    var batch = await _unitOfWork.BatchesRepository
                        .FilterByExpression(x => x.BusinessDayId == businessDay.Id && x.CollectedId == collectedHubId)
                        .ToListAsync();
                    var businessDayResponse = new BusinessDayContainBatchStatistics()
                    {
                        Id = businessDay.Id,
                        Name = businessDay.Name,
                        RegiterDay = businessDay.RegiterDay,
                        EndOfRegister = businessDay.EndOfRegister,
                        OpenDay = businessDay.OpenDay,
                        StopSellingDay = businessDay.StopSellingDay,
                        EndOfDay = businessDay.EndOfDay,
                        CreatedAt = businessDay.CreatedAt,
                        UpdatedAt = businessDay.UpdatedAt,
                        Status = businessDay.Status,
                        NumOfBatchesPending =
                            batch.Count(x => x.Status == EnumConstants.BatchStatus.Pending.ToString()),
                        NumOfBatchesProcessed =
                            batch.Count(x => x.Status == EnumConstants.BatchStatus.Processed.ToString()),
                        NumOfBatchesReceived =
                            batch.Count(x => x.Status == EnumConstants.BatchStatus.Received.ToString()),
                        NumOfBatchesNotReceived =
                            batch.Count(x => x.Status == EnumConstants.BatchStatus.NotReceived.ToString())
                    };
                    businessDayResponses.Add(businessDayResponse);
                }

                if (businessDayResponses == null || !businessDayResponses.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List BusinessDays is Empty!", businessDayResponses);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, "Get List BusinessDays Done!", businessDayResponses);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllBusinessDaysContainBatchQuantity Service Method");
                throw;
            }
        }

        public async Task<OperationResult<BusinessDayResponse>> GetBusinessDayById(Guid businessDayId)
        {
            var result = new OperationResult<BusinessDayResponse>();
            try
            {
                var businessDay = await _unitOfWork.BusinessDayRepository.GetBusinessDayByIdAsync(businessDayId);
                if (businessDay == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                    return result;
                }
                else if (businessDay.Status != "Inactive")
                {
                    var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get BusinessDay by Id: {businessDayId} Success!",
                        businessDayResponse);
                    return result;
                }

                result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error occurred in GetBusinessDayById Service Method for category ID: {businessDayId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateBusinessDay(Guid businessDayId,
            BusinessDayRequestUpdate businessDayRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var businessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(businessDayId);
                if (businessDay != null)
                {
                    businessDay.Name = businessDayRequestUpdate.Name;
                    businessDay.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    _unitOfWork.BusinessDayRepository.Update(businessDay);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok,
                            $"Update BusinessDay have Id: {businessDayId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update BusinessDay Failed!");
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound,
                        $"Can't find BusinessDay have Id: {businessDayId}. Update Faild!.", false);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UpdateBusinessDay Service Method");
                throw;
            }
        }

        public async Task UpdateEndOfDayForAllBusinessDays()
        {
            var result = new OperationResult<bool>();
            try
            {

                var businessDays = await _unitOfWork.BusinessDayRepository
                    .GetAllBusinessDayNotEndOfDayYet(ed => ed.EndOfDay == null && ed.Status == "StopSellingDay");

                foreach (var existingBusinessDay in businessDays)
                {
                    bool islistOrdersCompleted =
                        await _unitOfWork.OrderRepository.AreAllOrdersCompletedForBusinessDay(existingBusinessDay.Id);

                    // Kiểm tra xem tất cả Orders đã hoàn thành chưa
                    if (islistOrdersCompleted)
                    {
                        // Tất cả Orders đã hoàn thành, cập nhật EndOfDay cho BusinessDay
                        existingBusinessDay.EndOfDay = DateTime.UtcNow.AddHours(7);
                        existingBusinessDay.Status = CommonEnumStatus.PaymentConfirm.ToString();
                        _unitOfWork.BusinessDayRepository.Update(existingBusinessDay);
                        var checkResult = _unitOfWork.Save();
                        if (checkResult > 0)
                        {
                            _logger.LogInformation(
                                $"End Of Day: ! {existingBusinessDay.EndOfDay} | Status: {existingBusinessDay.Status}");
                            result.AddResponseStatusCode(StatusCode.Ok, $"Get Dashboard End Of Day!", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdateEndOfDayForAllBusinessDays Service Method!");
                throw;
            }
        }

        public async Task CheckAndStopSellingDayJob()
        {
            var result = new OperationResult<bool>();
            try
            {
                var businessDays = await _unitOfWork.BusinessDayRepository.GetAllActiveBusinessDaysUpToToday();
                foreach (var businessDay in businessDays)
                {
                    if(businessDay != null)
                    {
                        var menus = await _unitOfWork.MenuRepository.GetAllMenuInCurrentBusinessDay(businessDay.Id);
                        if (!menus.Any())
                        {
                            businessDay.Status = "StopSellingDay";
                            businessDay.StopSellingDay = DateTime.UtcNow.AddHours(7);
                            _unitOfWork.BusinessDayRepository.Update(businessDay);
                            var checkResult = _unitOfWork.Save();
                            if (checkResult > 0)
                            {
                                _logger.LogInformation($"Business Day {businessDay.Id} updated to StopSellingDay at {DateTime.UtcNow.AddHours(7)}");
                                result.AddResponseStatusCode(StatusCode.Ok, $"Updated BusinessDay {businessDay.Id} to StopSellingDay", true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CheckAndStopSellingDayJob Service Method");
                throw;
            }
        }



        // Remove all orders status isPaid == false when business day status is stop selling
        public async Task<OperationResult<bool>> RemoveProductItemInCartJob()
        {
            var result = new OperationResult<bool>();
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllWithoutPaging(false, null, x => x.IsPaid == false)
                    .ToListAsync();
                var businessDays = await _unitOfWork.BusinessDayRepository
                    .GetAllWithoutPaging(false, null,
                        x => x.Status == EnumConstants.CommonEnumStatus.StopSellingDay.ToString()).Select(x => x.Id)
                    .ToListAsync();
                foreach (var order in orders)
                {
                    if (businessDays.Contains(order.BusinessDayId ?? Guid.Empty))
                    {
                        await _unitOfWork.OrderRepository.DeleteAsync(order);
                        var checkResult = await _unitOfWork.SaveChangesAsync();
                        if (checkResult <= 0)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogInformation(
                                $"Remove orders {order.Code} in cart in business id {order.BusinessDayId} failure! ");
                            result.AddError(StatusCode.BadRequest, "Remove Product Item In Cart Failed!");
                            return result;
                        }
                    }
                }

                await transaction.CommitAsync();
                result.AddResponseStatusCode(StatusCode.Ok, "Remove Product Item In Cart Success!", true);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error occurred in RemoveProductItemInCartJob Service Method!");
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
            return result;
        }
    }
}