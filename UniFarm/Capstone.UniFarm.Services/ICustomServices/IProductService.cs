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
    public interface IProductService
    {
        Task<OperationResult<List<ProductResponse>>> GetAllProducts();
        Task<OperationResult<List<ProductResponse>>> GetAllProductsByCategoryId(Guid categoryId);
        Task<OperationResult<ProductResponse>> GetProductById(Guid productId);
        Task<OperationResult<bool>> CreateProduct(ProductRequest productRequest);
        Task<OperationResult<bool>> DeleteProduct(Guid productId);
        Task<OperationResult<bool>> UpdateProduct(Guid productId, ProductRequest productRequest);
    }
}
