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

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Task<OperationResult<IEnumerable<Category>>> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<CategoryResponse>> GetCategory(Guid companyId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<CategoryResponse>> CreateCategory(CategoryRequest companyRequest)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> DeleteCategory(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<CategoryResponse>> UpdateCategory(Guid Id, CategoryRequest companyRequest)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Pagination<CategoryResponse>>> GetCategoryPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetCategoriesCountAsync(ISpecifications<Category> specifications)
        {
            throw new NotImplementedException();
        }
    }

}
