﻿using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Capstone.UniFarm.Repositories.Repository;

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
        public IStationRepository StationRepository { get; }
        public ICollectedHubRepository CollectedHubRepository { get; }
        public IMenuRepository MenuRepository { get; }
        public IProductItemInMenuRepository ProductItemInMenuRepository { get; }
        public IBusinessDayRepository BusinessDayRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderDetailRepository OrderDetailRepository { get; }
        public ITransferRepository TransferRepository { get; }
        public IApartmentStationRepository ApartmentStationRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public IBatchRepository BatchesRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public IFarmHubSettlementRepository FarmHubSettlementRepository { get; }
        public IPriceTableRepository PriceTableRepository { get; }
        public IPriceTableItemRepository PriceTableItemRepository { get; }
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
            IStationRepository stationRepository,
            ICollectedHubRepository collectedHubRepository,
            IProductItemRepository productItemRepository,
            IProductItemInMenuRepository productItemInMenuRepository,
            IBusinessDayRepository businessDayRepository,
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            ITransferRepository transferRepository,
            IApartmentStationRepository apartmentStationRepository,
            IBatchRepository batchRepository,
            ITransactionRepository transactionRepository,
            IFarmHubSettlementRepository farmHubSettlementRepository,
            IPriceTableRepository priceTableRepository,
            IPriceTableItemRepository priceTableItemRepository
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
            StationRepository = stationRepository;
            CollectedHubRepository = collectedHubRepository;
            BusinessDayRepository = businessDayRepository;
            PaymentRepository = paymentRepository;
            OrderRepository = orderRepository;
            OrderDetailRepository = orderDetailRepository;
            TransferRepository = transferRepository;
            ApartmentStationRepository = apartmentStationRepository;
            BatchesRepository = batchRepository;
            TransactionRepository = transactionRepository;
            FarmHubSettlementRepository = farmHubSettlementRepository;
            PriceTableRepository = priceTableRepository;
            PriceTableItemRepository = priceTableItemRepository;
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
        
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }
        
        public async Task<IDbContextTransaction> EndTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
            return transaction;
        }
    }
}
