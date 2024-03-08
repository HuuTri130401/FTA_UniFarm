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
    public class ProductItemInMenuRepository : GenericRepository<ProductItemInMenu>, IProductItemInMenuRepository
    {
        public ProductItemInMenuRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<ProductItemInMenu>> GetProductItemsByMenuId(Guid menuId)
        {
            return await _dbSet
                .Include(p => p.ProductItem)
                .Include(m => m.Menu)
                .Where(mn => mn.MenuId == menuId).ToListAsync();
        }

        public async Task<bool> GetProductItemByMenuId(Guid menuId)
        {
            return await _dbSet.AnyAsync(mn => mn.MenuId == menuId);
        }

        public void DeleteProductItemInMenu(ProductItemInMenu productItemInMenu)
        {
            _dbSet.Remove(productItemInMenu);
        }
        public async Task<ProductItemInMenu> GetByMenuIdAndProductItemId(Guid menuId, Guid productItemId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pim => pim.MenuId == menuId && pim.ProductItemId == productItemId);
        }
    }
}
