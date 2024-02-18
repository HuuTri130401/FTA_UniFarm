using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("AccountRole")]
    public partial class AccountRole
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? StationId { get; set; }
        public Guid? CollectedHubId { get; set; }
        public Guid? FarmHubId { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("AccountRoles")]
        public virtual Account? Account { get; set; }
    }
}
