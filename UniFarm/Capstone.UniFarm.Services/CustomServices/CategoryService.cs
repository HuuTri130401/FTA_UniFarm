using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Domain.Specifications;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.ICustomServices;
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
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> CreateCategory(Category category)
        {
            try
            {
                await _unitOfWork.CategoryRepository.Add(category);
                var result = _unitOfWork.Save();
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            try
            {
                if (categoryId > 0)
                {
                    var category = await _unitOfWork.CategoryRepository.GetById(categoryId);
                    if (category != null)
                    {
                        category.Status = 0;
                        _unitOfWork.CategoryRepository.Update(category);
                        var result = _unitOfWork.Save();
                        if (result > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                var listCategories = await _unitOfWork.CategoryRepository.GetAll();
                if (listCategories != null)
                {
                    return listCategories;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            try
            {
                return await _unitOfWork.CategoryRepository.GetById(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetCategoryById service method for category ID: {categoryId}");
                throw;
            }
        }
        public async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                _unitOfWork.CategoryRepository.Update(category);
                var result = _unitOfWork.Save();
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IReadOnlyList<Category>> GetCategoriesAsync(ISpecifications<Category> specifications)
        {
            return await _unitOfWork.CategoryRepository.ListAsync(specifications);
        }

        public async Task<int> GetCategoriesCountAsync(ISpecifications<Category> specifications)
        {
            return await _unitOfWork.CategoryRepository.CountAsync(specifications);
        }
    }
}
