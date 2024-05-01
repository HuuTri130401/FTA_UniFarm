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
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MenuService> _logger;
        private readonly IMapper _mapper;

        public MenuService(IUnitOfWork unitOfWork, ILogger<MenuService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<bool>> AssignMenuToBusinessDay(Guid businessDayId, Guid menuId)
        {
            var result = new OperationResult<bool>();

            try
            {
                var businessDay = await _unitOfWork.BusinessDayRepository.GetByIdAsync(businessDayId);
                if (businessDay == null)
                {
                    result.AddError(StatusCode.NotFound, $"BusinessDay with Id {businessDayId} not found.");
                    return result;
                }
                else if(businessDay.Status != "Active")
                {
                    result.AddError(StatusCode.BadRequest, $"BusinessDay have Id: {businessDayId} stopped selling.");
                    return result;
                }
                        
                var menu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                if (menu == null)
                {
                    result.AddError(StatusCode.NotFound, $"Menu with Id {menuId} not found.");
                    return result;
                }

                DateTime currentDate = DateTime.UtcNow.AddHours(7);

                // Check currentDate beetween [RegiterDay - EndOfRegister] 
                if (currentDate < businessDay.RegiterDay || currentDate > businessDay.EndOfRegister)
                {
                    result.AddError(StatusCode.BadRequest, "Menu cannot be assigned to BusinessDay outside the registration period.");
                    return result;
                }

                if(menu.BusinessDayId != null)
                {
                    var today = DateTime.UtcNow.AddHours(7).Date;
                    var opendayIsTodayAndIsActiveBusinessDay = await _unitOfWork.BusinessDayRepository
                        .GetOpendayIsToday(today);
                    if (opendayIsTodayAndIsActiveBusinessDay.Id == menu.BusinessDayId)
                    {
                        if(opendayIsTodayAndIsActiveBusinessDay.Status == "Active")
                        {
                            result.AddError(StatusCode.BadRequest, "Menu is selling Today, can not assign to other businessday!");
                            return result;
                        }
                    }
                }

                menu.BusinessDayId = businessDayId;
                menu.Status = "Active";
                _unitOfWork.MenuRepository.Update(menu);
                var checkResult = _unitOfWork.Save();

                if (checkResult > 0)
                {
                    var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuId(menuId);
                    foreach (var productItemInMenu in productItemsInMenu)
                    {
                        var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemInMenu.ProductItemId);
                        if (productItem != null)
                        {
                            productItem.Status = "Selling";
                            _unitOfWork.ProductItemRepository.Update(productItem);
                            var checkSaveStatusProductItem = _unitOfWork.Save();
                            if (checkSaveStatusProductItem > 0)
                            {
                                result.AddResponseStatusCode(StatusCode.Ok, "Menu assigned to BusinessDay successfully!", true);
                            }
                        }
                    }
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Failed to assign menu to BusinessDay.");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in AssignMenuToBusinessDay Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> CreateMenuForFarmHub(Guid farmHubAccountId, MenuRequest menuRequest)
        {
            var result = new OperationResult<bool>();

            try
            {
                var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
                if (accountRoleInfor != null && accountRoleInfor.FarmHubId != null)
                {
                    var existingMenuName = await _unitOfWork
                            .MenuRepository
                            .GetSingleOrDefaultMenuAsync(c => c.Name == menuRequest.Name 
                            && c.FarmHubId == (Guid)accountRoleInfor.FarmHubId
                            && c.Status != "Inactive");

                    if (existingMenuName != null)
                    {
                        result.AddError(StatusCode.BadRequest, $"Menu name: {existingMenuName.Name} of FarmHub has FarmHubId: {existingMenuName.FarmHubId} already exists!");
                        return result;
                    }
                    var menu = _mapper.Map<Menu>(menuRequest);
                    menu.FarmHubId = (Guid)accountRoleInfor.FarmHubId;
                    menu.Status = "Preparing";
                    menu.CreatedAt = DateTime.UtcNow.AddHours(7);
                    await _unitOfWork.MenuRepository.AddAsync(menu);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Menu for FarmHub Success!", true);
                    }
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Please Create Your FarmHub before Create Menu!");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in DeleteMenu Service Method");
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteMenu(Guid menuId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingMenu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                if (existingMenu != null && existingMenu.Status != "Inactive")
                {
                    existingMenu.Status = "Inactive";
                    existingMenu.BusinessDayId = null;
                    _unitOfWork.MenuRepository.Update(existingMenu);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        var productItemsInMenu = await _unitOfWork.ProductItemInMenuRepository.GetProductItemsByMenuId(menuId);
                        if (productItemsInMenu != null)
                        {
                            foreach (var productItemInMenu in productItemsInMenu)
                            {
                                productItemInMenu.Status = "Inactive";
                                _unitOfWork.ProductItemInMenuRepository.Update(productItemInMenu);
                                _unitOfWork.Save();
                                var productItem = await _unitOfWork.ProductItemRepository.GetByIdAsync(productItemInMenu.ProductItemId);
                                if (productItem != null)
                                {
                                    //check xem còn trong menu active nào không
                                    //nếu chỉ nằm trong menu inactive thì UnRegistered
                                    //nếu nằm trong ít nhất 1 menu active thì Registered
                                    //product item in menu sẽ là inactive theo menu id đó vì không bán nữa (Inactive tất cả productItemInMenu)
                                    //check product item belong to orther menu ?
                                    //hàm này mới chuyển sang registered chứ chưa phải unregistered nếu sản không có trong menu nào

                                    var otherMenusForProduct = await _unitOfWork
                                        .ProductItemInMenuRepository
                                        .FindStatusProductItem(p => p.ProductItemId == productItem.Id
                                                                    && p.Status != "Inactive");
                                    var newStatus = !otherMenusForProduct.Any() ? "Unregistered" : "Registered";
                                    productItem.Status = newStatus;
                                    _unitOfWork.ProductItemRepository.Update(productItem);
                                    var checkSaveStatusProductItem = _unitOfWork.Save();
                                    if (checkSaveStatusProductItem > 0)
                                    {
                                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Menu have Id: {menuId} Success.", true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Menu Failed!"); 
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Menu have Id: {menuId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in DeleteMenu Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<MenuResponse>>> GetAllMenusByFarmHubAccountId(Guid farmHubAccountId)
        {
            var result = new OperationResult<List<MenuResponse>>();
            try
            {
                var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);
                if (accountRoleInfor != null && accountRoleInfor.FarmHubId != null)
                {
                    var farmHubId = accountRoleInfor.FarmHubId;
                    var listMenus = await _unitOfWork.MenuRepository.GetAllMenuByFarmHubIdAsync((Guid)farmHubId);
                    var menusIsActive = listMenus.Where(menu => menu.Status != "Inactive");
                    var listMenusResponse = _mapper.Map<List<MenuResponse>>(menusIsActive);

                    if (listMenusResponse == null || !listMenusResponse.Any())
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"List Menus in this FarmHub is Empty!", listMenusResponse);
                        return result;
                    }
                    result.AddResponseStatusCode(StatusCode.Ok, "Get List Menus In FarmHub Done.", listMenusResponse);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Please Create Your FarmHub before Get List Menus!");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllMenusByFarmHubAccountId Service Method");
                throw;
            }
        }

        public async Task<OperationResult<MenuResponse>> GetMenuById(Guid menuId)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                if (menu == null || menu.Status == "Inactive")
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Menu with Id: {menuId}");
                    return result;
                }
                var menuResponse = _mapper.Map<MenuResponse>(menu);
                result.AddResponseStatusCode(StatusCode.Ok, $"Get Menu by Id: {menuId} Success!", menuResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetMenuById service method for Menu ID: {menuId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateMenu(Guid farmHubAccountId, Guid menuId, MenuRequestUpdate menuRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingMenu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                var accountRoleInfor = await _unitOfWork.AccountRoleRepository.GetAccountRoleByAccountIdAsync(farmHubAccountId);

                if (existingMenu != null)
                {
                    bool isAnyFieldUpdated = false;
                    if (menuRequestUpdate.Name != null)
                    {
                        var existingMenuName = await _unitOfWork
                                .MenuRepository
                                .GetSingleOrDefaultMenuAsync(c => c.Name == menuRequestUpdate.Name 
                                && c.FarmHubId == (Guid)accountRoleInfor.FarmHubId
                                && c.Status != "Inactive");

                        if (existingMenuName != null)
                        {
                            result.AddError(StatusCode.BadRequest, $"Menu name: {existingMenuName.Name} of FarmHub has FarmHubId: {existingMenuName.FarmHubId} already exists!");
                            return result;
                        }
                        existingMenu.Name = menuRequestUpdate.Name;
                        isAnyFieldUpdated = true;
                    }
                    if (menuRequestUpdate.Tag != null)
                    {
                        existingMenu.Tag = menuRequestUpdate.Tag;
                        isAnyFieldUpdated = true;
                    }

                    if (isAnyFieldUpdated)
                    {
                        existingMenu.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    }

                    _unitOfWork.MenuRepository.Update(existingMenu);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update Menu have Id: {menuId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update Menu Failed!");
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdateMenu Service Method");
                throw;
            }
        }
    }
}
