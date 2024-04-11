using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public record AccountResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Avatar { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CollectedHub? CollectedHub { get; set; } = null!;
        public FarmHub? FarmHub { get; set; } = null!;
        public Station? Station { get; set; } = null!;
    }
}
