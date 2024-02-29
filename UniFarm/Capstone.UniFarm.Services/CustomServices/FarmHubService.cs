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

namespace Capstone.UniFarm.Services.CustomServices
{
    public class FarmHubService : IFarmHubService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FarmHubService> _logger;
        private readonly IMapper _mapper;

        public FarmHubService(IUnitOfWork unitOfWork, ILogger<FarmHubService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> CreateFarmHub(FarmHubRequest farmHubRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var farmHub = _mapper.Map<FarmHub>(farmHubRequest);
                farmHub.Status = "Active";
                farmHub.CreatedAt = DateTime.Now;
                await _unitOfWork.FarmHubRepository.AddAsync(farmHub);
                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.Created, "Add FarmHub Success!", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add FarmHub Failed!"); ;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteFarmHub(Guid farmhubId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingFarmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(farmhubId);
                if (existingFarmHub != null)
                {
                    existingFarmHub.Status = "InActive";
                    _unitOfWork.FarmHubRepository.Update(existingFarmHub);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete FarmHub have Id: {farmhubId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete FarmHub Failed!"); ;
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find FarmHub have Id: {farmhubId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<FarmHubResponse>>> GetAllFarmHubs()
        {
            var result = new OperationResult<List<FarmHubResponse>>();
            try
            {
                var listFarmHubs = await _unitOfWork.FarmHubRepository.GetAllAsync();
                var listFarmHubsResponse = _mapper.Map<List<FarmHubResponse>>(listFarmHubs);

                if (listFarmHubsResponse == null || !listFarmHubsResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List FarmHub is Empty!", listFarmHubsResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List FarmHubs Done.", listFarmHubsResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllFarmHubs Service Method");
                throw;
            }
        }

        public async Task<OperationResult<FarmHubResponse>> GetFarmHubById(Guid farmhubId)
        {
            var result = new OperationResult<FarmHubResponse>();
            try
            {
                var farmhub = await _unitOfWork.FarmHubRepository.GetByIdAsync(farmhubId);
                if (farmhub == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found FarmHub with Id: {farmhubId}");
                    return result;
                }
                var farmhubResponse = _mapper.Map<FarmHubResponse>(farmhub);
                result.AddResponseStatusCode(StatusCode.Ok, $"Get FarmHub by Id: {farmhubId} Success!", farmhubResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetFarmHubById service method for FarmHub ID: {farmhubId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateFarmHub(Guid farmhubId, FarmHubRequestUpdate farmHubRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingFarmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(farmhubId);
                if (existingFarmHub != null)
                {
                    bool isAnyFieldUpdated = false;
                    if (farmHubRequestUpdate.Name != null)
                    {
                        existingFarmHub.Name = farmHubRequestUpdate.Name;
                        isAnyFieldUpdated = true;
                    }
                    if (farmHubRequestUpdate.Code != null)
                    {
                        existingFarmHub.Code = farmHubRequestUpdate.Code;
                        isAnyFieldUpdated = true;
                    }
                    if (farmHubRequestUpdate.Description != null)
                    {
                        existingFarmHub.Description = farmHubRequestUpdate.Description;
                        isAnyFieldUpdated = true;
                    }
                    if (farmHubRequestUpdate.Image != null)
                    {
                        existingFarmHub.Image = farmHubRequestUpdate.Image;
                        isAnyFieldUpdated = true;
                    }
                    if (farmHubRequestUpdate.Address != null)
                    {
                        existingFarmHub.Address = farmHubRequestUpdate.Address;
                        isAnyFieldUpdated = true;
                    }
                    if (farmHubRequestUpdate.Status != null && (farmHubRequestUpdate.Status == "Active" || farmHubRequestUpdate.Status == "InActive"))
                    {
                        existingFarmHub.Status = farmHubRequestUpdate.Status;
                        isAnyFieldUpdated = true;
                    }

                    if (isAnyFieldUpdated)
                    {
                        existingFarmHub.UpdatedAt = DateTime.Now;
                    }

                    _unitOfWork.FarmHubRepository.Update(existingFarmHub);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update FarmHub have Id: {farmhubId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update FarmHub Failed!"); ;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
