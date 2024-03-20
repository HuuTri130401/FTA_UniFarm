using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices;

public class ApartmentStationService : IApartmentStationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApartmentStationService(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    
    public async Task<OperationResult<ApartmentStation>> Upsert(Guid createdBy, ApartmentStationRequestCreate objectRequestCreate)
    {
        var apartmentStation = new ApartmentStation
        {
            Id = Guid.NewGuid(),
            StationId = objectRequestCreate.StationId,
            ApartmentId = objectRequestCreate.ApartmentId,
            AccountId = createdBy,
            IsDefault = objectRequestCreate.IsDefault
        };
        var result = new OperationResult<ApartmentStation>();
        try
        {
            var existAS = await _unitOfWork.ApartmentStationRepository.FilterByExpression(
                x => x.StationId == objectRequestCreate.StationId 
                     && x.ApartmentId == objectRequestCreate.ApartmentId  
                     && x.AccountId == createdBy).FirstOrDefaultAsync();

            if (existAS != null)
            {
                existAS.IsDefault = objectRequestCreate.IsDefault;
                await _unitOfWork.ApartmentStationRepository.UpdateAsync(existAS);
                await _unitOfWork.SaveChangesAsync();
                result.Payload = existAS;
                result.StatusCode = StatusCode.Ok;
                result.Message = "User has updated station and apartment successfully!";
            }
            else
            {
                await _unitOfWork.ApartmentStationRepository.AddAsync(apartmentStation);
                await _unitOfWork.SaveChangesAsync();
                result.Payload = apartmentStation;
                result.StatusCode = StatusCode.Ok;
                result.Message = "User has chosen station and apartment successfully!";
            }
        }
        catch (Exception e)
        {
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            result.Message = e.Message;
            result.Payload = null;
            return result;
        }
        return result;
    }

    public Task<OperationResult<IEnumerable<ApartmentStation>>> GetAllByAccountId(Guid accountId, bool isAscending = false, string? orderBy = null)
    {
        var result = new OperationResult<IEnumerable<ApartmentStation>>();
        try
        {
            var apartmentStations = _unitOfWork.ApartmentStationRepository.FilterByExpression(x => x.AccountId == accountId);
            if (orderBy != null)
            {
                if (isAscending)
                {
                    apartmentStations = apartmentStations.OrderBy(x => x.GetType().GetProperty(orderBy)!.GetValue(x, null));
                }
                else
                {
                    apartmentStations = apartmentStations.OrderByDescending(x => x.GetType().GetProperty(orderBy)!.GetValue(x, null));
                }
            }
            if(!apartmentStations.Any())
            {
                result.IsError = false;
                result.StatusCode = StatusCode.NotFound;
                result.Message = "User has not chosen any station and apartment yet!";
                result.Payload = null;
                return Task.FromResult(result);
            }

            foreach (var item in apartmentStations)
            {
                item.Station = _unitOfWork.StationRepository.FilterByExpression(x => x.Id == item.StationId).FirstOrDefault();
                item.Apartment = _unitOfWork.ApartmentRepository.FilterByExpression(x => x.Id == item.ApartmentId).FirstOrDefault();
            }
            
            result.Payload = apartmentStations.ToList();
            result.StatusCode = StatusCode.Ok;
            result.Message = "User has gotten all apartment stations successfully!";
        }
        catch (Exception e)
        {
            result.IsError = true;
            result.StatusCode = StatusCode.ServerError;
            result.Message = e.Message;
            result.Payload = null;
            return Task.FromResult(result);
        }
        return Task.FromResult(result);
    }

    
}