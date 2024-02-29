using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IApartmentService
{
    Task<OperationResult<IEnumerable<ApartmentResponse>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Apartment, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    Task<OperationResult<ApartmentResponse>> GetById(Guid id);
    Task<OperationResult<ApartmentResponse>> Create(ApartmentRequestCreate objectRequestCreate);
    Task<OperationResult<bool>> Delete(Guid id);
    Task<OperationResult<ApartmentResponse>> Update(Guid id, ApartmentRequestUpdate objectRequestUpdate);
}