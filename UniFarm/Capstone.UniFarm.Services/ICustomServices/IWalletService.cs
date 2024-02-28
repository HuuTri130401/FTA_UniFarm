using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IWalletService
{
    Task<OperationResult<IEnumerable<Wallet>>> GetAll(bool? isAscending, string? orderBy = null, Expression<Func<Area, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
    Task<OperationResult<Wallet>> GetById(Guid objectId);
    Task<OperationResult<Wallet>> Create(WalletRequest objectRequestCreate);
    Task<OperationResult<Wallet>> Update(Guid Id, Wallet objectRequestUpdate);
}