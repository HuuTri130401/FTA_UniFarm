using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Transfer")]
    public partial class Transfer
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CollectedId { get; set; }
        public Guid StationId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? ExpectedReceiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(CollectedId))]
        [InverseProperty(nameof(CollectedHub.Transfers))]
        public virtual CollectedHub Collected { get; set; } = null!;
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("Transfers")]
        public virtual Order Order { get; set; } = null!;
        [ForeignKey(nameof(StationId))]
        [InverseProperty("Transfers")]
        public virtual Station Station { get; set; } = null!;
    }
}
