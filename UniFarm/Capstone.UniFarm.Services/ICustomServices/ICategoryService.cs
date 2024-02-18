using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface ICategoryService
    {
        Task<bool> CreateCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(int categoryId);
        Task<IReadOnlyList<Category>> GetCategoriesAsync(ISpecifications<Category> specifications);
        Task<int> GetCategoriesCountAsync(ISpecifications<Category> specifications);
    }
}
