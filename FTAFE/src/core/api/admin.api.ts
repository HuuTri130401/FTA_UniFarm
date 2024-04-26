import _get from 'lodash.get';

import { http } from './http';

export type CreateStaffForm = {
    email: string;
    password: string;
    phoneNumber: string;
    firstName: string;
    lastName: string;
    userName: string;
    role: string;
};
export interface MonthFilter {
    month: number;
}
export const ADMIN_API = {
    createStaff: async (body: CreateStaffForm) => {
        const res = await http.post('/admin/account/create', body);
        return res;
    },
    createTransactionForFarmHubInBusinessDay: async (businessDayId: string) => {
        const res = await http.post(`/settlement/payout-for-all-farmhub/businessday/${businessDayId}`);
        return res;
    },
    getDashboard: async () => {
        const res = await http.get('/admin/dashboard');
        return _get(res, 'data');
    },
    getProductStatistic: async () => {
        const res = await http.get('/admin/dashboard/product-statistic');
        return _get(res, 'data');
    },
    getTopFarmHub: async (top?: number) => {
        const res = await http.get('/admin/dashboard/farmhub-ranking', {
            params: {
                top: top || 5,
            },
        });
        return _get(res, 'data');
    },
    getOrders: async () => {
        const res = await http.get('/admin/orders', {
            params: {
                pageSize: 999,
            },
        });
        return _get(res, 'data');
    },
    gerReportByMonth: async (month: Partial<number>) => {
        const res = await http.get('/admin/dashboard/report-by-month', {
            params: {
                month: month,
            },
        });

        return _get(res, 'data');
    },
};
