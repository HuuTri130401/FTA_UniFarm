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
        public Transfer()
        {
            Orders = new HashSet<Order>();
        }
        [Key]
        public Guid Id { get; set; }
        public Guid CollectedId { get; set; }
        public Guid StationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string? NoteSend { get; set; }
        public string? NoteReceived { get; set; }
        public string? Code { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }
        
        [ForeignKey(nameof(CollectedId))]
        [InverseProperty(nameof(CollectedHub.Transfers))]
        public virtual CollectedHub Collected { get; set; } = null!;

        [ForeignKey(nameof(StationId))]
        [InverseProperty("Transfers")]
        public virtual Station Station { get; set; } = null!;
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
