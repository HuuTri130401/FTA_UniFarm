using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FTAScript_V1Context dbContext) : base(dbContext)
        {
        }

        public async Task<bool> ExistsCategoryAsync(Expression<Func<Category, bool>> predicate)
        {
            var existCategory = await _dbSet.Where(predicate).FirstOrDefaultAsync();
            if (existCategory == null)
            {
                return true;
            }
            return false;
        }

        public async Task<Category> GetSingleOrDefaultAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }
    }
}
