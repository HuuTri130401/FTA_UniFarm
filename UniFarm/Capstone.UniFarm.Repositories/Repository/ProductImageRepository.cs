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
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(FTAScript_V1Context context) : base(context)
        {
        }
        public virtual async Task<List<ProductImage>> GetAllProductImageAsync(Guid productItemId)
        {
            return await _dbSet.Where(pi => pi.ProductItemId == productItemId).ToListAsync();
        }
    }
}
