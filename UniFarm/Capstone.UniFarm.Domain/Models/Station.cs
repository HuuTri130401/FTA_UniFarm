using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Station")]
    public partial class Station
    {
        public Station()
        {
            ApartmentStations = new HashSet<ApartmentStation>();
            Orders = new HashSet<Order>();
            Transfers = new HashSet<Transfer>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? AreaId { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        [StringLength(1000)]
        public string? Address { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(AreaId))]
        [InverseProperty("Stations")]
        public virtual Area? Area { get; set; }
        [InverseProperty(nameof(ApartmentStation.Station))]
        public virtual ICollection<ApartmentStation> ApartmentStations { get; set; }
        [InverseProperty(nameof(Order.Station))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Transfer.Station))]
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
