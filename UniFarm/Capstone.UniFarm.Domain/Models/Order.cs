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
            Transactions = new HashSet<Transaction>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? StationId { get; set; }
        public Guid BusinessDayId { get; set; }
        public DateTime CreatedAt { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? ShipAddress { get; set; }
        public DateTime? ExpectedReceiveDate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalFarmHubPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalBenefit { get; set; }
        public Guid? BatchId { get; set; }
        public Guid? CollectedHubId { get; set; }
        public Guid? TransferId { get; set; }
        [StringLength(255)]
        public string? CustomerStatus { get; set; }
        [StringLength(255)]
        public string? DeliveryStatus { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiredDayInStation { get; set; }
        public DateTime? ShippedDate { get; set; }
        public Guid? ShipByStationStaffId { get; set; }

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
        
        [InverseProperty(nameof(Transaction.Order))]
        public virtual ICollection<Transaction> Transactions { get; set; }

        [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
