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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> CreateProduct(ProductRequest productRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(productRequest.CategoryId);
                if (existingCategory == null)
                {
                    result.AddError(StatusCode.NotFound, $"Category with id: {productRequest.CategoryId} not found!");
                    return result;
                }
                else
                {
                    var product = _mapper.Map<Product>(productRequest);
                    product.CreatedAt = DateTime.Now;
                    product.Status = "Active";
                    await _unitOfWork.ProductRepository.AddAsync(product);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Product Success!", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Add Product Failed!"); ;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteProduct(Guid productId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (existingProduct != null)
                {
                    existingProduct.Status = "Inactive";
                    _unitOfWork.ProductRepository.Update(existingProduct);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Product have Id: {productId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Product Failed!"); ;
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Product have Id: {productId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<ProductResponse>>> GetAllProducts()
        {
            var result = new OperationResult<List<ProductResponse>>();
            try
            {
                var listProducts = await _unitOfWork.ProductRepository.GetAllAsync();
                var activeProducts = listProducts.Where(pr => pr.Status == "Active").ToList();
                var listProductsResponse = _mapper.Map<List<ProductResponse>>(activeProducts);

                if (listProductsResponse == null || !listProductsResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List Products is Empty!", listProductsResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Products Done.", listProductsResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProducts Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductResponse>>> GetAllProductsByCategoryId(Guid categoryId)
        {
            var result = new OperationResult<List<ProductResponse>>();
            try
            {
                var listProducts = await _unitOfWork.ProductRepository.GetAllProductByCategoryId(categoryId);
                var activeProducts = listProducts.Where(pr => pr.Status == "Active").ToList();
                var listProducsResponse = _mapper.Map<List<ProductResponse>>(activeProducts);

                if (listProducsResponse == null || !listProducsResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Products with Category Id {categoryId} is Empty!", listProducsResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, $"Get List Products with Category Id {categoryId} Done!", listProducsResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProductsByCategoryId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ProductResponse>> GetProductById(Guid productId)
        {
            var result = new OperationResult<ProductResponse>();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Product with Id: {productId}");
                    return result;
                } else if(product.Status == "Active")
                {
                    var productResponse = _mapper.Map<ProductResponse>(product);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get Product by Id: {productId} Success!", productResponse);
                }
                result.AddError(StatusCode.NotFound, $"Can't found Product with Id: {productId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetProductById service method for category ID: {productId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateProduct(Guid productId, ProductRequestUpdate productRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (existingProduct != null)
                {
                    bool isAnyFieldUpdated = false;
                    if (productRequest.CategoryId != null)
                    {
                        var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync((Guid)productRequest.CategoryId);
                        if (existingCategory == null)
                        {
                            result.AddError(StatusCode.NotFound, $"Category with id: {productRequest.CategoryId} not found!"); ;
                            return result;
                        }
                        existingProduct.CategoryId = productRequest.CategoryId;
                        isAnyFieldUpdated = true;
                    }
                    if (productRequest.Name != null)
                    {
                        existingProduct.Name = productRequest.Name;
                        isAnyFieldUpdated = true;
                    }
                    if (productRequest.Code != null)
                    {
                        existingProduct.Code = productRequest.Code;
                        isAnyFieldUpdated = true;
                    }
                    if (productRequest.Description != null)
                    {
                        existingProduct.Description = productRequest.Description;
                        isAnyFieldUpdated = true;
                    }
                    if (productRequest.Label != null)
                    {
                        existingProduct.Label = productRequest.Label;
                        isAnyFieldUpdated = true;
                    }
                    if (productRequest.Status != null && (productRequest.Status == "Active" || productRequest.Status == "Inactive"))
                    {
                        existingProduct.Status = productRequest.Status;
                        isAnyFieldUpdated = true;
                    }
                    if (isAnyFieldUpdated)
                    {
                        existingProduct.UpdatedAt = DateTime.Now;
                    }

                    _unitOfWork.ProductRepository.Update(existingProduct);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update Product have Id: {productId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update Product Failed!"); ;
                    }
                }
                else
                {
                    result.AddError(StatusCode.NotFound, $"Product with id: {productId} not found!"); ;
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
