import _get from 'lodash.get';

import { http } from './http';
export const OrderAPI = {
    getAll: async () => {
        const res = await http.get('/collect-hub/orders', {
            params: {
                pageSize: 999,
            },
        });
        return _get(res, 'data');
    },
    getByBatchId: async (batchId: string) => {
        const res = await http.get(`/batch/${batchId}/orders`);
        return _get(res, 'data.payload');
    },
    updateProcessOrder: async (orderId: string, status: string) => {
        const res = await http.put(
            `/batch/process-order/${orderId}`,
            {},
            {
                params: {
                    approveStatus: status,
                },
            }
        );
    },
    getByTransferId: async (transferId: string) => {
        const res = await http.get(`/orders/${transferId}`, {
            params: {
                pageSize: 999,
            },
        });

        return _get(res, 'data');
    },
    getForCreateTransfer: async (stationId: string) => {
        const res = await http.get('/collect-hub/orders', {
            params: {
                pageSize: 999,
                deliveryStatus: 'AtCollectedHub',
                stationId: stationId,
            },
        });
        return _get(res, 'data');
    },
};
