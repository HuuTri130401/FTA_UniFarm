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
        }

        [Key]
        public Guid Id { get; set; }
        public string TransactionType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        [StringLength(100)]
        public string Status { get; set; }
        public Guid PayerWalletId { get; set; }
        [ForeignKey(nameof(PayerWalletId))]
        [InverseProperty("TransactionsAsPayers")]
        public virtual Wallet PayerWallet { get; set; } = null!;
        public Guid PayeeWalletId { get; set; }
        public Guid? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("Transactions")]
        public virtual Order? Order { get; set; }
    }
}
