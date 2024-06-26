﻿using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Domain.Specifications;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface ICategoryService
    {
        Task<OperationResult<List<CategoryResponse>>> GetAllCategories();
        Task<OperationResult<List<CategoryResponseForCustomer>>> GetAllCategoriesForCustomer();
        Task<OperationResult<CategoryResponse>> GetCategoryById(Guid categoryId);
        Task<OperationResult<bool>> CreateCategory(CategoryRequest categoryRequest);
        Task<OperationResult<bool>> DeleteCategory(Guid categoryId);
        Task<OperationResult<bool>> UpdateCategory(Guid categoryId, CategoryRequestUpdate categoryRequestUpdate);
        Task<int> AdjustDisplayIndexAsync(Guid categoryId, int newDisplayIndex);
    }
}
