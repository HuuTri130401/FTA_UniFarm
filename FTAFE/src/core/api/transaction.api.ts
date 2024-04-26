import { Transaction } from '@models/transaction';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1TransactionPaypalDeposit extends Pick<Transaction, 'amount'> {
    redirectUrl: string;
    cancelUrl: string;
}

export interface IV1TransactionMomoDeposit extends Pick<Transaction, 'amount'> {
    redirectUrl: string;
    cancelUrl: string;
}

// interface MomoObjectReturn {
//     partnerCode: string;
//     orderId: string;
//     requestId: string;
//     amount: number;
//     responseTime: number;
//     message: string;
//     resultCode: number;
//     payUrl: string;
// }

export interface IV1TransactionGetRevenue {
    from: string;
    to: string;
}

export const transactionApi = {
    v1DepositPaypal: async (input: IV1TransactionPaypalDeposit) => {
        const res = await http.post('/transaction/paypal', input);
        return _get(res, 'data') as string;
    },

    v1DepositMomo: async (input: IV1TransactionMomoDeposit) => {
        const res = await http.post('/transaction/momo', input);
        return _get(res, 'data') as string;
    },

    v1SuccessDeposit: async (token: string) => {
        const res = await http.put(`/transaction/success/${token}`);
        return _get(res, 'data') as string;
    },

    v1Revenue: async (dto: IV1TransactionGetRevenue) => {
        const res = await http.get('/transaction/statistics', {
            params: { ...dto },
        });
        return _get(res, 'data') as Array<{
            total: number;
            day: string;
        }>;
    },
};
