using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Repositories.Repository;

public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
{
    public WalletRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public override void SoftRemove(Wallet entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
    }

    public new Wallet Remove(Wallet entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
        return entity;
    }

    public async Task<Wallet> GetWalletByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Where(a => a.AccountId == accountId)
            .FirstOrDefaultAsync();
    }

    public async Task<Wallet> UpdateBalance(Guid walletId, decimal balance)
    {
        var wallet = await _dbSet.FindAsync(walletId);
        if (wallet == null)
        {
            throw new KeyNotFoundException("Wallet not found.");
        }
        wallet.Balance = balance;
        _dbSet.Update(wallet);
        return wallet;
    }
}