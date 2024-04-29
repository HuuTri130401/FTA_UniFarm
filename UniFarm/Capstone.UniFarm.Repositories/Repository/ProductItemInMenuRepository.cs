using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Capstone.UniFarm.Repositories.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                .ThenInclude(pi => pi.ProductImages)
                .Include(m => m.Menu)
                .Where(mn => mn.MenuId == menuId)
                .ToListAsync();
        }

        public async Task<List<ProductItemInMenu>> GetProductItemsByMenuIdForCustomer(ProductItemInMenuParameters productItemInMenuParameters, Guid menuId)
        {
            var productItemsInMenu = await _dbSet
                .SearchProductItemsInMenu(productItemInMenuParameters.SearchTerm)
                .Include(p => p.ProductItem)
                    .ThenInclude(pi => pi.ProductImages)
                .Include(m => m.Menu)
                .Where(mn => mn.MenuId == menuId && mn.Status == "Active")
                .ToListAsync();
            var count = _dbSet.Count();
            return PagedList<ProductItemInMenu>
                .ToPagedList(productItemsInMenu, count, productItemInMenuParameters.PageNumber, productItemInMenuParameters.PageSize);
        }        
        
        public async Task<List<ProductItemInMenu>> GetProductItemInMenuByProductIdCustomer(Guid menuId)
        {
            return await _dbSet
                .Include(p => p.ProductItem)
                    .ThenInclude(pi => pi.ProductImages)
                .Include(m => m.Menu)
                .Where(mn => mn.MenuId == menuId && mn.Status == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductItemInMenu>> FindStatusProductItem(Expression<Func<ProductItemInMenu, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .ToListAsync();
        }
    }
}
