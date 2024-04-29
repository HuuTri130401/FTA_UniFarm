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
    public interface IProductItemInMenuRepository : IGenericRepository<ProductItemInMenu>
    {
        Task<List<ProductItemInMenu>> GetProductItemsByMenuId(Guid menuId);
        Task<List<ProductItemInMenu>> GetProductItemsByMenuIdForCustomer(ProductItemInMenuParameters productItemInMenuParameters, Guid menuId);
        Task<List<ProductItemInMenu>> GetProductItemInMenuByProductIdCustomer(Guid menuId);
        Task<IEnumerable<ProductItemInMenu>> FindStatusProductItem(Expression<Func<ProductItemInMenu, bool>> predicate);
    }
}
