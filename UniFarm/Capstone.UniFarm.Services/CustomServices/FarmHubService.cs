using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class FarmHubService : IFarmHubService
    {
        private readonly UserManager<Account> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FarmHubService> _logger;
        private readonly IMapper _mapper;
        private readonly IWalletService _walletService;
        private readonly ICloudinaryService _cloudinaryService;
        public FarmHubService(
            UserManager<Account> userManager,
            IUnitOfWork unitOfWork,
            ILogger<FarmHubService> logger,
            IMapper mapper,
            IWalletService walletService,
            ICloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _walletService = walletService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<OperationResult<AccountAndFarmHubRequest>> CreateFarmHubShop(AccountAndFarmHubRequest accountAndFarmHubRequest)
        {
            var result = new OperationResult<AccountAndFarmHubRequest>();
            try
            {
                var checkEmail = await _userManager.FindByEmailAsync(accountAndFarmHubRequest.Email);
                if (checkEmail != null)
                {
                    result.AddError(StatusCode.BadRequest, "Email already exists");
                    result.IsError = true;
                    return result;
                }

                var checkPhone =
                    await _unitOfWork.AccountRepository.FindSingleAsync(
                        x => x.Phone == accountAndFarmHubRequest.PhoneNumber);
                if (checkPhone != null)
                {
                    result.AddError(StatusCode.BadRequest, "Phone already exists");
                    result.IsError = true;
                    return result;
                }

                var existingUserName = await _userManager.FindByNameAsync(accountAndFarmHubRequest.UserName);
                if (existingUserName != null)
                {
                    result.AddError(StatusCode.BadRequest, "UserName already exists!");
                    result.IsError = true;
                    return result;
                }

                bool checkFarmHubName = await _unitOfWork.FarmHubRepository.CheckFarmHubNameAsync(accountAndFarmHubRequest.FarmHubName);
                if (checkFarmHubName)
                {
                    result.AddError(StatusCode.BadRequest, "FarmHubName already exists!");
                    result.IsError = true;
                    return result;
                }
                var newAccount = _mapper.Map<Account>(accountAndFarmHubRequest);
                newAccount.CreatedAt = DateTime.UtcNow.AddHours(7);
                newAccount.PasswordHash = _userManager.PasswordHasher.HashPassword(newAccount, accountAndFarmHubRequest.Password);
                newAccount.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;
                newAccount.RoleName = EnumConstants.RoleEnumString.FARMHUB;

                var imageFarmhub = _cloudinaryService.UploadImageAsync(accountAndFarmHubRequest.FarmHubImage);
                var farmHub = _mapper.Map<FarmHub>(accountAndFarmHubRequest);

                bool checkFarmHubCode;
                do
                {
                    farmHub.Code = "FARM" + await CodeGenerator.GenerateCode(4);
                    checkFarmHubCode = await _unitOfWork.FarmHubRepository.CheckFarmHubCodeAsync(farmHub.Code);
                } while (checkFarmHubCode);

                farmHub.Image = await imageFarmhub;
                farmHub.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;
                farmHub.CreatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.FarmHubRepository.AddAsync(farmHub);

                var response = await _userManager.CreateAsync(newAccount);
                if (response.Succeeded)
                {
                    var wallet = new WalletRequest
                    {
                        AccountId = newAccount.Id,
                    };

                    var walletResult = await _walletService.Create(wallet);
                    if (walletResult.IsError)
                    {
                        result.AddError(StatusCode.BadRequest,
                            "Create wallet error " + walletResult.Errors.FirstOrDefault()?.Message);
                        result.IsError = true;
                        return result;
                    }

                    var accountRole = new AccountRole
                    {
                        Id = Guid.NewGuid(),
                        AccountId = newAccount.Id,
                        Account = newAccount,
                        FarmHubId = farmHub.Id,
                        Status = EnumConstants.ActiveInactiveEnum.ACTIVE
                    };
                    newAccount.AccountRoles = accountRole;
   
                    await _unitOfWork.AccountRoleRepository.AddAsync(accountRole);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Account and Setup FarmHub Shop Success!", accountAndFarmHubRequest);
                    }
                    await _userManager.AddToRoleAsync(newAccount, EnumConstants.RoleEnumString.FARMHUB);
                    result.Payload = accountAndFarmHubRequest;
                    result.IsError = false;
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Account and Setup FarmHub Shop Failed!");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CreateFarmHubShop Service Method");
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
                    existingFarmHub.Status = "Inactive";
                    _unitOfWork.FarmHubRepository.Update(existingFarmHub);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete FarmHub have Id: {farmhubId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete FarmHub Failed!");
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find FarmHub have Id: {farmhubId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in DeleteFarmHub service method for FarmHub ID: {farmhubId}");
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

        public async Task<OperationResult<FarmHubResponse>> GetFarmHubInforByFarmHubAccountId(Guid farmHubAccountId)
        {
            var result = new OperationResult<FarmHubResponse>();
            var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
            if (accountRoleInfor != null && accountRoleInfor.FarmHubId != null)
            {
                var farmHubId = accountRoleInfor.FarmHubId;
                var farmhub = await _unitOfWork.FarmHubRepository.GetByIdAsync((Guid)farmHubId);
                if (farmhub == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found FarmHub with Id: {(Guid)farmHubId}");
                }
                var farmhubResponse = _mapper.Map<FarmHubResponse>(farmhub);
                result.AddResponseStatusCode(StatusCode.Ok, $"Get FarmHub by Id: {(Guid)farmHubId} Success!", farmhubResponse);
            }
            else
            {
                result.AddError(StatusCode.NotFound, $"Please Create Before Get FarmHub Profile!");
            }
            return result;
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
                    if (farmHubRequestUpdate.Status != null && (farmHubRequestUpdate.Status == "Active" || farmHubRequestUpdate.Status == "Inactive"))
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdateFarmHub service method for FarmHub ID: {farmhubId}");
                throw;
            }
        }
    }
}
