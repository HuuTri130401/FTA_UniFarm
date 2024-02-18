using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Batch")]
    public partial class Batch
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CollectedId { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid OrderId { get; set; }
        public Guid BusinessDayId { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(BusinessDayId))]
        [InverseProperty("Batches")]
        public virtual BusinessDay BusinessDay { get; set; } = null!;
        [ForeignKey(nameof(CollectedId))]
        [InverseProperty(nameof(CollectedHub.Batches))]
        public virtual CollectedHub Collected { get; set; } = null!;
        [ForeignKey(nameof(FarmHubId))]
        [InverseProperty("Batches")]
        public virtual FarmHub FarmHub { get; set; } = null!;
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("Batches")]
        public virtual Order Order { get; set; } = null!;
    }
}
