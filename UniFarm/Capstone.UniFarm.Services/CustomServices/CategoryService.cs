using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.ICustomServices;
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

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateCategory(Category category)
        {
            await _unitOfWork.CategoryRepository.Add(category);
            var result = _unitOfWork.Save();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCategory(int categoryId)
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

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var listCategories = await _unitOfWork.CategoryRepository.GetAll();
            return listCategories;
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(categoryId);
            if (category != null)
            {
                return category;
            }
            return null;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _unitOfWork.CategoryRepository.Update(category);
            var result = _unitOfWork.Save();
            if (result > 0)
            {
                return true;
            }
            return false;
        }
    }
}
