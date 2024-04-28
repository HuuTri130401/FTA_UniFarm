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
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<Menu>> GetAllMenuByFarmHubIdAsync(Guid farmHubId)
        {
            return await _dbSet.Where(fh => fh.FarmHubId == farmHubId).ToListAsync();
        }

        public async Task<List<ProductItem>> GetAllProductItemByProductId(Guid productId, Guid businessDayId)
        {
            return await _dbSet
                   .Where(m => m.BusinessDayId == businessDayId && m.Status == "Active")
                   .SelectMany(m => m.ProductItemInMenus)
                   .Include(od => od.ProductItem)
                   .ThenInclude(pi => pi.Product)
                   .Where(p => p.ProductItem.Product.Id == productId)
                   .Where(pim => pim.Status == "Active")
                   .Select(pim => pim.ProductItem)
                   .Distinct()
                   .ToListAsync();
        }

        public async Task<List<ProductItem>> GetProductItemsByBusinessDayAsync(ProductItemParameters productItemParameters, Guid businessDayId)
        {
            var productItems = await _dbSet
                   .Where(m => m.BusinessDayId == businessDayId && m.Status == "Active")
                   .SelectMany(m => m.ProductItemInMenus)
                   .Where(pim => pim.Status == "Active")
                   .Select(pim => pim.ProductItem)
                   .Distinct()
                   .ToListAsync();
            var count = _dbSet.Count();
            return PagedList<ProductItem>
                .ToPagedList(productItems, count, productItemParameters.PageNumber, productItemParameters.PageSize);
        }

        public async Task<PagedList<ProductItem>> GetProductItemsByBusinessDayInHomeScreenAsync(
            ProductItemParameters productItemParameters, Guid businessDayId)
        {
            var productItemInMenus = await _dbSet
                .Where(m => m.BusinessDayId == businessDayId && m.Status == "Active")
                .SelectMany(m => m.ProductItemInMenus)
                .Where(pim => pim.Status == "Active")
                .Include(pim => pim.ProductItem)
                .ToListAsync();

            var productItemsWithSoldInfo = productItemInMenus
                .GroupBy(pim => pim.ProductItem.Id)
                .Select(g => new
                {
                    ProductItem = g.First().ProductItem, // Giả định rằng tất cả các item trong một nhóm đều có cùng ProductItem
                    TotalSold = g.Sum(pim => pim.Sold)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToList();
            var count = _dbSet.Count();

            var sortedProductItems = productItemsWithSoldInfo
                .Select(x => x.ProductItem)
                .ToList();

            // Tạo PagedList từ kết quả
            return new PagedList<ProductItem>(sortedProductItems, count, productItemParameters.PageNumber, productItemParameters.PageSize);
        }

        public async Task<List<Menu>> GetAllMenuInCurrentBusinessDay(Guid businessDayId)
        {
            return await _dbSet.Where(b => b.BusinessDayId == businessDayId).ToListAsync();
        }

        public async Task<Menu> GetSingleOrDefaultMenuAsync(Expression<Func<Menu, bool>> predicate)
        {
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }
    }
}
