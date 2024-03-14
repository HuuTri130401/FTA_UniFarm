﻿using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.Repository
{
    public class ProductItemRepository : GenericRepository<ProductItem>, IProductItemRepository
    {
        public ProductItemRepository(FTAScript_V1Context context) : base(context)
        {
        }

        public async Task<List<ProductItem>> GetAllProductItemByFarmHubId(Guid farmHubId)
        {
            return await _dbSet
                    .Where(pi => pi.FarmHubId == farmHubId)
                    .ToListAsync(); 
        }

        public async Task<List<ProductItem>> GetAllProductItemByProductId(Guid productId)
        {
            return await _dbSet
                .Where(p => p.ProductId == productId)
                .Include(pi => pi.ProductImages)
                .ToListAsync();
        }

        public async Task<ProductItem> GetProductItemByIdAsync(Guid productId)
        {
            return await _dbSet
                .Include(pi => pi.ProductImages)
                .Include(fr => fr.FarmHub)
                .FirstOrDefaultAsync(pi => pi.Id == productId);
        }
    }
}
