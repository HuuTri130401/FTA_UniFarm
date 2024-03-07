using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public abstract record AboutMeResponse
{

    public record AboutMeRoleAndID
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
    }
    
    public record AboutCustomerResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Wallet Wallet { get; set; } = null!;
        /*public ICollection<Payment>? Payments { get; set; } = null!;
        public ICollection<Transaction>? Transactions { get; set; } = null!;
        public ApartmentStation ApartmentStation { get; set; } = null!;
        public ICollection<OrderResponse> Orders { get; set; } = null!;*/

    }

    public record AboutAdminResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Wallet Wallet { get; set; } = null!;
    }
    
    public record AboutFarmHubResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Wallet Wallet { get; set; } = null!;
        public FarmHubResponse FarmHub { get; set; } = null!;
    }
    
    public record AboutStationStaffResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Station? Station { get; set; } = null!;
    }
    
    public record AboutCollectedStaffResponse
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? Code { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CollectedHub? CollectedHub { get; set; } = null!;
    }
    
}