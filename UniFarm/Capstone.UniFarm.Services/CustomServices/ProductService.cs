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
                var product = _mapper.Map<Product>(productRequest);
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
                    existingProduct.Status = "InActive";
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
                result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Product have Id: {productId}. Delete Faild!.", false);
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
                var listProductsResponse = _mapper.Map<List<ProductResponse>>(listProducts);

                if (listProductsResponse == null || !listProductsResponse.Any())
                {
                    result.AddError(StatusCode.NotFound, "List Products is Empty!");
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
                }
                var productResponse = _mapper.Map<ProductResponse>(product);
                result.AddResponseStatusCode(StatusCode.Ok, $"Get Product by Id: {productId} Success!", productResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetProductById service method for category ID: {productId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateProduct(Guid productId, ProductRequest productRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (existingProduct != null)
                {
                    existingProduct = _mapper.Map<Product>(productRequest);
                    existingProduct.Id = productId;
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
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
