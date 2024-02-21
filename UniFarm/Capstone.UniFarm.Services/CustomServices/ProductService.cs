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

        public Task<OperationResult<ProductResponse>> CreateProduct(ProductRequest productRequest)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> DeleteProduct(Guid id)
        {
            throw new NotImplementedException();
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

        public Task<OperationResult<ProductResponse>> UpdateProduct(Guid Id, ProductRequest productRequest)
        {
            throw new NotImplementedException();
        }
    }
}
