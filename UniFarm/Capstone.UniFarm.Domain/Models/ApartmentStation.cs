using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("ApartmentStation")]
    public partial class ApartmentStation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? StationId { get; set; }
        public Guid? ApartmentId { get; set; }
        public Guid? AccountId { get; set; }
        public double? Distance { get; set; }
        public bool? IsDefault { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(AccountId))]
        [InverseProperty("ApartmentStations")]
        public virtual Account? Account { get; set; }
        [ForeignKey(nameof(ApartmentId))]
        [InverseProperty("ApartmentStations")]
        public virtual Apartment? Apartment { get; set; }
        [ForeignKey(nameof(StationId))]
        [InverseProperty("ApartmentStations")]
        public virtual Station? Station { get; set; }
    }
}
