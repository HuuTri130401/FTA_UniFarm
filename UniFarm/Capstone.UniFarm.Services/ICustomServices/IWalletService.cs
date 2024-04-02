using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IWalletService
{
    Task<OperationResult<IEnumerable<Wallet>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Wallet, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    Task<OperationResult<Wallet>> GetById(Guid objectId);
    Task<OperationResult<Wallet>> Create(WalletRequest objectRequestCreate);
    Task<OperationResult<Wallet>> Update(Guid id, Wallet objectRequestUpdate);
    Task<OperationResult<Wallet>> FindByExpression(Expression<Func<Wallet, bool>> filter , string[]? includeProperties = null);
    Task<OperationResult<WalletResponse>> GetByAccountId(Guid accountId);
}