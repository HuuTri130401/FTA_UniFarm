export interface Admin {
    id: string;
    createdAt: string;
    updatedAt: string;
    username: string;
    email: string;
}

export interface Dashboard {
    month: string;
    totalRevenue: number;
    totalDepositMoney: number;
    totalWithdrawMoney: number;
    totalRefundMoney: number;
    totalBenefit: number;
    totalOrder: number;
    totalOrderSuccess: number;
    totalOrderCancel: number;
    totalOrderExpired: number;
    totalNewCustomer: number;
    totalNewFarmHub: number;
}

export interface ProductStatistic {
    productItemId: string;
    productName: string;
    soldQuantity: number;
    percent: number;
}
