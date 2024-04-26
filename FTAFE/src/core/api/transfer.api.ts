import { CreateTransferForm, UpdateTransferForm } from '@models/transfer';

import { http } from './http';

export const TransferAPI = {
    create: async (body: CreateTransferForm) => {
        const res = await http.post(`transfer/create`, body);
        return res;
    },
    getAll: async () => {
        const res = await http.get(`transfers/getall`, {
            params: {
                pageSize: 999,
            },
        });
        return res;
    },
    update: async (body: UpdateTransferForm) => {
        const res = await http.put(`transfer/update`, body);
        return res;
    },
    resend: async (id: string) => {
        const res = await http.post(`/transfer/resend/${id}`);
        return res;
    },
};
