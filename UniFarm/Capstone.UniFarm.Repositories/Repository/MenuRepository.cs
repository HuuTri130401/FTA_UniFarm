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
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<Menu>> GetAllMenuByFarmHubIdAsync(Guid farmHubId)
        {
            return await _dbSet.Where(fh => fh.FarmHubId == farmHubId).ToListAsync();   
        }
    }
}
