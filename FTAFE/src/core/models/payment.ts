import { PagingProps } from './interface';

export type Payment = {
    id: string;
    userName: string;
    balance: number;
    transferAmount: number;
    from: string;
    to: string;
    paymentDay: string;
    status: string;
    type: string;
    bankName: string;
    bankOwnerName: string;
    bankAccountNumber: string;
    code: string;
    note: string;
};
export interface FilterPayment extends PagingProps, Pick<Payment, 'from' | 'to'> {}

export type Transaction = {
    id: string;
    transactionType: string;
    amount: number;
    paymentDate: string;
    status: string;
    payerWalletId: string;
    payerName: string;
    payeeWalletId: string;
    payeeName: string;
};

export type CreatePayment = {
    bankName: string;
    bankOwnerName: string;
    bankAccountNumber: string;
    amount: number;
    note: string;
};
