using AutoMapper;
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
                    result.AddResponseStatusCode(StatusCode.Ok, "List Categories is Empty!", listCategoriesResponse);
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
                else
                {
                    result.AddResponseStatusCode(StatusCode.NotFound, $"Can't find Category have Id: {categoryId}. Delete Faild!.", false);
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
                category.Status = "Active";
                category.CreatedAt = DateTime.UtcNow;
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
                    bool isAnyFieldUpdated = false;
                    if (categoryRequestUpdate.Name != null)
                    {
                        existingCategory.Name = categoryRequestUpdate.Name;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.Description != null)
                    {
                        existingCategory.Description = categoryRequestUpdate.Description;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.Image != null)
                    {
                        existingCategory.Image = categoryRequestUpdate.Image;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.Code != null)
                    {
                        existingCategory.Code = categoryRequestUpdate.Code;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.DisplayIndex != null)
                    {
                        existingCategory.DisplayIndex = categoryRequestUpdate.DisplayIndex;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.SystemPrice != null)
                    {
                        existingCategory.SystemPrice = categoryRequestUpdate.SystemPrice;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.MinSystemPrice != null)
                    {
                        existingCategory.MinSystemPrice = categoryRequestUpdate.MinSystemPrice;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.MaxSystemPrice != null)
                    {
                        existingCategory.MaxSystemPrice = categoryRequestUpdate.MaxSystemPrice;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.Margin != null)
                    {
                        existingCategory.Margin = categoryRequestUpdate.Margin;
                        isAnyFieldUpdated = true;
                    }
                    if (categoryRequestUpdate.Status != null && (categoryRequestUpdate.Status == "Active" || categoryRequestUpdate.Status == "InActive"))
                    {
                        existingCategory.Status = categoryRequestUpdate.Status;
                        isAnyFieldUpdated = true;
                    }

                    if (isAnyFieldUpdated)
                    {
                        existingCategory.UpdatedAt = DateTime.Now;
                    }
                    _unitOfWork.CategoryRepository.Update(existingCategory);

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
