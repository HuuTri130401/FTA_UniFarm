using Capstone.UniFarm.Domain.Models;
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

        public static IQueryable<ProductItemInMenu> SearchProductItemsInMenu(this IQueryable<ProductItemInMenu> productItemsInMenu,
            string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return productItemsInMenu;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return productItemsInMenu.Where(e => e.ProductItem.Title.ToLower().Contains(lowerCaseTerm));
        }

        //public static IQueryable<Batch> SearchBatches(this IQueryable<Batch> batches,
        //     string searchTerm)
        //{
        //    if (string.IsNullOrWhiteSpace(searchTerm))
        //        return batches;
        //    var lowerCaseTerm = searchTerm.Trim().ToLower();
        //    return batches.Where(e => e.Title.ToLower().Contains(lowerCaseTerm));
        //}
    }
}
