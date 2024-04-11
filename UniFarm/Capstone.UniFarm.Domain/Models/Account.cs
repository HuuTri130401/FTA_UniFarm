using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Capstone.UniFarm.Domain.Models
{
    [Table("Account")]
    public sealed partial class Account : IdentityUser<Guid>
    {
        public Account()
        {
            AccountRoles = new AccountRole();
            ApartmentStations = new HashSet<ApartmentStation>();
            Orders = new HashSet<Order>();
            Wallets = new HashSet<Wallet>();
        }

        public override Guid Id { get; set; }
        [StringLength(100)]
        public string? RoleName { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        [StringLength(12)]
        public string? Phone { get; set; }
        
        public override string UserName { get; set; }

        [StringLength(50)]
        public override string Email { get; set; }
        public string? Avatar { get; set; }  = null;
        [StringLength(100)]
        public string? Code { get; set; }
        public string? Address { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [InverseProperty(nameof(AccountRole.Account))]
        public AccountRole? AccountRoles { get; set; }

        [InverseProperty(nameof(ApartmentStation.Account))]
        public ICollection<ApartmentStation>? ApartmentStations { get; set; } = null;
        [InverseProperty(nameof(Order.Customer))]
        public ICollection<Order>? Orders { get; set; }  = null;
        [InverseProperty(nameof(Wallet.Account))]
        public ICollection<Wallet>? Wallets { get; set; }  = null;
    }
}
