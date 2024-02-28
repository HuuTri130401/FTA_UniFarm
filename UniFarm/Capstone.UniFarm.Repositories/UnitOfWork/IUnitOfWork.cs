using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Data;

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

        int Save();

        Task<int> SaveChangesAsync();
    }
}
