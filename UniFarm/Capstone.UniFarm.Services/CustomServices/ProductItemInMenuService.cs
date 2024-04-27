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
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Services.CustomServices
{
    public class ProductItemInMenuService : IProductItemInMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductItemInMenuService> _logger;
        private readonly IMapper _mapper;

        public ProductItemInMenuService(IUnitOfWork unitOfWork, ILogger<ProductItemInMenuService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> AddProductItemToMenu(Guid menuId,
            ProductItemInMenuRequest productItemInMenuRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingMenu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                if (existingMenu == null)
                {
                    result.AddError(StatusCode.NotFound, $"Menu with id: {menuId} not found!");
                    return result;
                }

                var existingProductItem = await _unitOfWork.ProductItemRepository
                    .GetByIdAsync(productItemInMenuRequest.ProductItemId);
                if (existingProductItem == null)
                {
                    result.AddError(StatusCode.NotFound,
                        $"ProductItem with id: {productItemInMenuRequest.ProductItemId} not found!");
                    return result;
                }

                if (existingMenu != null && existingProductItem != null)
                {
                    if (existingMenu.FarmHubId != existingProductItem.FarmHubId)
                    {
                        result.AddError(StatusCode.NotFound, $"Menu and ProductItem must belong to the same FarmHub!");
                        return result;
                    }
                    else
                    {
                        productItemInMenuRequest.ProductItemId = existingProductItem.Id;
                        var productItemInMenu = _mapper.Map<ProductItemInMenu>(productItemInMenuRequest);
                        productItemInMenu.MenuId = menuId;
                        productItemInMenu.Status = "Active";
                        await _unitOfWork.ProductItemInMenuRepository.AddAsync(productItemInMenu);
                        var checkResult = _unitOfWork.Save();
                        if (checkResult > 0)
                        {
                            existingProductItem.Status = "Registered";
                            _unitOfWork.ProductItemRepository.Update(existingProductItem);
                            var checkUpdateStatusProductItem = _unitOfWork.Save();
                            if (checkUpdateStatusProductItem > 0)
                            {
                                result.AddResponseStatusCode(StatusCode.Created, "Add Product Item to Menu Success!",
                                    true);
                            }
                        }
                        else
                        {
                            result.AddError(StatusCode.BadRequest, "Add Product Item to Menu Failed!");
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<ProductItemInMenuResponse>>> GetProductItemsInMenuByMenuId(Guid menuId)
        {
            var result = new OperationResult<List<ProductItemInMenuResponse>>();
            try
            {
                var listProductItemsInMenu =
                    await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuId(menuId);
                var productItemsInMenuIsActive = listProductItemsInMenu.Where(pi => pi.Status != "Inactive").ToList();
                var listProductItemsInMenuResponse =
                    _mapper.Map<List<ProductItemInMenuResponse>>(productItemsInMenuIsActive);

                if (listProductItemsInMenuResponse == null || !listProductItemsInMenuResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok,
                        $"List Product Items In Menu with Menu Id {menuId} is Empty!", listProductItemsInMenuResponse);
                    return result;
                }

                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items In Menu Done.",
                    listProductItemsInMenuResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetProductItemsInMenuByMenuId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> RemoveProductItemFromMenu(Guid productItemInMenuId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingProductItemInMenu =
                    await _unitOfWork.ProductItemInMenuRepository.GetByIdAsync(productItemInMenuId);

                if (existingProductItemInMenu != null)
                {
                    existingProductItemInMenu.Status = "Inactive";
                    _unitOfWork.ProductItemInMenuRepository.Update(existingProductItemInMenu);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        var productItemId = existingProductItemInMenu.ProductItemId;
                        // check product item belong to orther menu ?
                        var otherMenusForProduct = await _unitOfWork
                            .ProductItemInMenuRepository
                            .FindStatusProductItem(p => p.ProductItemId == productItemId
                                                        && p.Status != "Inactive");
                        var newStatus = !otherMenusForProduct.Any() ? "Unregistered" : "Registered";
                        var existingProductItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemId);
                        existingProductItem.Status = newStatus;
                        _unitOfWork.ProductItemRepository.Update(existingProductItem);
                        var checkUpdateStatusProductItem = _unitOfWork.Save();
                        if (checkUpdateStatusProductItem > 0)
                        {
                            result.AddResponseStatusCode(StatusCode.Ok,
                                $"Delete Product Item In Menu have Id: {productItemInMenuId} Success.", true);
                        }
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Product Item In Menu Failed!");
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound,
                        $"Can't find Product Item In Menu have Id: {productItemInMenuId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<IEnumerable<ProductItemSellingPercentRatio>>>
            GetProductItemSellingPercentRatio(Guid farmHubId, Guid businessDayId)
        {
            var result = new OperationResult<IEnumerable<ProductItemSellingPercentRatio>>();
            try
            {
                var menu = _unitOfWork.MenuRepository
                    .FilterByExpression(x => x.FarmHubId == farmHubId && x.BusinessDayId == businessDayId)
                    .FirstOrDefault();
                if (menu == null)
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.Message = $"Menu with FarmHub Id {farmHubId} and BusinessDay Id {businessDayId} not found!";
                    result.AddError(StatusCode.NotFound,
                        $"Menu with FarmHub Id {farmHubId} and BusinessDay Id {businessDayId} not found!");
                    result.IsError = true;
                    return result;
                }

                var productItemsInMenu =
                    await _unitOfWork.ProductItemInMenuRepository
                        .GetAllWithoutPaging(null, null, x => x.MenuId == menu.Id).ToListAsync();
                var productItemsInMenuResponse = new List<ProductItemSellingPercentRatio>();
                if (!productItemsInMenu.Any())
                {
                    result.StatusCode = StatusCode.NotFound;
                    result.Message = $"Product Items In Menu with Menu Id {menu.Id} not found!";
                    result.AddError(StatusCode.NotFound, $"Product Items In Menu with Menu Id {menu.Id} not found!");
                    result.IsError = true;
                    return result;
                }

                foreach (var productItemInMenu in productItemsInMenu)
                {
                    var productItem =
                        await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemInMenu.ProductItemId);
                    if (productItem == null)
                    {
                        result.StatusCode = StatusCode.NotFound;
                        result.Message = $"Product Item with Id {productItemInMenu.ProductItemId} not found!";
                        result.AddError(StatusCode.NotFound,
                            $"Product Item with Id {productItemInMenu.ProductItemId} not found!");
                        result.IsError = true;
                        return result;
                    }

                    var soldPercent = productItemInMenu.Quantity == 0 || productItemInMenu.Sold == 0
                        ? 0
                        : productItemInMenu.Sold / productItemInMenu.Quantity * 100;
                    if (soldPercent > 0)
                    {
                        soldPercent = Math.Round(soldPercent ?? 0, 2);
                    }
                    var productItemSellingPercentRatio = new ProductItemSellingPercentRatio
                    {
                        ProductItemId = productItem.Id,
                        Title = productItem.Title,
                        SalePrice = productItemInMenu.SalePrice,
                        Quantity = productItemInMenu.Quantity,
                        Sold = productItemInMenu.Sold,
                        Status = productItemInMenu.Status!,
                        SoldPercent = soldPercent
                    };
                    productItemsInMenuResponse.Add(productItemSellingPercentRatio);
                }

                result.StatusCode = StatusCode.Ok;
                result.Message = "Get Product Item Selling Percent Ratio Success!";
                result.Payload = productItemsInMenuResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetProductItemSellingPercentRatio Service Method");
                result.IsError = true;
                result.StatusCode = StatusCode.ServerError;
                throw;
            }

            return result;
        }
    }
}