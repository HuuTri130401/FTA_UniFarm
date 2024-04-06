using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IApartmentStationService
{
    Task<OperationResult<ApartmentStation>> Upsert(Guid createdBy, ApartmentStationRequestCreate objectRequestCreate);

    Task<OperationResult<IEnumerable<ApartmentStationResponse>>> GetAllByAccountId(Guid accountId, bool isAscending = false,
        string? orderBy = null);
}