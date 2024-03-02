using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IProductItemService
    {
        Task<OperationResult<List<ProductItemResponse>>> GetAllProductItemsByProductId(Guid productId);
        Task<OperationResult<ProductItemResponse>> GetProductItemById(Guid productItemId);
        Task<OperationResult<bool>> CreateProductItemForProduct(Guid productId, ProductItemRequest productItemRequest);
        Task<OperationResult<bool>> DeleteProductItem(Guid productItemId);
        Task<OperationResult<bool>> UpdateProductItem(Guid productItemId, ProductItemRequestUpdate productItemRequestUpdate);
    }
}
