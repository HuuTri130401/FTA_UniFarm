using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task<List<Menu>> GetAllMenuByFarmHubIdAsync(Guid farmHubId);
        Task<List<ProductItem>> GetProductItemsByBusinessDayAsync(ProductItemParameters productItemParameters, Guid businessDayId);
        Task<List<ProductItem>> GetAllProductItemByProductId(Guid productId, Guid businessDayId);
        Task<PagedList<ProductItem>> GetProductItemsByBusinessDayInHomeScreenAsync(
            ProductItemParameters productItemParameters, Guid businessDayId);
        Task<List<Menu>> GetAllMenuInCurrentBusinessDay(Guid businessDayId);
        Task<Menu> GetSingleOrDefaultMenuAsync(Expression<Func<Menu, bool>> predicate);
    }
}
