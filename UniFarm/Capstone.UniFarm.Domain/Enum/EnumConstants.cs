﻿using System.ComponentModel;
using System.Text.Json.Serialization;

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
    /// 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomerStatus
    {
        Pending, // Đơn hàng đang chờ xử lý
        Confirmed, // Đơn hàng đã được xác nhận
        Cancelled, // Đơn hàng đã bị hủy
        OnTheWayToCollectedHub, // Đang giao tới collected hub
        AtCollectedHub, // Đã đến collected hub
        OnTheWayToStation, // Đang giao tới station
        ReadyForPickup // Sẵn sàng để khách hàng đến nhận
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BatchStatus
    {
        Pending,
        Processed
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConfirmStatus
    {
        Confirmed,
        Cancelled
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApproveStatus
    {
        Approved,
        Cancelled
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeliveryStatus
    {
        Pending,
        OnTheWayToCollectedHub, // Đang giao tới collected hub
        AtCollectedHub, // Đã đến collected hub
        OnTheWayToStation, // Đang giao tới station
        ReadyForPickup // Sẵn sàng để khách hàng đến nhận
    }

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

    public abstract record NotificationMessage
    {
        public static readonly string CART_DOES_NOT_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID_STATIONID_BUSINESSDAYID = "Cart does not exist with same productItemId and farmHubId";
        public static readonly string CART_EXIST_WITH_SAME_PRODUCTITEMID_AND_FARMHUBID = "Cart exist with same productItemId and farmHubId";
        public static readonly string ORDER_DETAIL_DOES_NOT_EXIST = "OrderDetail does not exist";
        public static readonly string PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST = "Product Item In Menu does not exist";
        public static readonly string PRODUCT_ITEM_DOES_NOT_EXIST_OR_INACTIVE = "Product Item does not exist or is not active";
        public static readonly string PRODUCT_ITEM_AND_FARMHUBID_DOES_NOT_EXIST = "Product Item and FarmHubId does not exist";
        public static readonly string FARMHUB_DOES_NOT_EXIST = "FarmHub does not exist";
        public static readonly string CREATE_CART_SUCCESS = "Create cart success";
        public static readonly string CREATE_CART_FAILURE = "Create cart failure";
        public static readonly string UPDATE_CART_SUCCESS = "Update cart success";
        public static readonly string CREATE_CART_ORDER_DETAIL_SUCCESS = "Create cart order detail success";
        public static readonly string CREATE_CART_ORDER_DETAIL_FAILURE = "Create cart order detail failure";
        public static readonly string UPDATE_CART_ORDER_DETAIL_SUCCESS = "Update cart order detail success";
        public static readonly string ADD_TO_CART_SUCCESS = "Add to cart success";
        public static readonly string ADD_TO_CART_FAILURE = "Add to cart failure";
        public static readonly string STATION_DOES_NOT_EXIST = "Station does not exist";
        public static readonly string BUSINESSDAY_DOES_NOT_EXIST = "BusinessDay does not exist";
        public static readonly string NOT_FOUND_ANY_ITEM_IN_CART = "Not found any item in cart";
        public static readonly string GET_CART_SUCCESS = "Get cart success";
    }

    public abstract record TransferStatusEnum
    {
        public static readonly string PENDING = "Pending";
        public static readonly string RECEIVED = "Received";
        public static readonly string NOT_RECEIVED = "NotReceived";
        public static readonly string EXPIRED = "Expired";
    }


    public abstract record OrderMessage
    {
        public static readonly string ORDER_NOT_FOUND = "Order not found";
    }

    public abstract record CollectedHubMessage
    {
        public static readonly string COLLECTED_NOT_FOUND = "Collected not found with id ";
    }

    public abstract record StationMessage
    {
        public static readonly string STATION_NOT_FOUND = "Station not found with id ";
    }
    public abstract record TransferMessage
    {
        public static readonly string TRANSFER_NOT_FOUND = "Transfer not found with id ";
        public static readonly string CREATE_TRANSFER_SUCCESS = "Create transfer success";
        public static readonly string GET_ALL_TRANSFER_SUCCESS = "Get all transfer success";
        public static readonly string UPDATE_TRANSFER_STATUS_SUCCESS = "Update transfer status success";
    }

    public abstract record OrderCustomerStatus
    {
        public static readonly string DANG_DONG_GOI = "Đang đóng gói";
        public static readonly string CHO_VAN_CHUYEN = "Chờ vận chuyển";
        public static readonly string DANG_VAN_CHUYEN = "Đang vận chuyển";
        public static readonly string CHO_NHAN_HANG = "Chờ nhận hàng";
        public static readonly string DA_NHAN_HANG = "Đã nhận hàng";
        public static readonly string HUY_NHAN_HANG = "Hủy nhận hàng";
    }

    public abstract record OrderDeliveryStatus
    {
        public static readonly string DANG_DONG_GOI = "Đang đóng gói";
        public static readonly string CHO_VAN_CHUYEN = "Chờ vận chuyển";
        public static readonly string DANG_VAN_CHUYEN_DEN_KHO = "Đang vận chuyển đến kho";
        public static readonly string KHO_DA_NHAN_HANG = "Kho đã nhận hàng";
        public static readonly string DANG_VAN_CHUYEN_DEN_TRAM = "Đang vận chuyển đến trạm";
        public static readonly string TRAM_DA_NHAN_HANG = "Trạm đã nhận hàng";
    }



}