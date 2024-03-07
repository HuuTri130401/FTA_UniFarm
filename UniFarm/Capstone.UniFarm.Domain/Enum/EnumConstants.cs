﻿using System.ComponentModel;

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

    public abstract record RoleEnumString
    {
        public static readonly string FARMHUB = "FarmHub";
        public static readonly string CUSTOMER = "Customer";
        public static readonly string COLLECTEDSTAFF = "CollectedStaff";
        public static readonly string STATIONSTAFF = "StationStaff";
        public static readonly string ADMIN = "Admin";
        
    }
}