using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.CustomServices;

public class AccountRoleService : IAccountRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public AccountRoleService(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<OperationResult<IEnumerable<AccountRole>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<AccountRole, bool>>? filter = null, string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AccountRole>>();
        try
        {
            var accountRoles = _unitOfWork.AccountRoleRepository.FilterAll(isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
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

    public async Task<OperationResult<AccountRole>> GetById(Guid objectId)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var accountRole = await _unitOfWork.AccountRoleRepository.GetByIdAsync(objectId);
            result.Payload = _mapper.Map<AccountRole>(accountRole);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }
        return result;
    }
    
    public async Task<OperationResult<AccountRole>> Create(AccountRoleRequest objectRequestCreate)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var accountRole = _mapper.Map<AccountRole>(objectRequestCreate);
            var createdAccountRole = await _unitOfWork.AccountRoleRepository.AddAsync(accountRole);
            if(createdAccountRole == null)
            {
                result.AddError(StatusCode.BadRequest,"AccountRole not created");
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }

            await _unitOfWork.SaveChangesAsync();
            result.Payload = _mapper.Map<AccountRole>(createdAccountRole);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
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
                result.AddError(StatusCode.NotFound,"AccountRole not found");
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            objectAccountRole.Status = "Inactive";
            result.Payload = true;
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }
        return result;
    }
    
    public async Task<OperationResult<AccountRole>> Update(Guid objectId, AccountRoleRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var accountRole = _mapper.Map<AccountRole>(objectRequestUpdate);
            _unitOfWork.AccountRoleRepository.Update(accountRole);
            await _unitOfWork.SaveChangesAsync();
            result.Payload = _mapper.Map<AccountRole>(accountRole);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }
        return result;
    }

    public async Task<OperationResult<AccountRole>> CreateModel(AccountRole entity)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var createdAccountRole = _unitOfWork.AccountRoleRepository.AddAsync(entity);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.AddError(StatusCode.BadRequest, "AccountRole not created");
                result.IsError = true;
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }
            result.Payload = entity;
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }
        return result;
    }

    public Task<OperationResult<AccountRole>> GetAccountRoleByExpression(Expression<Func<AccountRole, bool>> predicate, string[]? includeProperties = null)
    {
        var result = new OperationResult<AccountRole>();
        try
        {
            var accountRole = _unitOfWork.AccountRoleRepository.FilterByExpression(
                predicate, includeProperties);
            result.Payload = _mapper.Map<AccountRole>(accountRole);
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