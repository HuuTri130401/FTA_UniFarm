using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Menu")]
    public partial class Menu
    {
        public Menu()
        {
            ProductItemInMenus = new HashSet<ProductItemInMenu>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid? BusinessDayId { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        [StringLength(100)]
        public string? Tag { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(BusinessDayId))]
        [InverseProperty("Menus")]
        public virtual BusinessDay? BusinessDay { get; set; }
        [ForeignKey(nameof(FarmHubId))]
        [InverseProperty("Menus")]
        public virtual FarmHub FarmHub { get; set; } = null!;
        [InverseProperty(nameof(ProductItemInMenu.Menu))]
        public virtual ICollection<ProductItemInMenu> ProductItemInMenus { get; set; }
    }
}
