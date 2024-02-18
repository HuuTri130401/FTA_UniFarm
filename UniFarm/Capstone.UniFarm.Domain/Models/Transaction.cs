using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Transaction")]
    public partial class Transaction
    {
        public Transaction()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public DateTime PaymentDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(WalletId))]
        [InverseProperty("Transactions")]
        public virtual Wallet Wallet { get; set; } = null!;
        [InverseProperty(nameof(Order.Transaction))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
