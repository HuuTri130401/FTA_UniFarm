﻿using AutoMapper;
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

        public async Task<OperationResult<bool>> CreateMenuForFarmHub(Guid farmHubId, MenuRequest menuRequest)
        {
            var result = new OperationResult<bool>();

            try
            {
                var existingFarmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync(farmHubId);
                if (existingFarmHub != null)
                {
                    var menu = _mapper.Map<Menu>(menuRequest);
                    menu.FarmHubId = farmHubId;
                    menu.Status = "Active";
                    menu.CreatedAt = DateTime.Now;
                    await _unitOfWork.MenuRepository.AddAsync(menu);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Created, "Add Menu for FarmHub Success!", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Add Menu for FarmHub Failed!"); ;
                    }
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Menu for FarmHub Failed!"); ;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteMenu(Guid menuId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingMenu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                if (existingMenu != null)
                {
                    existingMenu.Status = "InActive";
                    _unitOfWork.MenuRepository.Update(existingMenu);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Menu have Id: {menuId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Menu Failed!"); ;
                    }
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Menu have Id: {menuId}. Delete Faild!.", false);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<List<MenuResponse>>> GetAllMenusByFarmHubId(Guid farmHubId)
        {
            var result = new OperationResult<List<MenuResponse>>();
            try
            {
                var listMenus = await _unitOfWork.MenuRepository.GetAllMenuByFarmHubIdAsync(farmHubId);
                var listMenusResponse = _mapper.Map<List<MenuResponse>>(listMenus);

                if (listMenusResponse == null || !listMenusResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, $"List Menus with FarmHub Id {farmHubId} is Empty!", listMenusResponse);
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Menus Done.", listMenusResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllMenusByFarmHubId Service Method");
                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<OperationResult<MenuResponse>> GetMenuById(Guid menuId)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);
                if (menu == null)
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

        public async Task<OperationResult<bool>> UpdateMenu(Guid menuId, MenuRequestUpdate menuRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingMenu = await _unitOfWork.MenuRepository.GetByIdAsync(menuId);

                if (existingMenu != null)
                {
                    bool isAnyFieldUpdated = false;
                    if (menuRequestUpdate.Name != null)
                    {
                        existingMenu.Name = menuRequestUpdate.Name;
                        isAnyFieldUpdated = true;
                    }
                    if (menuRequestUpdate.Tag != null)
                    {
                        existingMenu.Tag = menuRequestUpdate.Tag;
                        isAnyFieldUpdated = true;
                    }
                    if (menuRequestUpdate.FarmHubId != null)
                    {
                        var existingFarmHub = await _unitOfWork.FarmHubRepository.GetByIdAsync((Guid)menuRequestUpdate.FarmHubId);
                        if (existingFarmHub == null)
                        {
                            result.AddError(StatusCode.BadRequest, "Menu must belong to a FarmHub to update!");
                            return result;
                        }
                        isAnyFieldUpdated = true;
                    }

                    if (menuRequestUpdate.Status != null && (menuRequestUpdate.Status == "Active" || menuRequestUpdate.Status == "InActive"))
                    {
                        existingMenu.Status = menuRequestUpdate.Status;
                        isAnyFieldUpdated = true;
                    }
                    if (isAnyFieldUpdated)
                    {
                        existingMenu.UpdatedAt = DateTime.Now;
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}