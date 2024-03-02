using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            ProductItems = new HashSet<ProductItem>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? CategoryId { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? Name { get; set; } = null!;
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }
        [StringLength(50)]
        public string? Label { get; set; }
        [StringLength(100)]
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("Products")]
        public virtual Category Category { get; set; } = null!;
        
        [InverseProperty(nameof(ProductItem.Product))]
        public virtual ICollection<ProductItem> ProductItems { get; set; }
    }
}
