using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("CollectedHub")]
    public partial class CollectedHub
    {
        public CollectedHub()
        {
            Batches = new HashSet<Batch>();
            Transfers = new HashSet<Transfer>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        [StringLength(255)]
        public string? CollectedHubAddress { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [InverseProperty(nameof(Batch.Collected))]
        public virtual ICollection<Batch> Batches { get; set; }
        [InverseProperty(nameof(Transfer.Collected))]
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
