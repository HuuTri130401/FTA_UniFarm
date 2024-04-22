import _get from 'lodash.get';

import { http } from './http';
export enum StaffType {
    collect = 'collected-hub',
    station = 'station',
}

export const staffApi = {
    getStaffNotWorking: async (type: StaffType) => {
        const res = await http.get(`/admin/${type}/staffs-not-working`, {
            params: {
                pageSize: 999,
            },
        });

        return _get(res, 'data');
    },
    addStaffToCollect: async (staffId: string, collectId: string) => {
        const res = await http.put(`/admin/account-role/update/${staffId}`, {
            accountId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            stationId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            collectedHubId: `${collectId}`,
            farmHubId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            status: 'Active',
        });
        return _get(res, 'data');
    },
    addStaffToStation: async (staffId: string, stationId: string) => {
        const res = await http.put(`/admin/account-role/update/${staffId}`, {
            accountId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            stationId: `${stationId}`,
            collectedHubId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            farmHubId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            status: 'Active',
        });
        return _get(res, 'data');
    },
};
export interface IV1GetFilterStaff {
    name?: string;
    email?: string;
    phone?: string;
}