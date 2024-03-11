using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.CustomServices;

public class AreaService : IAreaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStationService _stationService;
    private readonly IApartmentService _apartmentService;

    public AreaService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStationService stationService,
        IApartmentService apartmentService
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _stationService = stationService;
        _apartmentService = apartmentService;
    }

    public Task<OperationResult<IEnumerable<AreaResponse>>> GetAll(bool? isAscending,
        string? orderBy = null,
        Expression<Func<Area, bool>>? filter = null,
        string[]? includeProperties = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<AreaResponse>>();
        try
        {
            var areas = _unitOfWork.AreaRepository.FilterAll(isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
            result.Payload = _mapper.Map<IEnumerable<AreaResponse>>(areas);
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get all areas successfully";
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return Task.FromResult(result);
    }
    
    public async Task<OperationResult<AreaResponse>> GetById(Guid objectId)
    {
        var result = new OperationResult<AreaResponse>();
        try
        {
            var area = await _unitOfWork.AreaRepository.GetByIdAsync(objectId);
            result.Payload = _mapper.Map<AreaResponse>(area);
            result.StatusCode = StatusCode.Ok;

        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }

    public async Task<OperationResult<AreaResponse>> Create(AreaRequestCreate objectRequestCreate)
    {
        var result = new OperationResult<AreaResponse>();
        try
        {
            var area = _mapper.Map<Area>(objectRequestCreate);
            await _unitOfWork.AreaRepository.AddAsync(area);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count > 0)
            {
                result.Payload = _mapper.Map<AreaResponse>(area);
                result.StatusCode = StatusCode.Created;
            }
            else
            {
                result.AddError(StatusCode.BadRequest, "Create Area Failed");
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }

    public async Task<OperationResult<bool>> Delete(Guid id)
    {
        var result = new OperationResult<bool>();
        try
        {
            var area = await _unitOfWork.AreaRepository.GetByIdAsync(id);
            if (area == null)
            {
                result.AddError(StatusCode.NotFound, "Area not found");
                return result;
            }

            _unitOfWork.AreaRepository.SoftRemove(area);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count > 0)
            {
                result.Payload = true;
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.AddError(StatusCode.BadRequest, "Delete Area Failed");
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }

    public async Task<OperationResult<AreaResponse>> Update(Guid id, AreaRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<AreaResponse>();
        try
        {
            var area = await _unitOfWork.AreaRepository.GetByIdAsync(id);
            if (area == null)
            {
                result.AddError(StatusCode.NotFound, "Area not found");
                return result;
            }

            _mapper.Map(objectRequestUpdate, area);
            _unitOfWork.AreaRepository.Update(area);
            var count = _unitOfWork.SaveChangesAsync();
            if (count.Result > 0)
            {
                result.Payload = _mapper.Map<AreaResponse>(area);
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.AddError(StatusCode.BadRequest, "Update Area Failed");
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }
    
    public async Task<OperationResult<IEnumerable<StationResponse>>> GetStationsOfArea(
        bool? isAscending,
        string? orderBy = null,
        Expression<Func<Area, bool>>? filterArea = null,
        Expression<Func<Station, bool>>? filterStation = null,
        string[]? includeProperties = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<StationResponse>>();
        try
        {
            var areas = _unitOfWork.AreaRepository.FilterByExpression(filterArea ?? (x => true));
            if (!areas.Any())
            {
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Area not found";
                return result;
            }
            var stations = await _stationService.GetAll(isAscending, orderBy, filterStation, includeProperties, pageIndex, pageSize);
            if(stations.Payload == null)
            {
                result.StatusCode = StatusCode.Ok;
                result.Message = "No stations found";
                return result;
            }
            result.StatusCode = StatusCode.Ok;
            result.Payload = stations.Payload;
            result.Message = "Get stations of area successfully";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            throw;
        }

        return result;
    }

    public async Task<OperationResult<IEnumerable<ApartmentResponse>>> GetApartmentsOfArea(
        bool? isAscending, 
        string? orderBy = null, 
        Expression<Func<Area, bool>>? filterArea = null,
        Expression<Func<Apartment, bool>>? filterApartment = null, 
        string[]? includeProperties = null, 
        int pageIndex = 0, 
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<ApartmentResponse>>();
        try
        {
            var areas = _unitOfWork.AreaRepository.FilterByExpression(filterArea);
            if (!areas.Any())
            {
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Area not found";
                return result;
            }
            var apartments = await _apartmentService.GetAll(isAscending, orderBy, filterApartment, includeProperties, pageIndex, pageSize);
            if(apartments.Payload == null)
            {
                result.StatusCode = StatusCode.Ok;
                result.Message = "No apartments found";
                return result;
            }
            result.StatusCode = StatusCode.Ok;
            result.Payload = apartments.Payload;
            result.Message = "Get apartments of area successfully";
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            throw;
        }
        return result;
    }
}