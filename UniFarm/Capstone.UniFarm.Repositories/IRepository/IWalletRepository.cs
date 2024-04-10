using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Repositories.IRepository;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    Task<Wallet> GetWalletByAccountIdAsync(Guid accountId);
    Task<Wallet> UpdateBalance(Guid walletId, decimal balance);
}