using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniFarmContext _dbContext;
        public ICategoryRepository CategoryRepository { get; }

        public UnitOfWork(UniFarmContext dbContext, ICategoryRepository categoryRepo)
        {
            _dbContext = dbContext;
            CategoryRepository = categoryRepo;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        public int Save()
        {
            return _dbContext.SaveChanges();
        }
    }
}
