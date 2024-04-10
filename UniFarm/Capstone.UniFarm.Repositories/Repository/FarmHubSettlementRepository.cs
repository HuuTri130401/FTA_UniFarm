using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class FarmHubSettlementRepository : GenericRepository<FarmHubSettlement>, IFarmHubSettlementRepository
    {
        public FarmHubSettlementRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<FarmHubSettlement> GetFarmHubSettlementAsync(Guid businessDayId, Guid farmHubId)
        {
                return await _dbSet
                .Include(b => b.BusinessDay)
                .Where(s => s.FarmHubId == farmHubId && s.BusinessDayId == businessDayId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<FarmHubSettlement>> GetAllFarmHubSettlementAsync(Guid businessDayId)
        {
            return await _dbSet
            .Include(b => b.BusinessDay)
            .Where(s => s.BusinessDayId == businessDayId)
            .ToListAsync();
        }
    }
}
