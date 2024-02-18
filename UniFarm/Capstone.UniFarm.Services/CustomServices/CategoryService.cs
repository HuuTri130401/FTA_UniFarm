using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.Category.Request;
using Capstone.UniFarm.Services.ViewModels.Category.Response;
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

        public Task<OperationResult<CategoryResponse>> CreateCategory(CategoryRequest companyRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var listCategories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return listCategories;
        }

        public Task<OperationResult<CategoryResponse>> GetCategory(Guid companyId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Pagination<CategoryResponse>>> GetCategoryPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<CategoryResponse>> UpdateCategory(Guid Id, CategoryRequest companyRequest)
        {
            throw new NotImplementedException();
        }

        Task<OperationResult<bool>> ICategoryService.DeleteCategory(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<OperationResult<IEnumerable<Category>>> ICategoryService.GetAllCategories()
        {
            throw new NotImplementedException();
        }
    }
}
