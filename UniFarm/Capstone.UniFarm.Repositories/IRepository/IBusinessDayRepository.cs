using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IBusinessDayRepository : IGenericRepository<BusinessDay>
    {
        Task UpdateBusinessDayStatus(Guid businessDayId, string status);
        List<BusinessDay> GetAllBusinessDay();
        Task<BusinessDay> GetBusinessDayByIdAsync(Guid businessDayId);
    }
}
