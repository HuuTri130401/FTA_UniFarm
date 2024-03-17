﻿using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IProductItemInMenuRepository : IGenericRepository<ProductItemInMenu>
    {
        Task<List<ProductItemInMenu>> GetProductItemsByMenuId(Guid menuId);
        Task<bool> GetProductItemByMenuId(Guid menuId);
        void DeleteProductItemInMenu(ProductItemInMenu productItemInMenu);
        Task<ProductItemInMenu> GetByMenuIdAndProductItemId(Guid menuId, Guid productItemId);
        Task<IEnumerable<ProductItemInMenu>> FindStatusProductItem(Expression<Func<ProductItemInMenu, bool>> predicate);
    }
}
