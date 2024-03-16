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
                if (!IsValidBusinessDay(businessDayRequest))
                {
                    result.AddError(StatusCode.BadRequest, "Invalid BusinessDay configuration! " +
                       "\r\nEndOfRegister = RegisterDay + 2 days " +
                       "\r\nOpenDay = EndOfRegister + 1 day!");
                }
                else
                {
                    var businessDay = _mapper.Map<BusinessDay>(businessDayRequest);
                    businessDay.Status = "Active";
                    //businessDay.Status = "Pending";
                    businessDay.CreatedAt = DateTime.Now;
                    await _unitOfWork.BusinessDayRepository.AddAsync(businessDay);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add BusinessDay Success!", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Add BusinessDay Failed!"); ;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //private async void UpdateBusinessDayStatus(object state)
        //{
        //    try
        //    {
        //        var listBusinessDays = _unitOfWork.BusinessDayRepository.GetAllBusinessDay();
        //        var checkResult = _unitOfWork.Save();
        //        if (checkResult > 0)
        //        {
        //            foreach (var businessDay in listBusinessDays)
        //            {
        //                if (businessDay.OpenDay == DateTime.Today)
        //                {
        //                    await _unitOfWork.BusinessDayRepository.UpdateBusinessDayStatus(businessDay.Id, "Active");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public bool IsValidBusinessDay(BusinessDayRequest businessDayRequest)
        {
            if (businessDayRequest.EndOfRegister <= businessDayRequest.RegiterDay.AddDays(2) 
                && businessDayRequest.OpenDay == businessDayRequest.EndOfRegister.AddDays(1))
                return true;
            return false;
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
    }
}
