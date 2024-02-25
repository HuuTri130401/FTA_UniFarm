using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Capstone.UniFarm.Domain.Specifications;
namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface ICategoryService
    {

        Task<OperationResult<IEnumerable<Category>>> GetAllCategories();
        Task<OperationResult<CategoryResponse>> GetCategory(Guid objectId);
        Task<OperationResult<CategoryResponse>> CreateCategory(CategoryRequest objectRequest);
        Task<OperationResult<bool>> DeleteCategory(Guid id);
        Task<OperationResult<CategoryResponse>> UpdateCategory(Guid Id, CategoryRequest objectRequest);
        Task<OperationResult<Pagination<CategoryResponse>>> GetCategoryPaginationAsync(int pageIndex = 0, int pageSize = 10);
        
        Task<Guid> GetCategoriesCountAsync(ISpecifications<Category> specifications);
    }
}
