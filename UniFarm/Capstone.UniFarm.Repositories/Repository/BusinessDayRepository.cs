using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class BusinessDayRepository : GenericRepository<BusinessDay>, IBusinessDayRepository
    {
        public BusinessDayRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<BusinessDay>> GetAllBusinessDay()
        {
            return await _dbSet
                .OrderByDescending(op => op.OpenDay)
                //.Include(m => m.Menus)
                .ToListAsync();
        }

        public async Task<IEnumerable<BusinessDay>> GetAllBusinessDayNotEndOfDayYet(Expression<Func<BusinessDay, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<BusinessDay> GetBusinessDayByIdAsync(Guid businessDayId)
        {
            return await _dbSet
                .Include(m => m.Menus)
                .FirstOrDefaultAsync(bd => bd.Id == businessDayId);
        }

        public async Task<bool> IsUniqueOpenDay(DateTime openDay)
        {
            return !await _dbSet.AnyAsync(b => b.OpenDay.Date == openDay.Date);
        }

        public async Task UpdateBusinessDayStatus(Guid businessDayId, string status)
        {
            var businessDay = await _dbSet.FirstOrDefaultAsync(bd => bd.Id == businessDayId);
            if (businessDay != null)
            {
                businessDay.Status = status;
            }
        }
    }
}
