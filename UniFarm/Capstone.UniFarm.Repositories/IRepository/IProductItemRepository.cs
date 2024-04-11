using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IProductItemRepository : IGenericRepository<ProductItem>
    {
        Task<List<ProductItem>> SearchProductItems(ProductItemParameters productItemParameters);
        Task<List<ProductItem>> GetAllProductItems(ProductItemParameters productItemParameters);
        Task<List<ProductItem>> GetAllProductItemByProductId(Guid productId);
        //Task<List<ProductItem>> SearchProductItems(ProductItemParameters productItemParameters, Guid businessDayId);
        //Task<List<ProductItem>> GetAllProductItems(ProductItemParameters productItemParameters, Guid businessDayId);
        //Task<List<ProductItem>> GetAllProductItemByProductId(Guid productId, Guid businessDayId);
        Task<List<ProductItem>> FarmHubGetAllProductItemByProductId(Guid farmHubId, Guid productId);
        Task<List<ProductItem>> GetAllProductItemByFarmHubId(Guid farmHubId);
        Task<ProductItem> GetProductItemByIdAsync(Guid productId);
        Task<ProductItem> CustomerGetProductItemById(Guid productItemId, Guid menuId);
    }
}
