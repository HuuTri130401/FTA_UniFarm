using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.RequestFeatures;
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
using static Capstone.UniFarm.Services.ViewModels.ModelResponses.AdminDashboardResponse;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class ProductItemService : IProductItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductItemService> _logger;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductItemService(IUnitOfWork unitOfWork, ILogger<ProductItemService> logger, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
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
                    if (accountRoleInfor != null && accountRoleInfor.FarmHubId != null)
                    {
                        var productItem = _mapper.Map<Domain.Models.ProductItem>(productItemRequest);
                        productItem.ProductId = productId;
                        productItem.Status = "Unregistered";
                        productItem.CreatedAt = DateTime.UtcNow.AddHours(7);
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
                        _unitOfWork.Save();

                        var productImage = new ProductImage();
                        int displayIndex = 1;
                        foreach (var imageRequest in productItemRequest.Images)
                        {
                            productImage.Id = Guid.NewGuid();
                            productImage.DisplayIndex = displayIndex++;
                            productImage.Caption = $"{productItemRequest.Title}: {productImage.DisplayIndex}";
                            var imageUrl = await _cloudinaryService.UploadImageAsync(imageRequest);
                            productImage.ImageUrl = imageUrl;
                            productImage.ProductItemId = productItem.Id;
                            productImage.Status = "Active";
                            await _unitOfWork.ProductImageRepository.AddAsync(productImage);
                            _unitOfWork.Save();
                        }
                        result.AddResponseStatusCode(StatusCode.Created, "Add Product Item for Product Success!", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Please Create Your FarmHub before Create Product Item!");
                    }
                }
                else
                {
                    result.AddError(StatusCode.NotFound, $"Not Found Product with Id: ${productId}!");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in CreateProductItemForProduct service method");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in DeleteProductItem service method");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemInMenuResponseForCustomer>>> CustomerGetAllProductItemsByProductId(Guid productId)
        {
            var result = new OperationResult<List<ProductItemInMenuResponseForCustomer>>();
            try
            {
                var today = DateTime.UtcNow.AddHours(7).Date;
                var currentBusinessDay = await _unitOfWork.BusinessDayRepository.GetOpendayIsToday(today);
                if (currentBusinessDay == null)
                {
                    result.AddError(StatusCode.Ok, $"Not open business day for sale today!");
                    return result;
                }

                var menuForToday = await _unitOfWork.MenuRepository.GetAllMenuInCurrentBusinessDay(currentBusinessDay.Id);
                var productItems = new List<ProductItemInMenuResponseForCustomer>();

                foreach (var menu in menuForToday)
                {
                    var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemInMenuByProductIdCustomer(menu.Id);
                    var productItemsResponse = _mapper.Map<List<ProductItemInMenuResponseForCustomer>>(productItemsInMenu);
                    foreach (var pim in productItemsResponse)
                    {
                        productItems.Add(pim);
                    }
                }
                if(productItems == null || !productItems.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Product Items with Product Id {productId} is Empty!", null);
                    return result;
                }
                var listProductItemsByProductId = productItems.Where(pi => pi.ProductItem.ProductId == productId).ToList();
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items Done.", listProductItemsByProductId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in CustomerGetAllProductItemsByProductId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemResponse>>> GetAllProductItemsByProductId(Guid productId)
        {
            var result = new OperationResult<List<ProductItemResponse>>();
            try
            {
                var listProductItems = await _unitOfWork.ProductItemRepository.GetAllProductItemByProductId(productId);
                var activeProductItems = listProductItems.Where(pi => pi.Status == "Selling").ToList();
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
                if (accountRoleInfor != null && accountRoleInfor.FarmHubId != null)
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
                else
                {
                    result.AddError(StatusCode.BadRequest, "Please Create Your FarmHub before Get List Product Items!");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProductItemsByFarmHubAccountId Service Method");
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
                else if (productItem.Status != "Inactive")
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
        
        public async Task<OperationResult<ProductItemResponse>> CustomerGetProductItemById(Guid productItemId, Guid menuId)
        {
            var result = new OperationResult<ProductItemResponse>();
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.CustomerGetProductItemById(productItemId, menuId);
                if (productItem == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Product Item with Id: {productItemId}");
                    return result;
                }
                else if (productItem.Status != "Inactive")
                {
                    //var tmp = new ProductItemResponse
                    //{
                    //    Price = (decimal)productItem.ProductItemInMenus.FirstOrDefault(pim => pim.MenuId == menuId).SalePrice,
                    //    Quantity = (double)productItem.ProductItemInMenus.FirstOrDefault(pim => pim.MenuId == menuId).Quantity,
                    //};
                    productItem.Price = (decimal)productItem.ProductItemInMenus.FirstOrDefault(pim => pim.MenuId == menuId).SalePrice;
                    productItem.Quantity = (double)productItem.ProductItemInMenus.FirstOrDefault(pim => pim.MenuId == menuId).Quantity;
                    var productItemResponse = _mapper.Map<ProductItemResponse>(productItem);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get Product Item by Id: {productItemId} Success!", productItemResponse);
                    return result;
                }
                result.AddError(StatusCode.NotFound, $"Can't found Product Item with Id: {productItemId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in CustomerGetProductItemById service method for productItem ID: {productItemId}");
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
            catch (Exception ex) { 
                _logger.LogError(ex, $"Error occurred in UpdateProductItem service method"); 
                throw; 
            }
        }

        public async Task<OperationResult<List<ProductItemInMenuResponseForCustomer>>> SearchProductItems(ProductItemInMenuParameters productItemInMenuParameters)
        {
            var result = new OperationResult<List<ProductItemInMenuResponseForCustomer>>();
            try
            {
                var today = DateTime.UtcNow.AddHours(7).Date;
                var currentBusinessDay = await _unitOfWork.BusinessDayRepository.GetOpendayIsToday(today);
                if (currentBusinessDay == null)
                {
                    result.AddError(StatusCode.Ok, $"Not open business day for sale today!");
                    return result;
                }

                var menuForToday = await _unitOfWork.MenuRepository.GetAllMenuInCurrentBusinessDay(currentBusinessDay.Id);
                var productItems = new List<ProductItemInMenuResponseForCustomer>();

                foreach (var menu in menuForToday)
                {
                    var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuIdForCustomer(productItemInMenuParameters, menu.Id);
                    var productItemsResponse = _mapper.Map<List<ProductItemInMenuResponseForCustomer>>(productItemsInMenu);
                    foreach (var pim in productItemsResponse)
                    {
                        pim.BusinessDayId = currentBusinessDay.Id;
                        productItems.Add(pim);
                    }
                }

                if (productItems == null || !productItems.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Product Items is Empty!", productItems);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items Done.", productItems);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in SearchProductItems Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemInMenuResponseForCustomer>>> GetAllProductItems(ProductItemInMenuParameters productItemInMenuParameters)
        {
            var result = new OperationResult<List<ProductItemInMenuResponseForCustomer>>();
            try
            {
                var today = DateTime.UtcNow.AddHours(7).Date;
                var currentBusinessDay = await _unitOfWork.BusinessDayRepository.GetOpendayIsToday(today);
                if (currentBusinessDay == null)
                {
                    result.AddError(StatusCode.Ok, $"Not open business day for sale today!");
                    return result;
                }

                var menuForToday = await _unitOfWork.MenuRepository.GetAllMenuInCurrentBusinessDay(currentBusinessDay.Id);
                var productItems = new List<ProductItemInMenuResponseForCustomer>();

                foreach (var menu in menuForToday)
                {
                    var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuIdForCustomer(productItemInMenuParameters, menu.Id);
                    var productItemsResponse = _mapper.Map<List<ProductItemInMenuResponseForCustomer>>(productItemsInMenu);
                    
                    foreach(var pim in productItemsResponse)
                    {
                        pim.BusinessDayId = currentBusinessDay.Id;
                        productItems.Add(pim);
                    }
                }

                if (productItems == null || !productItems.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Product Items is Empty!", productItems);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items Done.", productItems);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllProductItems Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemResponse>>> FarmHubGetAllProductItemsByProductId(Guid farmHubId, Guid productId)
        {
            var result = new OperationResult<List<ProductItemResponse>>();
            try
            {
                var listProductItems = await _unitOfWork.ProductItemRepository.FarmHubGetAllProductItemByProductId(farmHubId, productId);
                var listProductItemsResponse = _mapper.Map<List<ProductItemResponse>>(listProductItems);

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
                _logger.LogError(ex, $"Error occurred in FarmHubGetAllProductItemsByProductId Service Method");
                throw;
            }
        }
    }
}
