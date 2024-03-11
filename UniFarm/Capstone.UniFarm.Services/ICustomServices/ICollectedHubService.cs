using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface ICollectedHubService
{
    Task<OperationResult<IEnumerable<CollectedHubResponse>>> GetAll(
        bool? isAscending,
        string? orderBy = null,
        Expression<Func<CollectedHub, bool>>? filter = null,
        string[]? includeProperties = null,
        int pageIndex = 0,
        int pageSize = 10);

    Task<OperationResult<CollectedHubResponse>> GetById(Guid objectId);

    Task<OperationResult<CollectedHubResponse>> GetFilterByExpression(Expression<Func<CollectedHub, bool>> filter,
        string[]? includeProperties = null);

    Task<OperationResult<CollectedHubResponse>> Create(CollectedHubRequestCreate objectRequestCreate);
    Task<OperationResult<bool>> Delete(Guid id);
    Task<OperationResult<CollectedHubResponse>> Update(Guid id, CollectedHubRequestUpdate objectRequestUpdate);
    Task<OperationResult<CollectedHubResponseContainStaffs>> GetCollectedStaffs(Guid id);

    Task<OperationResult<IEnumerable<AboutMeResponse.StaffResponse>>>
        GetCollectedStaffsData(
            Guid id,
            bool? isAscending,
            string? orderBy = null,
            Expression<Func<Account, bool>>? filter = null,
            string[]? includeProperties = null,
            int pageIndex = 0,
            int pageSize = 10);

    Task<OperationResult<IEnumerable<AboutMeResponse.StaffResponse>>>
        GetCollectedStaffsNotWorking(
            bool? isAscending,
            string? orderBy = null,
            Expression<Func<Account, bool>>? filter = null,
            string[]? includeProperties = null,
            int pageIndex = 0,
            int pageSize = 10);
}