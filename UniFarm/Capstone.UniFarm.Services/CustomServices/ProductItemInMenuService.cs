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
    public class ProductItemInMenuService : IProductItemInMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductItemInMenuService> _logger;
        private readonly IMapper _mapper;

        public ProductItemInMenuService(IUnitOfWork unitOfWork, ILogger<ProductItemInMenuService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> AddProductItemToMenu(Guid menuId, ProductItemInMenuRequest productItemInMenuRequest)
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
                var existingProductItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemInMenuRequest.ProductItemId);
                if (existingProductItem == null)
                {
                    result.AddError(StatusCode.NotFound, $"ProductItem with id: {productItemInMenuRequest.ProductItemId} not found!");
                    return result;
                }

                if (existingMenu != null && existingProductItem != null)
                {
                    if(existingMenu.FarmHubId != existingProductItem.FarmHubId)
                    {
                        result.AddError(StatusCode.NotFound, $"Menu and ProductItem must belong to the same FarmHub!");
                        return result;
                    }
                    else
                    {
                        var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuId(menuId);
                        if (productItemsInMenu.Any(p => p.ProductItemId == productItemInMenuRequest.ProductItemId))
                        {
                            result.AddError(StatusCode.BadRequest, $"Product Item have Id: {productItemInMenuRequest.ProductItemId} already exists in the menu!");
                            return result;
                        }

                        productItemInMenuRequest.ProductItemId = existingProductItem.Id;
                        var productItemInMenu = _mapper.Map<ProductItemInMenu>(productItemInMenuRequest);
                        productItemInMenu.MenuId = menuId;
                        productItemInMenu.Status = "Pending";
                        await _unitOfWork.ProductItemInMenuRepository.AddAsync(productItemInMenu);
                        var checkResult = _unitOfWork.Save();
                        if (checkResult > 0)
                        {
                            result.AddResponseStatusCode(StatusCode.Created, "Add Product Item to Menu Success!", true);
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
                var listProductItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuId(menuId);
                var productItemsInMenuIsActive = listProductItemsInMenu.Where(pi => pi.Status != "Inactive").ToList();
                var listProductItemsInMenuResponse = _mapper.Map<List<ProductItemInMenuResponse>>(productItemsInMenuIsActive);

                if (listProductItemsInMenuResponse == null || !listProductItemsInMenuResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Product Items In Menu with Menu Id {menuId} is Empty!", listProductItemsInMenuResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Product Items In Menu Done.", listProductItemsInMenuResponse);
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
                var existingProductItemInMenu = await _unitOfWork.ProductItemInMenuRepository.GetByIdAsync(productItemInMenuId);
                if (existingProductItemInMenu != null)
                {
                    existingProductItemInMenu.Status = "Inactive";
                    _unitOfWork.ProductItemInMenuRepository.Update(existingProductItemInMenu);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Product Item In Menu have Id: {productItemInMenuId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Product Item In Menu Failed!"); ;
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Product Item In Menu have Id: {productItemInMenuId}. Delete Faild!.", false);
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
