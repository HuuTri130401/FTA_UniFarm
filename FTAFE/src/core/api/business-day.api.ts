import { CreateBusinessDay } from '@models/business-day';
import _get from 'lodash.get';

import { http } from './http';
export type BusinessDayStat = {
    totalRevenue: number;
    totalOrder: number;
    totalOrderDelivering: number;
    totalOrderSuccess: number;
    totalOrderCancelByCustomer: number;
    totalOrderCancelByFarm: number;
    totalOrderCancelBySystem: number;
    totalOrderExpired: number;
    totalOrderPending: number;
    totalOrderConfirmed: number;
};
export const BusinessDayAPI = {
    getAll: async () => {
        const res = await http.get('/business-days');
        return _get(res, 'data');
    },
    getById: async (id: string) => {
        const res = await http.get(`/business-day/${id}`);
        return _get(res, 'data');
    },
    getDetailByFarmHub: async (id: string) => {
        const res = await http.get(`farmhub/business-day/${id}`);
        return _get(res, 'data');
    },
    deleteOne: async (id: string) => {
        const res = await http.delete(`business-day/${id}`);
        return _get(res, 'data');
    },
    createOne: async (body: CreateBusinessDay) => {
        const res = await http.post('business-day', body);
        return _get(res, 'data');
    },
    createMenuInBusinessDay: async (id: string, menuId: string) => {
        const res = await http.post(`business-day/${id}/menu/${menuId}`);
        return _get(res, 'data');
    },
    getStatistic: async (id: string) => {
        const res = await http.get(`/admin/dashboard/business-day/${id}/order-statistic`);
        return _get(res, 'data');
    },
    getSettlement: async (businessDayId: string, farmHubId: string) => {
        const res = await http.get(`/settlement/businessday/${businessDayId}/farmhub/${farmHubId}`);
        return _get(res, 'data');
    },
};
