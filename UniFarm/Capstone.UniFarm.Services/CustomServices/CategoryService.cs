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
                    result.AddError(StatusCode.NotFound, "List Categories is Empty!");
                    return result;
                }
                result.AddResponseStatusCode(StatusCode.Ok, "Get List Categories Done.", listCategoriesResponse);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<CategoryResponse>> GetCategoryById(Guid categoryId)
        {
            var result = new OperationResult<CategoryResponse>();
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if(category == null)
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
                _logger.LogError(ex, $"Error occurred in GetCategoryById service method for category ID: {categoryId}");
                throw;
            }
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
