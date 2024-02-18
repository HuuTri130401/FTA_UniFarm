using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Repositories.IRepository;
using Capstone.UniFarm.Repositories.Repository;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services)
        {
            /*============ Register DB Context =============*/
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            /*============ Register Services =============*/
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAccountService, AccountService>();

            /*============ Register Repository =============*/
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();


            /*============ Register Others Service =============*/

            return services;
        }
    }
}
