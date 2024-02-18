using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("ProductItem")]
    public partial class ProductItem
    {
        public ProductItem()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ProductItemInMenus = new HashSet<ProductItemInMenu>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        public double? Quantity { get; set; }
        public double? MinOrder { get; set; }
        [StringLength(10)]
        public string? Unit { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ProductItems")]
        public virtual Product Product { get; set; } = null!;
        [InverseProperty(nameof(OrderDetail.ProductItem))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(ProductItemInMenu.ProductItem))]
        public virtual ICollection<ProductItemInMenu> ProductItemInMenus { get; set; }
    }
}
