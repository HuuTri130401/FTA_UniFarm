using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Repositories.Repository;

public class AccountRoleRepository : GenericRepository<AccountRole>, IAccountRoleRepository
{
    public AccountRoleRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public override void SoftRemove(AccountRole entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
    }

    public new AccountRole Remove(AccountRole entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
        return entity;
    }

    public async Task<AccountRole> GetAccountRoleByAccountIdAsync(Guid farmhubAccountId)
    {
        return await _dbSet.FirstOrDefaultAsync(fr => fr.AccountId == farmhubAccountId);
    }
}
