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
        public Batch()
        {
            Orders = new HashSet<Order>();
        }

        [Key] 
        public Guid Id { get; set; }
        public Guid CollectedId { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid BusinessDayId { get; set; }
        public Guid CollectedStaffProcessId { get; set; }
        public DateTime? FarmShipDate { get; set; }
        public DateTime? CollectedHubReceiveDate { get; set; }
        public string? ReceivedDescription { get; set; }
        public int? NumberOfOrdersInBatch { get; set; }
        public string? FeedBackImage { get; set; }
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
        public virtual ICollection<Order>? Orders { get; set; }
    }
}