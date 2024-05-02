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
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class BusinessDayRepository : GenericRepository<BusinessDay>, IBusinessDayRepository
    {
        public BusinessDayRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<BusinessDay> FarmHubGetBusinessDayByIdAsync(Guid farmhubId, Guid businessDayId)
        {
            return await _dbSet
                .Include(bd => bd.Menus.Where(menu => menu.FarmHubId == farmhubId))
                .FirstOrDefaultAsync(bd => bd.Id == businessDayId);
        }

        public async Task<List<BusinessDay>> GetAllBusinessDay()
        {
            return await _dbSet
                .OrderByDescending(op => op.OpenDay)
                //.Include(m => m.Menus)
                .ToListAsync();
        }

        public async Task<List<BusinessDay>> GetAllBusinessDayCompletedEndOfDay()
        {
            return await _dbSet
                .Where(st => st.Status == CommonEnumStatus.Completed.ToString())
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
                .AsNoTracking()
                .Include(m => m.Menus)
                .FirstOrDefaultAsync(bd => bd.Id == businessDayId);
        }

        public async Task<bool> IsUniqueOpenDay(DateTime openDay)
        {
            return !await _dbSet.AnyAsync(b => b.OpenDay.Date == openDay.Date);
        }

        public async Task<BusinessDay> GetOpendayIsToday(DateTime today)
        {
            return await _dbSet.FirstOrDefaultAsync(op => op.OpenDay.Date == today.Date);
        }

        public async Task UpdateBusinessDayStatus(Guid businessDayId, string status)
        {
            var businessDay = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(bd => bd.Id == businessDayId);
            if (businessDay != null)
            {
                businessDay.Status = status;
                _dbSet.Update(businessDay);
            }
        }

        public async Task<IEnumerable<BusinessDay>> GetAllActiveBusinessDaysUpToToday()
        {
            return await _dbSet
            .Where(bd => bd.OpenDay <= DateTime.UtcNow.AddHours(7).Date && bd.Status == "Active")
            .ToListAsync();
        }        
        
        public async Task<IEnumerable<BusinessDay>> GetAllActiveBusinessDay()
        {
            return await _dbSet
            .Where(bd => bd.Status == "Active")
            .ToListAsync();
        }
    }
}
