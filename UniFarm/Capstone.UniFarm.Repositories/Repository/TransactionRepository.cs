using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Capstone.UniFarm.Repositories.Repository;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public async Task<List<Transaction>> GetAllTransactions(Guid walletId)
    {
        return await _dbSet
            .Where(t => t.PayerWalletId == walletId || t.PayeeWalletId == walletId)
            .Include(w => w.PayerWallet)
            .ThenInclude(a => a.Account)
            .ToListAsync();
    }
}

