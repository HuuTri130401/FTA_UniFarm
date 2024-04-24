export enum DeliveryStatus {
    AtCollectedHub = 'Đã tới kho phân loại',
    CanceledByCollectedHub = 'Đã bị hủy bởi kho phân loại',
    CollectedHubNotReceived = 'Kho phân loại không nhận được hàng',
    Pending = 'Chờ xử lý',
    CanceledByFarmHub = 'Đã bị hủy bởi cửa hàng',
    OnTheWayToCollectedHub = 'Đang trên đường tới kho',
    OnTheWayToStation = 'Đang vận chuyển đến trạm',
    PickedUp = 'Đã nhận',
    Active = 'Đang hoạt động',
}
export enum CustomerStatus {
    AtCollectedHub = 'Đã tới kho phân loại',
    OnDelivery = 'Đang vận chuyển',
    CanceledByCollectedHub = 'Đã bị hủy bởi hệ thống',
    Confirmed = 'Đã xác nhận',
    CanceledByFarmHub = 'Đã bị hủy bởi cửa hàng',
    Pending = 'Chờ xử lý',
    PickedUp = 'Đã nhận',
}
export enum BatchStatus {
    Pending = 'Đợi xử lí',
    NotReceived = 'Chưa nhận hàng',
    Received = 'Đã nhận hàng',
    Processed = 'Đang xử lí',
}
