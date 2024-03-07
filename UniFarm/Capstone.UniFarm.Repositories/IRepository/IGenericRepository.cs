using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> AddAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        void SoftRemove(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        void SoftRemoveRange(List<TEntity> entities);
        TEntity Remove(TEntity entity);
        public IQueryable<TEntity> FilterAll(bool? isAscending, string? orderBy = null, Expression<Func<TEntity, bool>>? predicate = null, string[]? includeProperties = null, int pageIndex = 0, int pageSize = 10);
        IQueryable<TEntity> FilterByExpression(Expression<Func<TEntity, bool>> predicate, string[]? includeProperties = null);
        
        Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate, params Expression<Func<TEntity, object>>[]? includeProperties);
    }
}
