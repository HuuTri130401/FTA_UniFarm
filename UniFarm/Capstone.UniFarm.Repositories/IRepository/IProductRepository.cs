using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetAllProductByCategoryId(Guid categoryId);
        Task<Product> GetSingleOrDefaultProductAsync(Expression<Func<Product, bool>> predicate);
    }
}
