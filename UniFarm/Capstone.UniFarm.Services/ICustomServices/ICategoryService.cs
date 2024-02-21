using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Domain.Specifications;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface ICategoryService
    {

        Task<OperationResult<List<CategoryResponse>>> GetAllCategories();
        Task<OperationResult<CategoryResponse>> GetCategoryById(Guid companyId);
        Task<OperationResult<CategoryResponse>> CreateCategory(CategoryRequest companyRequest);
        Task<OperationResult<bool>> DeleteCategory(Guid id);
        Task<OperationResult<CategoryResponse>> UpdateCategory(Guid Id, CategoryRequest companyRequest);
        Task<OperationResult<Pagination<CategoryResponse>>> GetCategoryPaginationAsync(int pageIndex = 0, int pageSize = 10);
        Task<Guid> GetCategoriesCountAsync(ISpecifications<Category> specifications);
    }
}
