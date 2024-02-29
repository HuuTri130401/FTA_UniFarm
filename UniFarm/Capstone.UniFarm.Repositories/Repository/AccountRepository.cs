using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(FTAScript_V1Context context) : base(context)
        {
            
        }
        
        public override void SoftRemove(Account entity)
        {
            entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
            _dbSet.Update(entity);
        }
        
        public new Account Remove(Account entity)
        {
            entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
            _dbSet.Update(entity);
            return entity;
        }
    }
}
