﻿using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Domain.Specifications;
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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<CategoryResponse>>> GetAllCategories()
        {
            var result = new OperationResult<List<CategoryResponse>>();
            try
            {
                var listCategories = await _unitOfWork.CategoryRepository.GetAllAsync();
                var listCategoriesResponse = _mapper.Map<List<CategoryResponse>>(listCategories);

                if (listCategoriesResponse == null || !listCategoriesResponse.Any())
                {
                    result.AddError(StatusCode.NotFound, "List Categories is Empty!");
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Categories Done.", listCategoriesResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllCategories Service Method");
                throw;
            }
        }

        public async Task<OperationResult<CategoryResponse>> GetCategoryById(Guid categoryId)
        {
            var result = new OperationResult<CategoryResponse>();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Category with Id: {categoryId}");
                    return result;
                }
                var categoryResponse = _mapper.Map<CategoryResponse>(category);
                result.AddResponseStatusCode(StatusCode.Ok, $"Get Category by Id: {categoryId} Success!", categoryResponse);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetCategoryById Service Method for category ID: {categoryId}");
                throw;
            }
        }

        public async Task<OperationResult<bool>> DeleteCategory(Guid categoryId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category != null)
                {
                    category.Status = "InActive";
                    _unitOfWork.CategoryRepository.Update(category);
                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.Ok, $"Delete Category have Id: {categoryId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Delete Category Failed!"); ;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<OperationResult<Pagination<CategoryResponse>>> GetCategoryPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetCategoriesCountAsync(ISpecifications<Category> specifications)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> CreateCategory(CategoryRequest categoryRequest)
        {
            var result = new OperationResult<bool>();
            try
            {
                var category = _mapper.Map<Category>(categoryRequest);
                await _unitOfWork.CategoryRepository.AddAsync(category);
                var checkResult = _unitOfWork.Save();
                if (checkResult > 0)
                {
                    result.AddResponseStatusCode(StatusCode.Created, "Add Category Success!", true);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, "Add Category Failed!"); ;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResult<bool>> UpdateCategory(Guid categoryId, CategoryRequestUpdate categoryRequestUpdate)
        {
            var result = new OperationResult<bool>();
            try
            {
                var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (existingCategory != null)
                {
                    var temporaryCategory = _mapper.Map<Category>(categoryRequestUpdate);
                    temporaryCategory.Id = categoryId;
                    temporaryCategory.CreatedAt = existingCategory.CreatedAt; 
                    _unitOfWork.CategoryRepository.Update(temporaryCategory);

                    var checkResult = _unitOfWork.Save();
                    if (checkResult > 0)
                    {
                        result.AddResponseStatusCode(StatusCode.NoContent, $"Update Category have Id: {categoryId} Success.", true);
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, "Update Category Failed!"); ;
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
