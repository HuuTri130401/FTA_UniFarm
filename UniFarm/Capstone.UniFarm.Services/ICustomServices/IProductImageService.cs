using Capstone.UniFarm.Domain.Models;
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
    public interface IProductImageService
    {
        Task<OperationResult<List<ProductImageResponse>>> GetAllProductImagesByProductId(Guid productId);
        Task<OperationResult<ProductImageResponse>> GetProductImageById(Guid productImageId);
        Task<OperationResult<bool>> CreateProductImage(Guid productId, ProductImageRequest productImageRequest);
        Task<OperationResult<bool>> DeleteProductImage(Guid productImageId);
        Task<OperationResult<bool>> UpdateProductImage(Guid productImageId, ProductImageRequestUpdate productImageRequestUpdate);
    }
}
