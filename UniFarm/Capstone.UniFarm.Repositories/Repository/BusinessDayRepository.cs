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
    public class BusinessDayRepository : GenericRepository<BusinessDay>, IBusinessDayRepository
    {
        public BusinessDayRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public List<BusinessDay> GetAllBusinessDay()
        {
            return _dbSet.ToList();
        }

        public async Task UpdateBusinessDayStatus(Guid businessDayId, string status)
        {
            var businessDay = await _dbSet.FirstOrDefaultAsync(bd => bd.Id == businessDayId);
            if(businessDay != null)
            {
                businessDay.Status = status;
            }
        }
    }
}
