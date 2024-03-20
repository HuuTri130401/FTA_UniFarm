﻿using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.RequestFeatures
{
    public static class RepositoryExtensions
    {
        public static IQueryable<ProductItem> SearchProductItems(this IQueryable<ProductItem> productItems,
             string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return productItems;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return productItems.Where(e => e.Title.ToLower().Contains(lowerCaseTerm));
        }
    }
}
