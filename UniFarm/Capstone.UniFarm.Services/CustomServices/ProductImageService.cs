using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductImageService> _logger;
        private readonly IMapper _mapper;

        public ProductImageService(IUnitOfWork unitOfWork, ILogger<ProductImageService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> CreateProductImage(Guid productId,ProductImageRequest productImageRequest)
        {
            var result = new OperationResult<bool>();

            try
            {
                var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (existingProduct != null)
                {
                    var productImage = _mapper.Map<ProductImage>(productImageRequest);
                    productImage.ProductId = productId; 
                    await _unitOfWork.ProductImageRepository.AddAsync(productImage);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Image to Product Success!", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Add Image to Product Failed!"); ;
                    }
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Image to Product Failed!"); ;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<OperationResult<bool>> DeleteProductImage(Guid productImageId)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<List<ProductImageResponse>>> GetAllProductImagesByProductId(Guid productId)
        {
            var result = new OperationResult<List<ProductImageResponse>>();
            try
            {
                var listProductImages = await _unitOfWork.ProductImageRepository.GetAllProductImageAsync(productId);
                var listProductImagesResponse = _mapper.Map<List<ProductImageResponse>>(listProductImages);

                if (listProductImagesResponse == null || !listProductImagesResponse.Any())
                {
                    result.AddError(StatusCode.NotFound, "List ProductImages is Empty!");
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Images Done.", listProductImagesResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllFarmHubs Service Method");
                throw;
            }
            throw new NotImplementedException();
        }

        public Task<OperationResult<ProductImageResponse>> GetProductImageById(Guid productImageId)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> UpdateProductImage(Guid productImageId, ProductImageRequestUpdate productImageRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductImage = await _unitOfWork.ProductImageRepository.GetByIdAsync(productImageId);

                if (existingProductImage != null)
                {
                    if (productImageRequestUpdate.Image != null)
                    {
                        existingProductImage.Image = productImageRequestUpdate.Image;
                    }
                    if (productImageRequestUpdate.ProductId != null)
                    {
                        var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync((Guid)productImageRequestUpdate.ProductId);
                        if(existingProduct == null)
                        {
                            result.AddError(StatusCode.BadRequest, "Product Image must belong to a product to update.!");
                            return result;
                        }
                    }
                    if (productImageRequestUpdate.DisplayIndex != null)
                    {
                        existingProductImage.DisplayIndex = productImageRequestUpdate.DisplayIndex;
                    }

                    _unitOfWork.ProductImageRepository.Update(existingProductImage);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update ProductImage have Id: {productImageId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update ProductImage Failed!"); 
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
