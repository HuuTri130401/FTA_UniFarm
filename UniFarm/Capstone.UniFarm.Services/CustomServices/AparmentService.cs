using System.Linq.Expressions;
using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.CustomServices;

public class ApartmentService : IApartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApartmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<OperationResult<IEnumerable<ApartmentResponse>>> GetAll(bool? isAscending,
        string? orderBy = null,
        Expression<Func<Apartment, bool>>? filter = null,
        string[]? includeProperties = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<IEnumerable<ApartmentResponse>>();
        try
        {
            var listApartments = _unitOfWork.ApartmentRepository.FilterAll(isAscending, orderBy, filter, includeProperties, pageIndex, pageSize);
            if (!listApartments.Any())
            {
                result.AddError(StatusCode.NotFound, "Apartment not found");
                result.StatusCode = StatusCode.NotFound;
                return Task.FromResult(result);
            }
            result.Payload = _mapper.Map<IEnumerable<ApartmentResponse>>(listApartments);
            result.StatusCode = StatusCode.Ok;
            result.Message = "Get all apartments successfully";
            result.IsError = false;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            result.IsError = true;
            throw;
        }
        return Task.FromResult(result);
    }
    
    

    public async Task<OperationResult<ApartmentResponse>> GetById(Guid objectId)
    {
        var result = new OperationResult<ApartmentResponse>();
        try
        {
            var objectApartment = await _unitOfWork.ApartmentRepository.GetByIdAsync(objectId);
            result.Payload = _mapper.Map<ApartmentResponse>(objectApartment);
            result.StatusCode = StatusCode.Ok;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }

    public async Task<OperationResult<ApartmentResponse>> Create(ApartmentRequestCreate objectRequestCreate)
    {
        var result = new OperationResult<ApartmentResponse>();
        try
        {
            var objectApartment = _mapper.Map<Apartment>(objectRequestCreate);
            await _unitOfWork.ApartmentRepository.AddAsync(objectApartment);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count > 0)
            {
                result.Payload = _mapper.Map<ApartmentResponse>(objectApartment);
                result.StatusCode = StatusCode.Created;
            }
            else
            {
                result.AddError(StatusCode.BadRequest, "Create Apartment Failed");
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
            var objectApartment = await _unitOfWork.ApartmentRepository.GetByIdAsync(id);
            if (objectApartment == null)
            {
                result.AddError(StatusCode.NotFound, "Apartment not found");
                return result;
            }

            _unitOfWork.ApartmentRepository.SoftRemove(objectApartment);
            var count = await _unitOfWork.SaveChangesAsync();
            if (count > 0)
            {
                result.Payload = true;
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.AddError(StatusCode.BadRequest, "Delete Apartment Failed");
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }

    public async Task<OperationResult<ApartmentResponse>> Update(Guid id, ApartmentRequestUpdate objectRequestUpdate)
    {
        var result = new OperationResult<ApartmentResponse>();
        try
        {
            var objectApartment = await _unitOfWork.ApartmentRepository.GetByIdAsync(id);
            if (objectApartment == null)
            {
                result.AddError(StatusCode.NotFound, "Apartment not found");
                return result;
            }

            _mapper.Map(objectRequestUpdate, objectApartment);
            _unitOfWork.ApartmentRepository.Update(objectApartment);
            var count = _unitOfWork.SaveChangesAsync();
            if (count.Result > 0)
            {
                result.Payload = _mapper.Map<ApartmentResponse>(objectApartment);
                result.StatusCode = StatusCode.Ok;
            }
            else
            {
                result.AddError(StatusCode.BadRequest, "Update Apartment Failed");
            }
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
            throw;
        }

        return result;
    }
    
}