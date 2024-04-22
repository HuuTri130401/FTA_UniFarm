import _get from 'lodash.get';

import { CreateBatch, CreateBatchREQ } from '@models/batch';
import { http } from '.';
export enum BatchConfirmStatusEnum {
    RECEIVED = 'Received',
    NOT_RECEIVED = 'NotReceived',
}
export interface BatchConfirmFormData {
    ReceivedDescription: string | null;
    Status: BatchConfirmStatusEnum;
    FeedBackImage: Blob | null;
}
export const BatchAPI = {
    getListInBusinessDay: async (hubId: string, bDayId: string) => {
        const res = await http.get(`batches/collected-hub/${hubId}/business-day/${bDayId}`);

        return _get(res, 'data');
    },
    getListOrderInBusinessDayFarmHub: async (hubId: string, bDayId: string) => {
        const res = await http.get(`batch/orders-in-businessday/${bDayId}/farm-hub/${hubId}`);

        return _get(res, 'data');
    },
    getListBatchByBusinessDayFarmHub: async (hubId: string, bDayId: string) => {
        const res = await http.get(`batches/farmhub${hubId}/business-day/${bDayId}`);

        return _get(res, 'data');
    },
    getListOrderByFarmHub: async (hubId: string) => {
        const res = await http.get(`batches/${hubId}`);

        return _get(res, 'data');
    },
    confirm: async (batchId: string, body: BatchConfirmFormData) => {
        const res = await http.put(`batch/${batchId}`, body, {
            headers: {
                'Content-Type': 'multipart/form-data',
                'Access-Control-Allow-Origin': '*',
            },
        });
        return res;
    },
    getDetailBatch: async (batchId: string) => {
        const res = await http.get(`batch/${batchId}/orders`);
        return _get(res, 'data');
    },
    confirmOrder: async (orderId: string, confirmStatus: string) => {
        const res = await http.put(`batch/confirmed-order/${orderId}?confirmStatus=${confirmStatus}`);

        return _get(res, 'data');
    },
    createBatch: async (farmhubId: string, body: CreateBatch) => {
        const res = await http.post(`batch?farmHubId=${farmhubId}`, body);

        return _get(res, 'data');
    },
    createBatches: async (farmhubId: string, body: CreateBatchREQ) => {
        const res = await http.post(`batch?farmHubId=${farmhubId}`, body);

        return _get(res, 'data');
    },
};
