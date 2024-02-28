using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("ProductImage")]
    public partial class ProductImage
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? Caption { get; set; }
        public string? ImageUrl { get; set; }
        public int? DisplayIndex { get; set; }

        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ProductImages")]
        public virtual Product Product { get; set; } = null!;
    }
}
