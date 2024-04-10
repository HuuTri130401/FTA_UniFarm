using Capstone.UniFarm.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Capstone.UniFarm.Repositories.IRepository;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Task<List<Transaction>> GetAllTransactions(Guid walletId);
}