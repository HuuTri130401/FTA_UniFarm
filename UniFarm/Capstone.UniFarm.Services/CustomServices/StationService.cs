using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.CustomServices;

public class StationService : IStationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StationService(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public Task<OperationResult<IEnumerable<StationResponse>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Station, bool>>? filter = null, string[]? includeProperties = null,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<StationResponse>>();
        try
        {
            var stations = _unitOfWork.StationRepository.FilterAll(isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
            if(!stations.Any())
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
            var station =  await _unitOfWork.StationRepository.GetByIdAsync(objectId);
            if(station == null)
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

    public Task<OperationResult<StationResponse>> GetFilterByExpression(Expression<Func<Station, bool>> filter, string[]? includeProperties = null)
    {
        var result = new OperationResult<StationResponse>();
        try
        {
            var station = _unitOfWork.StationRepository.FilterByExpression(filter, null);
            if(station == null)
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
            stationResponse.Area = _mapper.Map<AreaResponse>(stationCreated.Area);
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
            if(station == null)
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            _unitOfWork.StationRepository.SoftRemove(station);
            var count = _unitOfWork.SaveChangesAsync().Result;
            if(count > 0)
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

    public async Task<OperationResult<StationResponse>> Update(Guid id, StationRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<StationResponse>();
        try
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(id);
            if(station == null)
            {
                result.Message = "station not found";
                result.StatusCode = StatusCode.NotFound;
                return result;
            }
            _mapper.Map(objectRequestUpdate, station);
            station.Area = await _unitOfWork.AreaRepository.GetByIdAsync(objectRequestUpdate.AreaId);
            _unitOfWork.StationRepository.Update(station);
            var count = await _unitOfWork.SaveChangesAsync();
            if(count > 0)
            {
                result.Payload = _mapper.Map<StationResponse>(station);
                
                station.Area = await _unitOfWork.AreaRepository.GetByIdAsync(objectRequestUpdate.AreaId);
                var stationResponse = _mapper.Map<StationResponse>(station);
                stationResponse.Area = _mapper.Map<AreaResponse>(station.Area);
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
}