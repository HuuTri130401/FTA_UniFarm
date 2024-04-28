using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category> GetSingleOrDefaultAsync(Expression<Func<Category, bool>> predicate);
        Task<bool> ExistsCategoryAsync(Expression<Func<Category, bool>> predicate);
    }
}
