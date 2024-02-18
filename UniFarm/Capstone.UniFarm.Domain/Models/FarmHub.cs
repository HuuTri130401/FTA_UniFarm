using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("FarmHub")]
    public partial class FarmHub
    {
        public FarmHub()
        {
            Batches = new HashSet<Batch>();
            Menus = new HashSet<Menu>();
            Orders = new HashSet<Order>();
            Products = new HashSet<Product>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [InverseProperty(nameof(Batch.FarmHub))]
        public virtual ICollection<Batch> Batches { get; set; }
        [InverseProperty(nameof(Menu.FarmHub))]
        public virtual ICollection<Menu> Menus { get; set; }
        [InverseProperty(nameof(Order.FarmHub))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Product.FarmHub))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
