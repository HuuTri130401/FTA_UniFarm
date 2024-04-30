using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Capstone.UniFarm.Domain.Enum;

public abstract record EnumConstants
{


    /// <summary>
    /// Account status, Account Role Status, Area Status, Apartment Status, Station status
    /// </summary>
    /// 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomerStatus
    {
        Pending, // Đơn hàng đang chờ xử lý
        Confirmed, // Đơn hàng đã được xác nhận
        CanceledByFarmHub, // Đơn hàng đã bị hủy bởi farmhub
        OnDelivery, // Đang vận chuyển
        AtCollectedHub, // Đơn hàng đã được vận chuyển tới kho phân loại
        CanceledByCollectedHub, // Đơn hàng bị hủy bởi hệ thống
        ReadyForPickup, // Sẵn sàng để khách hàng đến nhận
        CanceledByCustomer, // Đơn hàng bị hủy bởi khách hàng
        PickedUp, // Đã được khách hàng nhận
        Expired // Đơn hàng đã hết hạn
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FilterOrderStatus
    {
        Pending,
        Confirmed,
        OnDelivery,
        ReadyForPickup,
        PickedUp,
        Canceled,
        Expired
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeliveryStatus
    {
        Pending, // Đơn hàng đang chờ xử lý
        CanceledByCustomer, // Đơn hàng bị hủy bởi khách hàng
        CanceledByFarmHub, // Đơn hàng đã bị hủy bởi farmhub
        OnTheWayToCollectedHub, // Đang giao đơn hàng tới collected hub
        AtCollectedHub, // Đã đến collected hub
        CollectedHubNotReceived, // CollectedHub không nhận được đơn hàng
        CanceledByCollectedHub, // Đơn hàng bị hủy bởi hệ thống
        OnTheWayToStation, // Đơn hàng đang giao tới Station
        AtStation, // Đơn hàng đã đến Station
        StationNotReceived, // Station không nhận được đơn hàng
        ReadyForPickup, // Đơn hàng sẵn sàng để khách hàng đến nhận
        PickedUp, // Đơn hàng đã được khách hàng nhận
        Expired, // Đơn hàng đã hết hạn,
        Confirmed // Đơn hàng đã được xác nhận
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BatchStatus
    {
        Pending, // FarmHub đã tạo Batch, Chờ xử lý
        Received, // CollectedHub đã nhận Batch từ FarmHub
        NotReceived, // CollectedHub không nhận được Batch từ FarmHub
        Processed // CollectedHub đã xử lý tất cả đơn hàng trong Batch
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FarHubProcessOrder
    {
        Confirmed, // FarmHub xác nhận đơn hàng
        Canceled // FarmHub hủy bỏ đơn hàng
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CollectedHubProcessOrder
    {
        Received, // FarmHub xác nhận đơn hàng đã có
        NotReceived, // CollectedHub không nhận được đơn hàng
        Canceled, // CollectedHub hủy bỏ đơn hàng
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CollectedHubUpdateBatch
    {
        Received, // CollectedHub đã nhận Batch từ FarmHub
        NotReceived, // CollectedHub không nhận được Batch từ FarmHub
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StationUpdateTransfer
    {
        Pending,
        Resend, 
        Received, // Station đã nhận Transfer từ CollectedHub
        NotReceived, // Station không nhận được Transfer từ CollectedHub
        Processed // Station đã xử lý tất cả đơn hàng trong Transfer
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StationStaffUpdateOrderStatus
    {
        AtStation, // Đã tới trạm nhận hàng
        StationNotReceived, // Trạm không nhận được hàng
        PickedUp, // Đã nhận hàng
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatusCompleted
    {
        CanceledByCustomer,
        CanceledByFarmHub,
        CanceledByCollectedHub,
        Expired,
        PickedUp
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CommonEnumStatus
    {
        Active,
        PaymentConfirm,
        Completed,
        Inactive,
        StopSellingDay
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionEnum
    {
        Payment,
        Payout,
        Refund,
        Paid,
        Pending
    }

    public abstract record ActiveInactiveEnum
    {
        public static readonly string ACTIVE = "Active";
        public static readonly string INACTIVE = "Inactive";
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentEnum
    {
        SUCCESS,
        DENIED,
        PENDING
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FromToWallet
    {
        WALLET,
        BANK
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
        public static readonly string ORDER_DOES_NOT_EXIST = "Order does not exist";
        public static readonly string ORDER_DETAIL_DOES_NOT_EXIST = "OrderDetail does not exist";
        public static readonly string PRODUCT_ITEM_IN_MENU_DOES_NOT_EXIST = "Product Item In Menu does not exist";
        public static readonly string PRODUCT_ITEM_DOES_NOT_EXIST_OR_INACTIVE = "Product Item does not exist or is not active";
        public static readonly string PRODUCT_ITEM_AND_FARMHUBID_DOES_NOT_EXIST = "Product Item and FarmHubId does not exist";
        public static readonly string FARMHUB_DOES_NOT_EXIST = "FarmHub does not exist";
        public static readonly string CREATE_CART_SUCCESS = "Create cart success";
        public static readonly string CREATE_CART_FAILURE = "Create cart failure";
        public static readonly string UPDATE_CART_SUCCESS = "Update cart success";
        public static readonly string UPDATE_CART_FAILURE = "Update cart failure";
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
        public static readonly string RESEND_TRANSFER_SUCCESS = "Resend transfer success";
        public static readonly string CREATE_TRANSFER_SUCCESS = "Create transfer success";
        public static readonly string GET_ALL_TRANSFER_SUCCESS = "Get all transfer success";
        public static readonly string UPDATE_TRANSFER_STATUS_SUCCESS = "Update transfer status success";
    }

    public abstract record OrderCustomerStatus
    {
        public static readonly string DANG_VAN_CHUYEN = "Đang vận chuyển";
        public static readonly string CHO_NHAN_HANG = "Chờ nhận hàng";
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FilterOrder
    {
        CreatedAt,
        Code,
        ExpectedReceiveDate,
        TotalAmount,
        ExpiredDayInStation,
        ShippedDate,
        FullName,
        PhoneNumber,
        CustomerStatus,
        DeliveryStatus,
        UpdatedAt
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        Deposit,
        Withdraw,
        Refund
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentMethod
    {
        WITHDRAW,
        DEPOSIT,
    }
    
    public enum PaymentStatus
    {
        SUCCESS,
        DENIED
    }

    public enum TransactionStatus
    {
        Success,
        Failure
    }

    public record AdminWallet
    {
        public const string AdminWalletId = "5D76359B-9CD8-40D5-88E0-5F3498D49718";
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FarmHubSettlementPayment
    {
        Pending,
        Paid
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationType
    {
        Order,
        OrderDetail,
        ProductItemInMenu,
        ProductItem,
        FarmHub,
        Cart,
        Station,
        BusinessDay,
        Transfer,
        Pending,
        Received,
        NotReceived,
        Processed,
        Resend
    }
}