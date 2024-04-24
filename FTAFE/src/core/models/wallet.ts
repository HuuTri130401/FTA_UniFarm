import { BaseModel } from './interface';

export interface Wallet extends BaseModel {
    id: string;
    accountId: string;
    balance: number;
    createdAt: string;
    updatedAt: string | null;
    status: string;
    account: any;
    payments: [];
    transactions: [];
}

export const walletDefaultValues: Wallet = {
    id: '',
    balance: 0,
    accountId: '',
    createdAt: '',
    updatedAt: '',
    status: '',
    account: null,
    payments: [],
    transactions: [],
    isDeleted: false,
};
