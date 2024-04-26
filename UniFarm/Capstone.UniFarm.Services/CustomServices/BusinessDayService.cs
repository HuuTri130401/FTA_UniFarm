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
                if (businessDay != null)
                {
                    businessDay.Status = "Inactive";
                    _unitOfWork.BusinessDayRepository.Update(businessDay);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete BusinessDay have Id: {businessDayId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete BusinessDay Failed!"); ;
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find BusinessDay have Id: {businessDayId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
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
                    if(businessDay.Status == "Active")
                    {
                        businessDay.Status = "StopSellingDay";
                        businessDay.StopSellingDay = DateTime.UtcNow.AddHours(7);
                        _unitOfWork.BusinessDayRepository.Update(businessDay);
                        var checkResult = _unitOfWork.Save();
                        if (checkResult > 0)
                        {
                            result.AddResponseStatusCode(StatusCode.Ok, $"Stop Selling BusinessDay have Id: {businessDayId} Success.", true);
                        }
                        else
                        {
                            result.AddError(StatusCode.BadRequest, "Stop Selling BusinessDay Failed!"); 
                        }
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Stop Selling in BusinessDay Only Update Status Active!");
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find BusinessDay have Id: {businessDayId}. Stop Selling BusinessDay Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<BusinessDayResponse>> FarmHubGetBusinessDayById(Guid farmHubAccountId, Guid businessDayId)
        {
            var result = new OperationResult<BusinessDayResponse>();
            try
            {
                var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
                var businessDay = await _unitOfWork.BusinessDayRepository.FarmHubGetBusinessDayByIdAsync((Guid)accountRoleInfor.FarmHubId, businessDayId);
                if (businessDay == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                    return result;
                }
                else if (businessDay.Status != "Inactive")
                {
                    var businessDayResponse = _mapper.Map<BusinessDayResponse>(businessDay);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get BusinessDay by Id: {businessDayId} Success!", businessDayResponse);
                    return result;
                }
                result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in FarmHubGetBusinessDayById Service Method for category ID: {businessDayId}");
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
                result.AddResponseStatusCode(StatusCode.Ok, "List BusinessDays is Empty!", businessDayResponses);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllBusinessDay Service Method");
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
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get BusinessDay by Id: {businessDayId} Success!", businessDayResponse);
                    return result;
                }
                result.AddError(StatusCode.NotFound, $"Can't found BusinessDay with Id: {businessDayId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetBusinessDayById Service Method for category ID: {businessDayId}");
                throw;
            }
        }

        public Task<OperationResult<bool>> UpdateBusinessDay(Guid businessDayId, BusinessDayRequestUpdate businessDayRequestUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateEndOfDayForAllBusinessDays()
        {
            var result = new OperationResult<bool>();
            try
            {

                var businessDays = await _unitOfWork.BusinessDayRepository.GetAllBusinessDayNotEndOfDayYet(ed => ed.EndOfDay == null);

                foreach (var existingBusinessDay in businessDays)
                {
                    bool islistOrdersCompleted = await _unitOfWork.OrderRepository.AreAllOrdersCompletedForBusinessDay(existingBusinessDay.Id);

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
                            _logger.LogInformation($"End Of Day: ! {existingBusinessDay.EndOfDay} | Status: {existingBusinessDay.Status}");
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
    }
}
