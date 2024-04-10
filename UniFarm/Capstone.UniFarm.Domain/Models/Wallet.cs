using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Wallet")]
    public partial class Wallet
    {
        public Wallet()
        {
            Payments = new HashSet<Payment>();
            TransactionsAsPayers = new HashSet<Transaction>();
            //TransactionsAsPayees = new HashSet<Transaction>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("Wallets")]
        public virtual Account Account { get; set; } = null!;
        [InverseProperty(nameof(Payment.Wallet))]
        public virtual ICollection<Payment> Payments { get; set; }
        [InverseProperty("PayerWallet")]
        public virtual ICollection<Transaction> TransactionsAsPayers { get; set; }

        //[InverseProperty("PayeeWallet")]
        //public virtual ICollection<Transaction> TransactionsAsPayees { get; set; }
    }
}
