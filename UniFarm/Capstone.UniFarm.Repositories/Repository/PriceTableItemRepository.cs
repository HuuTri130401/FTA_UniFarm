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
    public class PriceTableItemRepository : GenericRepository<PriceTableItem>, IPriceTableItemRepository
    {
        public PriceTableItemRepository(FTAScript_V1Context context) : base(context)
        {
        }
    }
}
