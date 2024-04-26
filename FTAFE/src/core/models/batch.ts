export type Batch = {
    id: string;
    customerId: string;
    customerName: string;
    businessDayId: string;
    businessDayName: string;
    businessDayOpen: string;
    createdAt: string;
    code: string;
    shipAddress: string;
    totalAmount: number;
    customerStatus: string;
    deliveryStatus: string;
    isPaid: boolean;
    collectedHubReceiveDate: string;
    status: string;
    collectedHubName: string;
    collectedHubAddress: string;
    farmShipDate: string;
    orderDetails: OrderDetail[];
};

export type OrderDetail = {
    orderId: string;
    productName: string;
    productItemId: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    unit: string;
    title?: string;
    productItemTitle?: string;
};

export type Order = {
    id: string;
    customerId: string;
    customerName: string;
    createdAt: string;
    code: string;
    shipAddress: string;
    totalAmount: number;
    customerStatus: string;
    deliveryStatus: string;
    isPaid: boolean;
};

export type BatchDetail = {
    id: string;
    collectedId: string;
    collectedHubName: string;
    farmHubId: string;
    farmHubName: string;
    businessDayId: string;
    businessDayName: string;
    businessDayOpen: string;
    farmShipDate: string;
    collectedHubReceiveDate: string;
    receivedDescription: string;
    feedBackImage: string;
    status: string;
    orders: Order[];
};

export type CreateBatch = {
    businessDayName: string;
    businessDayId: string;
    collectedId: string;
    orderIds: string[];
};

export type CreateBatchREQ = {
    businessDayId: string;
    collectedId: string;
    orderIds: string[];
};

export type CreateBatchInput = {
    id: string;
    businessDayId: string;
    collectedId: string;
};
