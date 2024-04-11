using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class ProductItemResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public double CommissionFee { get; set; } 
        public Guid FarmHubId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProductOrigin { get; set; }
        public string SpecialTag { get; set; }
        public string StorageType { get; set; }
        public bool OutOfStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal Price { get; set; }
        public double Quantity { get; set; }
        public double Sold { get; set; }
        public double MinOrder { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public FarmHubResponse FarmHub { get; set; }
        public ICollection<ProductImageResponse> ProductImages { get; set; }
    }

    public class ProductItemResponseForCustomerView
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid FarmHubId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ProductOrigin { get; set; }
        public string SpecialTag { get; set; }
        public string StorageType { get; set; }
        public bool OutOfStock { get; set; }
        public double Sold { get; set; }
        public double MinOrder { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public ICollection<ProductImageResponse> ProductImages { get; set; }
    }
}
