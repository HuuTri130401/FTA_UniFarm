using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.Repositories.Repository;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public async Task<bool> AlreadyRefundedAsync(Guid orderId)
    {
        return await Task.FromResult(AlreadyRefunded(orderId));
    }

    public async Task<List<Transaction>> GetAllTransactionPayments()
    {
        return await _dbSet
            .Where(pst => pst.TransactionType == TransactionEnum.Payment.ToString())
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetAllTransactions(Guid walletId)
    {
        return await _dbSet
            .Where(t => t.PayerWalletId == walletId || t.PayeeWalletId == walletId)
            .Include(w => w.PayerWallet)
            .ThenInclude(a => a.Account)
            .OrderByDescending(t => t.PaymentDate)
            .ToListAsync();
    }

    private bool AlreadyRefunded(Guid orderId)
    {
        return _dbSet.Any(t => t.OrderId == orderId && t.TransactionType == TransactionEnum.Refund.ToString());
    }
}

