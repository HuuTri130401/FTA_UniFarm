using System.ComponentModel;

namespace Capstone.UniFarm.Domain.Enum;

public abstract record EnumConstants
{
    /*public enum RoleEnum
    {
        [Description("FarmHub")] FarmHub = 0,
        [Description("Customer")] Customer = 1,
        [Description("CollectedStaff")] CollectedStaff = 2,
        [Description("StationStaff")] StationStaff = 3,
    }*/

    /// <summary>
    /// Account status, Account Role Status, Area Status, Apartment Status, Station status
    /// </summary>
    public abstract record ActiveInactiveEnum
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string INACTIVE = "Inactive";
    }
    
    public abstract record PaymentEnum
    {
        public static readonly string SUCCESS = "Success";
        public static readonly string FAILURE = "Failure";
    }
    
    public abstract record PaymentTypeEnum
    {
        public static readonly string DEPOSIT = "Deposit";
        public static readonly string WITHDRAW = "Withdraw";
        public static readonly string VNPAY = "VNPAY";
        public static readonly string WALLET = "Wallet";
    }

    public abstract record RoleEnumString
    {
        public static readonly string FARMHUB = "FarmHub";
        public static readonly string CUSTOMER = "Customer";
        public static readonly string COLLECTEDSTAFF = "CollectedStaff";
        public static readonly string STATIONSTAFF = "StationStaff";
        public static readonly string ADMIN = "Admin";
        
    }
}