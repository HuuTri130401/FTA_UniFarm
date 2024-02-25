using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Area")]
    public partial class Area
    {
        public Area()
        {
            Apartments = new HashSet<Apartment>();
            Stations = new HashSet<Station>();
        }
        [Key]
        public Guid Id { get; set; }
        [StringLength(255)]
        public string? Province { get; set; }
        [StringLength(255)]
        public string? District { get; set; }
        [StringLength(255)]
        public string? Commune { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }

        [InverseProperty(nameof(Apartment.Area))]
        public virtual ICollection<Apartment> Apartments { get; set; }
        [InverseProperty(nameof(Station.Area))]
        public virtual ICollection<Station> Stations { get; set; }
    }
}
