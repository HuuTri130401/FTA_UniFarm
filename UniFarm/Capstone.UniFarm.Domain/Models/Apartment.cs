using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Apartment")]
    public partial class Apartment
    {
        public Apartment()
        {
            ApartmentStations = new HashSet<ApartmentStation>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? AreaId { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("Apartments")]
        public virtual Area? Area { get; set; }
        [InverseProperty(nameof(ApartmentStation.Apartment))]
        public virtual ICollection<ApartmentStation> ApartmentStations { get; set; }
    }
}
