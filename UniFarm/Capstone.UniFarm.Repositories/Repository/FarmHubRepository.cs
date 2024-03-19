using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class FarmHubRepository : GenericRepository<FarmHub>, IFarmHubRepository
    {
        public FarmHubRepository(FTAScript_V1Context context) : base(context)
        {

        }
        public async Task<bool> CheckFarmHubCodeAsync(string farmHubCode)
        {
            var existingFarmHub = _dbSet.FirstOrDefault(fh => fh.Code == farmHubCode);
            return existingFarmHub != null;
        }
        public async Task<bool> CheckFarmHubNameAsync(string farmHubName)
        {
            var existingFarmHub = _dbSet.FirstOrDefault(fh => fh.Name == farmHubName);
            return existingFarmHub != null;
        }
    }
}
