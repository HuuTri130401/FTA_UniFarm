import _get from 'lodash.get';

import { CreatePayment } from '@models/payment';
import { http } from './http';
export interface FilterPayment {
    from?: string;
    to?: string;
}
export enum PaymentRequestStatus {
    PENDING = 'PENDING',
    SUCCESS = 'SUCCESS',
    DENIED = 'DENIED',
}
export interface UpdatePaymentRequestForm {
    id: string;
    status: PaymentRequestStatus;
}
export const PaymentAPI = {
    getAll: async (filter: Partial<FilterPayment>) => {
        const res = await http.get('/payments', {
            params: {
                ...filter,
                pageSize: 999,
            },
        });
        return _get(res, 'data');
    },
    getAllUser: async (filter: Partial<FilterPayment>) => {
        const res = await http.get('/payments/user', {
            params: {
                ...filter,
                pageSize: 999,
            },
        });
        return _get(res, 'data');
    },
    updateStatusPaymentRequest: async (data: UpdatePaymentRequestForm) => {
        const res = await http.put(`/payment/update-withdraw-request`, data);
        return _get(res, 'data');
    },
    getTransactionsAll: async () => {
        const res = await http.get('/transactions/all');
        return _get(res, 'data');
    },

    createPayment: async (data: CreatePayment) => {
        const res = await http.post('/payment/create-withdraw-request', data);
        return _get(res, 'data');
    },
};
