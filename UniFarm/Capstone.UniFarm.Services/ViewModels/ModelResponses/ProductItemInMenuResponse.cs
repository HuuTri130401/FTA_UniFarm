using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class ProductItemInMenuResponse
    {
        public Guid Id { get; set; }
        public Guid ProductItemId { get; set; }
        public Guid MenuId { get; set; }
        public double Quantity { get; set; }
        public double Sold { get; set; }
        public double SalePrice { get; set; }
        //public decimal Price { get; set; }
        public string Status { get; set; }
        public MenuResponse? Menu { get; set; }
        public ProductItemResponse? ProductItem { get; set; }
    }

    public class ProductItemInMenuResponseForCustomer
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public double Sold { get; set; }
        public double SalePrice { get; set; }
        public string Status { get; set; }
        public ProductItemResponseForCustomerView? ProductItem { get; set; }
    }

    public record ProductItemSellingPercentRatio
    {
        public Guid ProductItemId { get; set; }
        public string Title { get; set; }
        public double? SalePrice { get; set; }
        public double? Quantity { get; set; }
        public double? Sold { get; set; }
        public string Status { get; set; }
        public double? SoldPercent { get; set; }
    }
}
