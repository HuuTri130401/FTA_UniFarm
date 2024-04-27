using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Capstone.UniFarm.Repositories.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class ProductItemRepository : GenericRepository<ProductItem>, IProductItemRepository
    {
        public ProductItemRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<ProductItem>> FarmHubGetAllProductItemByProductId(Guid farmHubId, Guid productId)
        {
            return await _dbSet
                .Where(p => p.ProductId == productId && p.FarmHubId == farmHubId)
                .Where(pr => pr.Status != "Inactive")
                .Include(pi => pi.ProductImages)
                .ToListAsync();
        }

        public async Task<List<ProductItem>> GetAllProductItemByFarmHubId(Guid farmHubId)
        {
            return await _dbSet
                .Include(pr => pr.Product)
                    .ThenInclude(p => p.Category)
                    .Include(pi => pi.ProductImages)
                .Where(pi => pi.FarmHubId == farmHubId)
                .ToListAsync();
        }

        public async Task<List<ProductItem>> GetAllProductItemByProductId(Guid productId)
        {
            return await _dbSet
                .Where(p => p.ProductId == productId)
                .Include(pim => pim.ProductItemInMenus)
                .Include(pi => pi.ProductImages)
                .ToListAsync();
        }

        public async Task<List<ProductItem>> GetAllProductItems(ProductItemParameters productItemParameters)
        {
            var productItems = await _dbSet
                .SearchProductItems(productItemParameters.SearchTerm)
                .Where(p => p.Status == "Selling")
                .Include(pi => pi.ProductImages)
                .ToListAsync();
            var count = _dbSet.Count();
            return PagedList<ProductItem>
                .ToPagedList(productItems, count, productItemParameters.PageNumber, productItemParameters.PageSize);
        }

        public async Task<ProductItem> GetProductItemByIdAsync(Guid productId)
        {
            return await _dbSet
                .Include(pi => pi.ProductImages)
                .Include(fr => fr.FarmHub)
                .FirstOrDefaultAsync(pi => pi.Id == productId);
        }        
        
        public async Task<ProductItem> CustomerGetProductItemById(Guid productItemId, Guid menuId)
        {
            return await _dbSet
                .Include(pim => pim.ProductItemInMenus)
                    .ThenInclude(m => m.Menu)
                .Include(pi => pi.ProductImages)
                .Include(fr => fr.FarmHub)
                .FirstOrDefaultAsync(pi => pi.Id == productItemId &&
                pi.ProductItemInMenus.Any(pim => pim.MenuId == menuId));
        }
    }
}
