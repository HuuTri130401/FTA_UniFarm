using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class AccountRoleService : IAccountRoleService
{
    private readonly UserManager<Account> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountRoleService(
        UserManager<Account> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<OperationResult<IEnumerable<AccountRole>>> GetAll(bool? isAscending, string? orderBy = null,
        Expression<Func<AccountRole, bool>>? filter = null, string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountRole>>();
        try
        {
            var accountRoles = _unitOfWork.AccountRoleRepository.FilterAll(isAscending, orderBy, filter,
                includeProperties, pageIndex, pageSize);
            result.Payload = _mapper.Map<IEnumerable<AccountRole>>(accountRoles);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return Task.FromResult(result);
    }

    public async Task<OperationResult<AccountRole>> GetByAccountId(Guid objectId)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var accountRole = await _unitOfWork.AccountRoleRepository.FilterByExpression(x => x.AccountId == objectId).FirstOrDefaultAsync();
            if(accountRole == null)
            {
                result.AddError(StatusCode.NotFound, "AccountRole not found");
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            result.Payload = accountRole;
            result.StatusCode = StatusCode.Ok;
            result.Message = "AccountRole find by accountId successfully";
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.Message = "AccountRole not found";
            result.IsError = true;
            throw;
        }

        return result;
    }



    public async Task<OperationResult<AccountRole>> Create(AccountRoleRequest objectRequestCreate)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var userRole = _userManager
                .GetRolesAsync(await _userManager.FindByIdAsync(objectRequestCreate.AccountId.ToString())).Result
                .FirstOrDefault();
            if (userRole == EnumConstants.RoleEnumString.CUSTOMER)
            {
                result.AddError(StatusCode.BadRequest, "Customer can not be added to Account Role");
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            var accountRoleExist = _unitOfWork.AccountRoleRepository.FilterByExpression(x =>
                x.AccountId == objectRequestCreate.AccountId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE);
            var accountRole = _mapper.Map<AccountRole>(objectRequestCreate);

            if (userRole == EnumConstants.RoleEnumString.FARMHUB)
            {
                if (objectRequestCreate.FarmHubId == null)
                {
                    result.AddError(StatusCode.BadRequest, "FarmHubId is required for this farm hub role");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                accountRoleExist = accountRoleExist.Where(
                    x => x.AccountId == objectRequestCreate.AccountId
                         && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
                         && x.FarmHubId != null);
                if (accountRoleExist.Any())
                {
                    result.AddError(StatusCode.BadRequest, "This account already assign to a farm hub");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }
                else
                {
                    var farmHub = await _unitOfWork.FarmHubRepository.FilterByExpression(x =>
                            x.Id == objectRequestCreate.FarmHubId &&
                            x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                        .FirstOrDefaultAsync();
                    if (farmHub == null)
                    {
                        result.AddError(StatusCode.BadRequest, "FarmHub not found or inactive");
                        result.IsError = true;
                        result.StatusCode = StatusCode.BadRequest;
                        return result;
                    }

                    objectRequestCreate.StationId = null;
                    objectRequestCreate.CollectedHubId = null;
                }
            }
            else if (userRole == EnumConstants.RoleEnumString.STATIONSTAFF)
            {
                if (objectRequestCreate.StationId == null)
                {
                    result.AddError(StatusCode.BadRequest, "StationId is required for this station staff role");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                accountRoleExist = accountRoleExist.Where(
                    x => x.AccountId == objectRequestCreate.AccountId
                         && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
                         && x.StationId != null);
                if (accountRoleExist.Any())
                {
                    result.AddError(StatusCode.BadRequest, "This account already assign to a station staff");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                var station = await _unitOfWork.StationRepository.FilterByExpression(x =>
                        x.Id == objectRequestCreate.StationId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                    .FirstOrDefaultAsync();
                if (station == null)
                {
                    result.AddError(StatusCode.BadRequest, "Station not found or inactive");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                objectRequestCreate.FarmHubId = null;
                objectRequestCreate.CollectedHubId = null;
            }
            else if (userRole == EnumConstants.RoleEnumString.COLLECTEDSTAFF)
            {
                if (objectRequestCreate.CollectedHubId == null)
                {
                    result.AddError(StatusCode.BadRequest,
                        "CollectedStaffId is required for this collected staff role");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                accountRoleExist = accountRoleExist.Where(
                    x => x.AccountId == objectRequestCreate.AccountId
                         && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
                         && x.CollectedHubId != null);
                if (accountRoleExist.Any())
                {
                    result.AddError(StatusCode.BadRequest, "This account already assign to a collected staff");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                var collectedHub = await _unitOfWork.CollectedHubRepository.FilterByExpression(x =>
                        x.Id == objectRequestCreate.CollectedHubId &&
                        x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                    .FirstOrDefaultAsync();
                if (collectedHub == null)
                {
                    result.AddError(StatusCode.BadRequest, "CollectedHub not found or inactive");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                objectRequestCreate.FarmHubId = null;
                objectRequestCreate.StationId = null;
            }

            accountRole.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;
            var createdAccountRole = await _unitOfWork.AccountRoleRepository.AddAsync(accountRole);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.AddError(StatusCode.BadRequest, "AccountRole not created");
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            result.Payload = createdAccountRole;
            result.Message = "AccountRole created successfully";
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<AccountRole>> Update(Guid accountId, AccountRoleRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var userRole = _userManager
                .GetRolesAsync(await _userManager.FindByIdAsync(objectRequestUpdate.AccountId.ToString())).Result
                .FirstOrDefault();
            if (userRole == EnumConstants.RoleEnumString.CUSTOMER)
            {
                result.AddError(StatusCode.BadRequest, "Customer can not be added to Account Role");
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }
            
            var accountRoleExist = _unitOfWork.AccountRoleRepository.FilterByExpression(x =>
                x.AccountId == objectRequestUpdate.AccountId);
            
            var accountRole = _mapper.Map<AccountRole>(objectRequestUpdate);

            accountRole.Id = accountRoleExist.FirstOrDefault()!.Id;
            if (userRole == EnumConstants.RoleEnumString.FARMHUB)
            {
                if (objectRequestUpdate.FarmHubId == null)
                {
                    result.AddError(StatusCode.BadRequest, "FarmHubId is required for this farm hub role");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                accountRoleExist = accountRoleExist.Where(
                    x => x.AccountId == objectRequestUpdate.AccountId
                         && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE
                         && x.FarmHubId != null);
                if (accountRoleExist.Any())
                {
                    result.AddError(StatusCode.BadRequest, "This account already assign to a farm hub");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }
                else
                {
                    var farmHub = await _unitOfWork.FarmHubRepository.FilterByExpression(x =>
                            x.Id == objectRequestUpdate.FarmHubId &&
                            x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                        .FirstOrDefaultAsync();
                    if (farmHub == null)
                    {
                        result.AddError(StatusCode.BadRequest, "FarmHub not found or inactive");
                        result.IsError = true;
                        result.StatusCode = StatusCode.BadRequest;
                        return result;
                    }

                    accountRole.StationId = null;
                    accountRole.CollectedHubId = null;
                }
            }
            else if (userRole == EnumConstants.RoleEnumString.STATIONSTAFF)
            {
                if (objectRequestUpdate.StationId == null)
                {
                    result.AddError(StatusCode.BadRequest, "StationId is required for this station staff role");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                var station = await _unitOfWork.StationRepository.FilterByExpression(x =>
                        x.Id == objectRequestUpdate.StationId && x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                    .FirstOrDefaultAsync();
                if (station == null)
                {
                    result.AddError(StatusCode.BadRequest, "Station not found or inactive");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                accountRole.FarmHubId = null;
                accountRole.CollectedHubId = null;
            }
            else if (userRole == EnumConstants.RoleEnumString.COLLECTEDSTAFF)
            {
                if (objectRequestUpdate.CollectedHubId == null)
                {
                    result.AddError(StatusCode.BadRequest,
                        "CollectedStaffId is required for this collected staff role");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                var collectedHub = await _unitOfWork.CollectedHubRepository.FilterByExpression(x =>
                        x.Id == objectRequestUpdate.CollectedHubId &&
                        x.Status == EnumConstants.ActiveInactiveEnum.ACTIVE)
                    .FirstOrDefaultAsync();
                if (collectedHub == null)
                {
                    result.AddError(StatusCode.BadRequest, "CollectedHub not found or inactive");
                    result.IsError = true;
                    result.StatusCode = StatusCode.BadRequest;
                    return result;
                }

                accountRole.FarmHubId = null;
                accountRole.StationId = null;
            }

            accountRole.Status = EnumConstants.ActiveInactiveEnum.ACTIVE;

            var updatedAccountRole = await _unitOfWork.AccountRoleRepository.UpdateAsync(accountRole);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.AddError(StatusCode.BadRequest, "AccountRole not updated");
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            result.Payload = updatedAccountRole;
            result.Message = "AccountRole updated successfully";
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            result.Message = "AccountRole not updated";
            throw;
        }

        return result;
    }

    public async Task<OperationResult<bool>> Delete(Guid objectId)
    {
        var result = new OperationResult<bool>();
        try
        {
            var objectAccountRole = await _unitOfWork.AccountRepository.GetByIdAsync(objectId);
            if (objectAccountRole == null)
            {
                result.AddError(StatusCode.NotFound, "AccountRole not found");
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            objectAccountRole.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
            result.Payload = true;
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.Message = "AccountRole not deleted";
            throw;
        }

        return result;
    }

    public async Task<OperationResult<bool>> DeleteByAccountId(Guid accountId)
    {
        var result = new OperationResult<bool>();
        try
        {
            var accountRole = await _unitOfWork.AccountRoleRepository.FilterByExpression(x => x.AccountId == accountId)
                .FirstOrDefaultAsync();
            if(accountRole == null)
            {
                result.AddError(StatusCode.NotFound, "AccountRole not found");
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                result.Payload = false;
                return result;
            }
            accountRole!.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
            await _unitOfWork.SaveChangesAsync();
            result.Payload = true;
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.Message = "AccountRole not deleted";
            result.IsError = true;
            throw;
        }
        return result;
    }

    public Task<OperationResult<AccountRole>> GetAccountRoleByExpression(
        Expression<Func<AccountRole, bool>> predicate,
        string[]? includeProperties = null)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var accountRole = _unitOfWork.AccountRoleRepository.FilterByExpression(
                predicate, includeProperties);
            if (!accountRole.Any())
            {
                result.AddError(StatusCode.NotFound, "AccountRole not found");
                result.IsError = true;
            }
            result.Payload = accountRole.FirstOrDefault();
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return Task.FromResult(result);
    }

    public Task<OperationResult<IEnumerable<AccountRole>>> GetAllWithoutPaging(bool? isAscending,
        string? orderBy = null,
        Expression<Func<AccountRole, bool>>? filter = null, string[]? includeProperties = null)
    {
        var result = new OperationResult<IEnumerable<AccountRole>>();
        try
        {
            var accountRoles = _unitOfWork.AccountRoleRepository.GetAllWithoutPaging(isAscending, orderBy, filter,
                includeProperties);
            result.Payload = accountRoles;
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return Task.FromResult(result);
    }
}