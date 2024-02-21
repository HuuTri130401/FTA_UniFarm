﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("ProductItemInMenu")]
    public partial class ProductItemInMenu
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductItemId { get; set; }
        public Guid MenuId { get; set; }
        public double? Price { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(MenuId))]
        [InverseProperty("ProductItemInMenus")]
        public virtual Menu Menu { get; set; } = null!;
        [ForeignKey(nameof(ProductItemId))]
        [InverseProperty("ProductItemInMenus")]
        public virtual ProductItem ProductItem { get; set; } = null!;
    }
}