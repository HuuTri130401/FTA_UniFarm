using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IPriceTableRepository : IGenericRepository<PriceTable>
    {
        Task<PriceTable> GetPriceTable();
        Task<List<PriceTable>> GetAllPrice();
    }
}
