using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Payment")]
    public partial class Payment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? WalletId { get; set; }
        [StringLength(255)]
        public string? From { get; set; }
        [StringLength(255)]
        public string? To { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public DateTime PaymentDay { get; set; } = DateTime.Now;
        [StringLength(100)]
        public string? Status { get; set; }
        
        public string Type { get; set; }

        [ForeignKey(nameof(WalletId))]
        [InverseProperty("Payments")]
        public virtual Wallet? Wallet { get; set; }
    }
}
