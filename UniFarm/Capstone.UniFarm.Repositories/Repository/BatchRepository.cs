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
    public class BatchRepository : GenericRepository<Batch>, IBatchRepository
    {
        public BatchRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<Batch>> GetAllBatchesInBusinessDay(Guid collectedHubId, Guid businessDayId)
        {
            return await _dbSet
                .Where(bd => bd.BusinessDayId == businessDayId)
                .Where(ch => ch.CollectedId == collectedHubId)
                .OrderByDescending(fs => fs.FarmShipDate)
                .ToListAsync();
        }

        public async Task<List<Batch>> GetAllBatchesByFarmHubId(Guid farmhubId)
        {
            return await _dbSet
                .Where(fr => fr.FarmHubId == farmhubId)
                .OrderByDescending(fs => fs.FarmShipDate)
                .ToListAsync();
        }
    }
}
