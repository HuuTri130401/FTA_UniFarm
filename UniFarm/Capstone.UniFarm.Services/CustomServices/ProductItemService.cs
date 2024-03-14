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
    public class ProductItemService : IProductItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductItemService> _logger;
        private readonly IMapper _mapper;

        public ProductItemService(IUnitOfWork unitOfWork, ILogger<ProductItemService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> CreateProductItemForProduct(Guid productId, Guid farmHubAccountId, ProductItemRequest productItemRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (existingProduct != null)
                {
                    var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
                    if (accountRoleInfor != null)
                    {
                        var productItem = _mapper.Map<ProductItem>(productItemRequest);
                        productItem.ProductId = productId;
                        productItem.Status = "Active";
                        productItem.CreatedAt = DateTime.Now;
                        productItem.FarmHubId = (Guid)accountRoleInfor.FarmHubId;
                        if (productItemRequest.Quantity > 0)
                        {
                            productItem.OutOfStock = false; //con hang
                        }
                        else
                        {
                            productItem.OutOfStock = true;
                        }
                        await _unitOfWork.ProductItemRepository.AddAsync(productItem);
                        var checkResult = _unitOfWork.Save();
                        if (checkResult > 0)
                        {
                            result.AddResponseStatusCode(StatusCode.Created, "Add Product Item for Product Success!", true);
                        }
                        else
                        {
                            result.AddError(StatusCode.BadRequest, "Add Product Item for Product Failed!");
                        }
                    }
                }
                else
                {
                    result.AddError(StatusCode.NotFound, $"Not Found Product with Id: ${productId}!");
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteProductItem(Guid productItemId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemId);
                if (existingProductItem != null)
                {
                    existingProductItem.Status = "Inactive";
                    _unitOfWork.ProductItemRepository.Update(existingProductItem);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Product Item have Id: {productItemId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Product Item Failed!"); ;
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Product Item have Id: {productItemId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemResponse>>> GetAllProductItemsByProductId(Guid productId)
        {
            var result = new OperationResult<List<ProductItemResponse>>();
            try
            {
                var listProductItems = await _unitOfWork.ProductItemRepository.GetAllProductItemByProductId(productId);
                var activeProductItems = listProductItems.Where(pi => pi.Status == "Available").ToList();
                var listProductItemsResponse = _mapper.Map<List<ProductItemResponse>>(activeProductItems);

                if (listProductItemsResponse == null || !listProductItemsResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Product Items with Product Id {productId} is Empty!", listProductItemsResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items Done.", listProductItemsResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProductItemsByProductId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemResponse>>> GetAllProductItemsByFarmHubAccountId(Guid farmHubAccountId)
        {
            var result = new OperationResult<List<ProductItemResponse>>();
            try
            {
                var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
                if (accountRoleInfor != null)
                {
                    var farmHubId = accountRoleInfor.FarmHubId;
                    var listProductItems = await _unitOfWork.ProductItemRepository.GetAllProductItemByFarmHubId((Guid)farmHubId);
                    var productItems = listProductItems.Where(pi => pi.Status != "Inactive").ToList();
                    var listProductItemsResponse = _mapper.Map<List<ProductItemResponse>>(productItems);

                    if (listProductItemsResponse == null || !listProductItemsResponse.Any())
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"List Product Items in this FarmHub is Empty!", listProductItemsResponse);
                        return result;
                    }
                    result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items In FarmHub Done.", listProductItemsResponse);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProductItemsByFarmhubId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ProductItemResponse>> GetProductItemById(Guid productItemId)
        {
            var result = new OperationResult<ProductItemResponse>();
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetProductItemByIdAsync(productItemId);
                if (productItem == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Product Item with Id: {productItemId}");
                    return result;
                }
                else if (productItem.Status == "Available")
                {
                    var productItemResponse = _mapper.Map<ProductItemResponse>(productItem);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get Product Item by Id: {productItemId} Success!", productItemResponse);
                    return result;
                }
                result.AddError(StatusCode.NotFound, $"Can't found Product Item with Id: {productItemId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetProductItemById service method for productItem ID: {productItemId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateProductItem(Guid productItemId, ProductItemRequestUpdate productItemRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemId);

                if (existingProductItem != null)
                {
                    bool isAnyFieldUpdated = false;
                    if (productItemRequestUpdate.ProductId != null)
                    {
                        var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync((Guid)productItemRequestUpdate.ProductId);
                        if (existingProduct == null)
                        {
                            result.AddError(StatusCode.NotFound, $"Product with id: {productItemRequestUpdate.ProductId} not found!"); ;
                            return result;
                        }
                        existingProductItem.ProductId = (Guid)productItemRequestUpdate.ProductId;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.Title != null)
                    {
                        existingProductItem.Title = productItemRequestUpdate.Title;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.Description != null)
                    {
                        existingProductItem.Description = productItemRequestUpdate.Description;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.ProductOrigin != null)
                    {
                        existingProductItem.ProductOrigin = productItemRequestUpdate.ProductOrigin;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.SpecialTag != null)
                    {
                        existingProductItem.SpecialTag = productItemRequestUpdate.SpecialTag;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.StorageType != null)
                    {
                        existingProductItem.StorageType = productItemRequestUpdate.StorageType;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.Price != null)
                    {
                        existingProductItem.Price = (decimal)productItemRequestUpdate.Price;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.Quantity != null)
                    {
                        existingProductItem.Quantity = productItemRequestUpdate.Quantity;
                        if (productItemRequestUpdate.Quantity > 0)
                        {
                            existingProductItem.OutOfStock = false; //con hang
                        }
                        else
                        {
                            existingProductItem.OutOfStock = true;
                        }
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.MinOrder != null)
                    {
                        existingProductItem.MinOrder = productItemRequestUpdate.MinOrder;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.Unit != null)
                    {
                        existingProductItem.Unit = productItemRequestUpdate.Unit;
                        isAnyFieldUpdated = true;
                    }
                    if (productItemRequestUpdate.Status != null && (productItemRequestUpdate.Status == "Active"
                        || productItemRequestUpdate.Status == "Inactive"
                        || productItemRequestUpdate.Status == "Available"))
                    {
                        existingProductItem.Status = productItemRequestUpdate.Status;
                        isAnyFieldUpdated = true;
                    }

                    if (isAnyFieldUpdated)
                    {
                        existingProductItem.UpdatedAt = DateTime.Now;
                    }

                    _unitOfWork.ProductItemRepository.Update(existingProductItem);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update ProductItem have Id: {productItemId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update ProductItem Failed!");
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
