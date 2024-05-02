using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IBusinessDayRepository : IGenericRepository<BusinessDay>
    {
        Task UpdateBusinessDayStatus(Guid businessDayId, string status);
        Task<List<BusinessDay>> GetAllBusinessDay();
        Task<BusinessDay> GetBusinessDayByIdAsync(Guid businessDayId);
        Task<BusinessDay> FarmHubGetBusinessDayByIdAsync(Guid farmhubId, Guid businessDayId);
        Task<bool> IsUniqueOpenDay(DateTime openDay);
        Task<IEnumerable<BusinessDay>> GetAllBusinessDayNotEndOfDayYet(Expression<Func<BusinessDay, bool>> predicate);
        Task<List<BusinessDay>> GetAllBusinessDayCompletedEndOfDay();
        Task<BusinessDay> GetOpendayIsToday(DateTime today);
        Task<IEnumerable<BusinessDay>> GetAllActiveBusinessDaysUpToToday();
        Task<IEnumerable<BusinessDay>> GetAllActiveBusinessDay();
    }
}
