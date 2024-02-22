using Capstone.UniFarm.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IAccountRepository AccountRepository { get; }
        IProductRepository ProductRepository { get; }
        IFarmHubRepository FarmHubRepository { get; }
        int Save();

        Task<int> SaveChangesAsync();
    }
}
