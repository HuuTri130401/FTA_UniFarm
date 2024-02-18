using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Account")]
    public partial class Account : IdentityUser<Guid>
    {
        public Account()
        {
            AccountRoles = new HashSet<AccountRole>();
            ApartmentStations = new HashSet<ApartmentStation>();
            Orders = new HashSet<Order>();
            Wallets = new HashSet<Wallet>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(100)]
        public string? RoleName { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        [StringLength(10)]
        public string? Phone { get; set; }
        [StringLength(50)]
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        [StringLength(100)]
        public string? Code { get; set; }
        public string? Address { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [InverseProperty(nameof(AccountRole.Account))]
        public virtual ICollection<AccountRole> AccountRoles { get; set; }
        [InverseProperty(nameof(ApartmentStation.Account))]
        public virtual ICollection<ApartmentStation> ApartmentStations { get; set; }
        [InverseProperty(nameof(Order.Customer))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Wallet.Account))]
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
