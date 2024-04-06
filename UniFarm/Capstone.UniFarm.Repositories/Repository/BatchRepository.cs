﻿using Capstone.UniFarm.Domain.Data;
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
                .Include(b => b.BusinessDay)
                .Where(bd => bd.BusinessDayId == businessDayId)
                .Where(ch => ch.CollectedId == collectedHubId)
                .OrderByDescending(fs => fs.FarmShipDate)
                .ToListAsync();
        }

        public async Task<List<Batch>> GetAllBatchesByFarmHubId(Guid farmhubId)
        {
            return await _dbSet
                .Include(b => b.BusinessDay)
                .Include(f => f.Collected)
                .Where(fr => fr.FarmHubId == farmhubId)
                .OrderByDescending(fs => fs.FarmShipDate)
                .ToListAsync();
        }

        public async Task<Batch> GetAllOrdersInBatch(Guid batchId)
        {
            var batch = await _dbSet
                .Include(cl => cl.Collected)
                .Include(fr => fr.FarmHub)
                .Include(bs => bs.BusinessDay)
                .Include(od => od.Orders)
                .ThenInclude(odt => odt.OrderDetails)
                .FirstOrDefaultAsync(bt => bt.Id == batchId);

            int orderCount = 0;
            if (batch != null)
            {
                orderCount = batch.Orders.Count;
                batch.NumberOfOrdersInBatch = orderCount;
            }
            return batch;
        }

        public async Task<List<Batch>> GetAllBatchesByFarmHubIdAndBusinessDayId(Guid farmhubId, Guid businessDayId)
        {
            return await _dbSet
                .Include(b => b.BusinessDay)
                .Include(f => f.Collected)
                .Where(fr => fr.FarmHubId == farmhubId && fr.BusinessDayId == businessDayId)
                .OrderByDescending(fs => fs.FarmShipDate)
                .ToListAsync();
        }
    }
}
