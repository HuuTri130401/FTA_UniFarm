using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Capstone.UniFarm.Repositories.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<Menu>> GetAllMenuByFarmHubIdAsync(Guid farmHubId)
        {
            return await _dbSet.Where(fh => fh.FarmHubId == farmHubId).ToListAsync();
        }

        public async Task<List<Menu>> GetAllMenuInCurrentBusinessDay(Guid businessDayId)
        {
            return await _dbSet.Where(b => b.BusinessDayId == businessDayId).ToListAsync();
        }

        public async Task<Menu> GetSingleOrDefaultMenuAsync(Expression<Func<Menu, bool>> predicate)
        {
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }
    }
}
