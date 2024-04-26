using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Capstone.UniFarm.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IAccountRepository AccountRepository { get; }
        IAreaRepository AreaRepository { get; }
        IProductRepository ProductRepository { get; }
        IFarmHubRepository FarmHubRepository { get; }
        IApartmentRepository ApartmentRepository { get; }
        IAccountRoleRepository AccountRoleRepository { get; }
        IWalletRepository WalletRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IMenuRepository MenuRepository { get; }
        IProductItemRepository ProductItemRepository { get; }
        IProductItemInMenuRepository ProductItemInMenuRepository { get; }
        IStationRepository StationRepository { get; }
        ICollectedHubRepository CollectedHubRepository { get; }
        IBusinessDayRepository BusinessDayRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        ITransferRepository TransferRepository { get; }
        IApartmentStationRepository ApartmentStationRepository { get; }
        IBatchRepository BatchesRepository { get; }
        IFarmHubSettlementRepository FarmHubSettlementRepository { get; }
        IPriceTableRepository PriceTableRepository { get; }
        IPriceTableItemRepository PriceTableItemRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        int Save();
        Task<int> SaveChangesAsync();
    }
}
