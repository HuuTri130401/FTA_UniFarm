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
        public IAreaRepository AreaRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IFarmHubRepository FarmHubRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        public UnitOfWork(FTAScript_V1Context dbContext,
            ICategoryRepository categoryRepo,
            IAreaRepository areaRepo,
            IAccountRepository accountRepo,
            IProductRepository productRepository,
            IFarmHubRepository farmHubRepository,
            IProductImageRepository productImageRepository
        )
        {
            _dbContext = dbContext;
            CategoryRepository = categoryRepo;
            AccountRepository = accountRepo;
            ProductRepository = productRepository;
            FarmHubRepository = farmHubRepository;
            AreaRepository = areaRepo;
            ProductImageRepository = productImageRepository;
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
