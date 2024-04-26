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
    public class PriceTableRepository : GenericRepository<PriceTable>, IPriceTableRepository
    {
        public PriceTableRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<PriceTable>> GetAllPrice()
        {
            return await _dbSet
                .Include(p => p.PriceTableItems)
                .ToListAsync();
        }

        public async Task<PriceTable> GetPriceTable()
        {
            return await _dbSet
                .Include(pt => pt.PriceTableItems)
                .Where(pt => pt.FromDate <= DateTime.UtcNow.AddHours(7) && pt.ToDate >= DateTime.UtcNow.AddHours(7) && pt.Status == "Active")
                .OrderByDescending(pt => pt.FromDate)
                .FirstOrDefaultAsync();
        }
    }
}
