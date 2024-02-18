using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.Repository
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> _dbSet;

        public GenericRepository(FTAScript_V1Context context)
        {
            _dbSet = context.Set<TEntity>();
        }
        public virtual Task<List<TEntity>> GetAllAsync() => _dbSet.ToListAsync();

        public virtual async Task<TEntity> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var result = await _dbSet.FindAsync(id);
            // todo should throw exception when not found
            if (result == null)
                throw new Exception($"Not Found by ID: [{id}] of [{typeof(TEntity).Name}]");
            return result;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual void SoftRemove(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void SoftRemoveRange(List<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public TEntity Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
            return entity;
        }

        public virtual void UpdateRange(List<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            return items;
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            return items.Where(predicate);
        }

        public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate, params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            return await FindAll(includeProperties).SingleOrDefaultAsync(predicate);
        }

        /*public async Task<IReadOnlyList<T>> ListAsync(ISpecifications<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        public async Task<int> CountAsync(ISpecifications<T> specifications)
        {
            return await ApplySpecification(specifications).CountAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecifications<T> specifications)
        {
            return SpecificationEvaluatOr<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), specifications);
        }*/
    }
}

