using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Repositories.IRepository;

public interface IAccountRoleRepository : IGenericRepository<AccountRole>
{
        Task<AccountRole> GetAccountRoleByAccountIdAsync(Guid farmhubAccountId);
}