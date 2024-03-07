using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.CustomServices;

public class CollectedHubService : ICollectedHubService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CollectedHubService(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public Task<OperationResult<IEnumerable<CollectedHubResponse>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<CollectedHub, bool>>? filter = null, string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<CollectedHubResponse>>();
        try
        {
            var collectedHubs = _unitOfWork.CollectedHubRepository.FilterAll(isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
            if(!collectedHubs.Any())
            {
                result.Message = "CollectedHub not found";
                result.StatusCode = StatusCode.NotFound;
                return Task.FromResult(result);
            }
            result.Payload = _mapper.Map<IEnumerable<CollectedHubResponse>>(collectedHubs);
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get CollectedHubs error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }
        return Task.FromResult(result);
    }

    public async Task<OperationResult<CollectedHubResponse>> GetById(Guid objectId)
    {
        var result = new OperationResult<CollectedHubResponse>();
        try
        {
            var collectedHub =  await _unitOfWork.CollectedHubRepository.GetByIdAsync(objectId);
            if(collectedHub == null)
            {
                result.Message = "CollectedHub not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            result.Payload = _mapper.Map<CollectedHubResponse>(collectedHub);
            result.StatusCode = StatusCode.Ok;
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get CollectedHub by Id Error" + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }

        return result;
    }

    public Task<OperationResult<CollectedHubResponse>> GetFilterByExpression(Expression<Func<CollectedHub, bool>> filter, string[]? includeProperties = null)
    {
        var result = new OperationResult<CollectedHubResponse>();
        try
        {
            var collectedHub = _unitOfWork.CollectedHubRepository.FilterByExpression(filter, null);
            if(!collectedHub.Any())
            {
                result.Message = "CollectedHub not found";
                result.StatusCode = StatusCode.NotFound;
                return Task.FromResult(result);
            }
            result.Payload = _mapper.Map<CollectedHubResponse>(collectedHub);
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get CollectedHub by filter success";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Get CollectedHub by filter error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }
        return Task.FromResult(result);
    }

    public async Task<OperationResult<CollectedHubResponse>> Create(CollectedHubRequestCreate objectRequestCreate)
    {
        var result = new OperationResult<CollectedHubResponse>();
        try
        {
            var collectedHub = _mapper.Map<CollectedHub>(objectRequestCreate);
            var collectedHubCreated = await _unitOfWork.CollectedHubRepository.AddAsync(collectedHub);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count == 0)
            {
                result.Message = "Create CollectedHub failed";
                result.StatusCode = StatusCode.BadRequest;
                return result;
            }
            var collectedHubResponse = _mapper.Map<CollectedHubResponse>(collectedHubCreated);
            result.Payload = collectedHubResponse;
            result.StatusCode = StatusCode.Created;
            result.Message = "Create CollectedHub success";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Create CollectedHub error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }
        return result;
    }

    public async Task<OperationResult<bool>> Delete(Guid id)
    {
        var result = new OperationResult<bool>();
        try
        {
            var collectedHub = await _unitOfWork.CollectedHubRepository.GetByIdAsync(id);
            if(collectedHub == null)
            {
                result.Message = "CollectedHub not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            _unitOfWork.CollectedHubRepository.SoftRemove(collectedHub);
            var count = _unitOfWork.SaveChangesAsync().Result;
            if(count > 0)
            {
                result.Payload = true;
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.Message = "Delete CollectedHub failed";
                result.StatusCode = StatusCode.BadRequest;
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Delete CollectedHub error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            throw;
        }
        return result;
    }

    public async Task<OperationResult<CollectedHubResponse>> Update(Guid id, CollectedHubRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<CollectedHubResponse>();
        try
        {
            var collectedHub = await _unitOfWork.CollectedHubRepository.GetByIdAsync(id);
            if(collectedHub == null)
            {
                result.Message = "CollectedHub not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            _mapper.Map(objectRequestUpdate, collectedHub);
            _unitOfWork.CollectedHubRepository.Update(collectedHub);
            var count = await _unitOfWork.SaveChangesAsync();
            if(count > 0)
            {
                result.Payload = _mapper.Map<CollectedHubResponse>(collectedHub);
                var collectedHubResponse = _mapper.Map<CollectedHubResponse>(collectedHub);
                result.Payload = collectedHubResponse;
                result.StatusCode = StatusCode.Ok;
                result.Message = "Update CollectedHub success";
                result.IsError = false;
            }
            else
            {
                result.Message = "Update CollectedHub failed";
                result.StatusCode = StatusCode.BadRequest;
                result.IsError = true;
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError("Update CollectedHub error " + ex.Message);
            result.StatusCode = StatusCode.ServerError;
            result.IsError = true;
            throw;
        }

        return result;
    }
}