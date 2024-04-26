import { BaseModel } from './interface';

export interface Transaction extends BaseModel {
    amount: number;
    paymentMethod: string;
    orderId: string;
    note: string;
    type: TransactionType;
    id: string;
    status: TransactionStatus;
}

export enum TransactionType {
    DEPOSIT = 'Deposit',
    WITHDRAW = 'Withdraw',
    REFUND = 'Refund',
    PAYMENT = 'Payment',
}

export enum TransactionStatus {
    PENDING = 'Pending',
    SUCCESS = 'Success',
}
