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
using static System.Net.Mime.MediaTypeNames;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductImageService> _logger;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductImageService(IUnitOfWork unitOfWork, ILogger<ProductImageService> logger, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<OperationResult<bool>> CreateProductImage(Guid productItemId, ProductImageRequest productImageRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemId);
                if (existingProductItem != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(productImageRequest.ImageUrl);
                    var productImage = _mapper.Map<ProductImage>(productImageRequest);
                    productImage.ImageUrl = imageUrl;
                    productImage.ProductItemId = productItemId;
                    productImage.Status = "Active";
                    await _unitOfWork.ProductImageRepository.AddAsync(productImage);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Image for ProductItem Success!", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Add Image for ProductItem Failed!"); 
                    }
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Image for ProductItem Failed!"); 
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in CreateProductImage service method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteProductImage(Guid productImageId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductImage = await _unitOfWork.ProductImageRepository.GetByIdAsync(productImageId);
                if (existingProductImage != null)
                {
                    existingProductImage.Status = "Inactive";
                    _unitOfWork.ProductImageRepository.Update(existingProductImage);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Product Image have Id: {productImageId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Product Image Failed!"); 
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Product Image have Id: {productImageId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in DeleteProductImage service method for productImage ID: {productImageId}");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductImageResponse>>> GetAllProductImagesByProductItemId(Guid productItemId)
        {
            var result = new OperationResult<List<ProductImageResponse>>();
            try
            {
                var listProductItemImages = await _unitOfWork.ProductImageRepository.GetAllProductImageAsync(productItemId);
                var activeProductItemImages = listProductItemImages.Where(pi => pi.Status == "Active").ToList();
                var listProductImagesResponse = _mapper.Map<List<ProductImageResponse>>(activeProductItemImages);

                if (listProductImagesResponse == null || !listProductImagesResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Product Images with ProductItem Id {productItemId} is Empty!", listProductImagesResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Images Done.", listProductImagesResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProductImagesByProductItemId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ProductImageResponse>> GetProductImageById(Guid productImageId)
        {
            var result = new OperationResult<ProductImageResponse>();
            try
            {
                var productItemImage = await _unitOfWork.ProductImageRepository.GetByIdAsync(productImageId);

                if (productItemImage == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Product Image with Id: {productImageId}");
                    return result;
                }
                else if (productItemImage.Status == "Active")
                {
                    var productImageResponse = _mapper.Map<ProductImageResponse>(productItemImage);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get Product Image by Id: {productImageId} Success!", productImageResponse);
                    return result;
                }
                result.AddError(StatusCode.NotFound, $"Can't found Product Image with Id: {productImageId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetProductImageById service method for productImage ID: {productImageId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateProductImage(Guid productImageId, ProductImageRequestUpdate productImageRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductImage = await _unitOfWork.ProductImageRepository.GetByIdAsync(productImageId);

                if (existingProductImage != null)
                {
                    if (productImageRequestUpdate.ImageUrl != null)
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(productImageRequestUpdate.ImageUrl);
                        existingProductImage.ImageUrl = imageUrl;
                    }
                    if (productImageRequestUpdate.ProductItemId != null)
                    {
                        var existingProduct = await _unitOfWork.ProductItemRepository.GetByIdAsync((Guid)productImageRequestUpdate.ProductItemId);
                        if (existingProduct == null)
                        {
                            result.AddError(StatusCode.BadRequest, "Product Image must belong to a product item to update!");
                            return result;
                        }
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
                } else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Product Image have Id: {productImageId}!", false);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdateProductImage service method");
                throw;
            }
        }
    }
}
