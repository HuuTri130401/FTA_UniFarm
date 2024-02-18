using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Order")]
    public partial class Order
    {
        public Order()
        {
            Batches = new HashSet<Batch>();
            OrderDetails = new HashSet<OrderDetail>();
            Transfers = new HashSet<Transfer>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid? StationId { get; set; }
        public Guid BusinessDayId { get; set; }
        public DateTime CreatedAt { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? ShipAddress { get; set; }
        public DateTime? ExpectedReceiveDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalAmount { get; set; }
        [StringLength(100)]
        public string? CustomerStatus { get; set; }
        [StringLength(100)]
        public string? DeliveryStatus { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(BusinessDayId))]
        [InverseProperty("Orders")]
        public virtual BusinessDay BusinessDay { get; set; } = null!;
        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(Account.Orders))]
        public virtual Account Customer { get; set; } = null!;
        [ForeignKey(nameof(FarmHubId))]
        [InverseProperty("Orders")]
        public virtual FarmHub FarmHub { get; set; } = null!;
        [ForeignKey(nameof(StationId))]
        [InverseProperty("Orders")]
        public virtual Station? Station { get; set; }
        [ForeignKey(nameof(TransactionId))]
        [InverseProperty("Orders")]
        public virtual Transaction? Transaction { get; set; }
        [InverseProperty(nameof(Batch.Order))]
        public virtual ICollection<Batch> Batches { get; set; }
        [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(Transfer.Order))]
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
