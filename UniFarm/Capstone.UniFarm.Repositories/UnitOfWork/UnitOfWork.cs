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
        private readonly FTAScript_V1Context _dbContext;
        public ICategoryRepository CategoryRepository { get; }

        public IAccountRepository AccountRepository { get; }

        public UnitOfWork(FTAScript_V1Context dbContext,
            ICategoryRepository categoryRepo,
            IAccountRepository accountRepo
        )
        {
            _dbContext = dbContext;
            CategoryRepository = categoryRepo;
            AccountRepository = accountRepo;
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

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
