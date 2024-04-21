using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("BusinessDay")]
    public partial class BusinessDay
    {
        public BusinessDay()
        {
            Batches = new HashSet<Batch>();
            Menus = new HashSet<Menu>();
            Orders = new HashSet<Order>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        public DateTime? RegiterDay { get; set; }
        public DateTime? EndOfRegister { get; set; }
        public DateTime OpenDay { get; set; }
        public DateTime? StopSellingDay { get; set; }
        //public TimeSpan? StartTime { get; set; }
        //public TimeSpan? EndTime { get; set; }
        public DateTime? EndOfDay { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [InverseProperty(nameof(Batch.BusinessDay))]
        public virtual ICollection<Batch> Batches { get; set; }
        [InverseProperty(nameof(Menu.BusinessDay))]
        public virtual ICollection<Menu> Menus { get; set; }
        [InverseProperty(nameof(Order.BusinessDay))]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<FarmHubSettlement> FarmHubSettlements { get; set; }
    }
}
