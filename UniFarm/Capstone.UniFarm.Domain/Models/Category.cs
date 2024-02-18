using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }
        public int? DisplayIndex { get; set; }
        public double? SystemPrice { get; set; }
        public double? MinSystemPrice { get; set; }
        public double? MaxSystemPrice { get; set; }
        public double? Margin { get; set; }

        [InverseProperty(nameof(Product.Category))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
