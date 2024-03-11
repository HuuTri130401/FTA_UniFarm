using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IAccountRoleService
{
    Task<OperationResult<IEnumerable<AccountRole>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<AccountRole, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    Task<OperationResult<AccountRole>> GetById(Guid id);
    Task<OperationResult<AccountRole>> Create(AccountRoleRequest objectRequestCreate);
    Task<OperationResult<bool>> Delete(Guid id);
    Task<OperationResult<bool>> DeleteByAccountId(Guid accountId);
    Task<OperationResult<AccountRole>> Update(Guid id, AccountRoleRequestUpdate objectRequestUpdate);
    Task<OperationResult<AccountRole>> GetAccountRoleByExpression(Expression<Func<AccountRole, bool>> predicate, string[]? includeProperties = null);
    
    Task<OperationResult<IEnumerable<AccountRole>>> GetAllWithoutPaging(bool? isAscending, string? orderBy = null, Expression<Func<AccountRole, bool>>? filter = null, string[]? includeProperties = null);

}