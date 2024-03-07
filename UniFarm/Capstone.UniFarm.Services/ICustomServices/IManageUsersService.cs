using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IManageUsersService
{
    Task<OperationResult<AccountRequestCreate>> CreateAccountForAdmin(AccountRequestCreate accountRequestCreate);

    Task<OperationResult<IEnumerable<AccountResponse>>> GetAllAccountsForAdmin(bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    
    Task<OperationResult<IEnumerable<AccountResponse>>> GetAllCustomersForAdmin(bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
        
    Task<OperationResult<IEnumerable<AccountResponse>>> GetAllFarmHubsForAdmin(bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
        
    Task<OperationResult<IEnumerable<AccountResponse>>> GetAllCollectedStaffsForAdmin(bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
        
    Task<OperationResult<IEnumerable<AccountResponse>>> GetAllStationStaffsForAdmin(bool? isAscending, string? orderBy = null, Expression<Func<Account, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    
    Task<OperationResult<AccountResponse>> GetAccountByExpression(Expression<Func<Account, bool>> predicate, string[]? includeProperties = null);
    
    Task<OperationResult<bool>> DeleteAccount(Guid id);


}