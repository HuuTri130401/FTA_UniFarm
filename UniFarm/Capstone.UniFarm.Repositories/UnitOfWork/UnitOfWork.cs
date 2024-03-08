using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly FTAScript_V1Context _dbContext;
        public ICategoryRepository CategoryRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IAreaRepository AreaRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IFarmHubRepository FarmHubRepository { get; }
        public IApartmentRepository ApartmentRepository { get; }
        public IAccountRoleRepository AccountRoleRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        public IProductItemRepository ProductItemRepository { get; }
        public IMenuRepository MenuRepository { get; }
        public IProductItemInMenuRepository ProductItemInMenuRepository { get; }
        public UnitOfWork(FTAScript_V1Context dbContext,
            ICategoryRepository categoryRepo,
            IAreaRepository areaRepo,
            IAccountRepository accountRepo,
            IProductRepository productRepository,
            IFarmHubRepository farmHubRepository,
            IApartmentRepository apartmentRepo,
            IAccountRoleRepository accountRoleRepository,
            IWalletRepository walletRepository,
            IProductImageRepository productImageRepository,
            IMenuRepository menuRepository,
            IProductItemRepository productItemRepository,
            IProductItemInMenuRepository productItemInMenuRepository
        )
        {
            _dbContext = dbContext;
            CategoryRepository = categoryRepo;
            AccountRepository = accountRepo;
            ProductRepository = productRepository;
            FarmHubRepository = farmHubRepository;
            AreaRepository = areaRepo;
            ApartmentRepository = apartmentRepo;
            AccountRoleRepository = accountRoleRepository;
            WalletRepository = walletRepository;
            ProductImageRepository = productImageRepository;
            MenuRepository = menuRepository;
            ProductItemRepository = productItemRepository;
            ProductItemInMenuRepository = productItemInMenuRepository;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
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
