using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IStationService
{
    
    Task<OperationResult<IEnumerable<StationResponse>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Station, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    Task<OperationResult<StationResponse>> GetById(Guid objectId);
    Task<OperationResult<StationResponse>> GetFilterByExpression(Expression<Func<Station, bool>>? filter = null, string[]? includeProperties = null);
    Task<OperationResult<StationResponse>> Create(StationRequestCreate objectRequestCreate);
    Task<OperationResult<bool>> Delete(Guid id);
    Task<OperationResult<StationResponse>> Update(Guid id, StationRequestUpdate objectRequestUpdate);
}