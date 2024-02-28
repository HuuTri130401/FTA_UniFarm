using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IAreaService
{
    Task<OperationResult<IEnumerable<AreaResponse>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Area, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    Task<OperationResult<AreaResponse>> GetById(Guid objectId);
    Task<OperationResult<AreaResponse>> Create(AreaRequestCreate objectRequestCreate);
    Task<OperationResult<bool>> Delete(Guid id);
    Task<OperationResult<AreaResponse>> Update(Guid Id, AreaRequestUpdate objectRequestUpdate);
}