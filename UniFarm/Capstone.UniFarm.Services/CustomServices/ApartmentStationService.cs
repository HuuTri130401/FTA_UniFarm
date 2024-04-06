using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
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


    public async Task<OperationResult<ApartmentStation>> Upsert(Guid createdBy,
        ApartmentStationRequestCreate objectRequestCreate)
    {
        
        var result = new OperationResult<ApartmentStation>();
        try
        {
            var existStation = await _unitOfWork.StationRepository.FilterByExpression(
                x => x.Id == objectRequestCreate.StationId).FirstOrDefaultAsync();
            var existApartment = await _unitOfWork.ApartmentRepository.FilterByExpression(
                x => x.Id == objectRequestCreate.ApartmentId).FirstOrDefaultAsync();
            if (existStation == null || existApartment == null)
            {
                result.IsError = true;
                result.StatusCode = StatusCode.NotFound;
                result.Message = "Station or Apartment is not found!";
                result.Payload = null;
                return result;
            }
            var apartmentStation = new ApartmentStation
            {
                Id = Guid.NewGuid(),
                StationId = objectRequestCreate.StationId,
                ApartmentId = objectRequestCreate.ApartmentId,
                AccountId = createdBy,
                IsDefault = objectRequestCreate.IsDefault
            };
            var existAS = await _unitOfWork.ApartmentStationRepository.FilterByExpression(
                x => x.StationId == objectRequestCreate.StationId
                     && x.ApartmentId == objectRequestCreate.ApartmentId
                     && x.AccountId == createdBy)
                .FirstOrDefaultAsync();

            if (existAS != null)
            {
                existAS.IsDefault = objectRequestCreate.IsDefault;
                await _unitOfWork.ApartmentStationRepository.UpdateAsync(existAS);
                await _unitOfWork.SaveChangesAsync();
                existAS.Station = existStation;
                existAS.Apartment = existApartment;
                result.Payload = existAS;
                result.StatusCode = StatusCode.Ok;
                result.Message = "User has updated station and apartment successfully!";
            }
            else
            {
                await _unitOfWork.ApartmentStationRepository.AddAsync(apartmentStation);
                await _unitOfWork.SaveChangesAsync();
                apartmentStation.Station = existStation;
                apartmentStation.Apartment = existApartment;
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

    public async Task<OperationResult<IEnumerable<ApartmentStationResponse>>> GetAllByAccountId(Guid accountId,
        bool isAscending = false, string? orderBy = null)
    {
        var result = new OperationResult<IEnumerable<ApartmentStationResponse>>();
        try
        {
            var apartmentStations =
                _unitOfWork.ApartmentStationRepository.FilterByExpression(x => x.AccountId == accountId);
            if (orderBy != null)
            {
                if (isAscending)
                {
                    apartmentStations =
                        apartmentStations.OrderBy(x => x.GetType().GetProperty(orderBy)!.GetValue(x, null));
                }
                else
                {
                    apartmentStations =
                        apartmentStations.OrderByDescending(x => x.GetType().GetProperty(orderBy)!.GetValue(x, null));
                }
            }

            if (!apartmentStations.Any())
            {
                result.IsError = false;
                result.StatusCode = StatusCode.NotFound;
                result.Message = "User has not chosen any station and apartment yet!";
                result.Payload = null;
                return result;
            }

            var apartmentStationsList = apartmentStations.ToList();
            var apartmentStationResponse = new List<ApartmentStationResponse>();
            foreach (var item in apartmentStationsList)
            {
                var stationResponse = await _unitOfWork.StationRepository.FilterByExpression(x => x.Id == item.StationId).FirstOrDefaultAsync();
                var apartmentResponse = await _unitOfWork.ApartmentRepository.FilterByExpression(x => x.Id == item.ApartmentId).FirstOrDefaultAsync();
                var areaResponse = await _unitOfWork.AreaRepository.FilterByExpression(x => x.Id == stationResponse.AreaId).FirstOrDefaultAsync();
                var apartmentStation = _mapper.Map<ApartmentStationResponse>(item);
                apartmentStation.Station = _mapper.Map<StationResponse.StationResponseSimple>(stationResponse);
                apartmentStation.Apartment = _mapper.Map<ApartmentResponse>(apartmentResponse);
                apartmentStation.Area = _mapper.Map<AreaResponse>(areaResponse);
                apartmentStationResponse.Add(apartmentStation);
            }

            result.Payload = apartmentStationResponse;
            result.StatusCode = StatusCode.Ok;
            result.Message = "User has gotten all apartment stations successfully!";
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
}