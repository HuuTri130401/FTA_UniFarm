using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface ITransactionService
{
    Task<OperationResult<IEnumerable<Transaction>>> GetAll(bool? isAscending, string? orderBy = null,
        Expression<Func<Transaction, bool>>? filter = null, string[]? includeProperties = null, int pageIndex = 0,
        int pageSize = 10);
}