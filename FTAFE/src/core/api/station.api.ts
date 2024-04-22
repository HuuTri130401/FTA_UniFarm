import { PagingProps } from '@models/interface';
import { CollectedHub, Staff, Station } from '@models/staff';
import _get from 'lodash.get';

import { http } from './http';

export interface StationFilter extends PagingProps, Pick<CollectedHub, 'name' | 'description' | 'address'> {}

export interface UpdateStation extends Omit<Station, 'createdAt' | 'updatedAt' | 'area'> {}

export interface CreateStation extends Pick<Station, 'code' | 'name' | 'description' | 'address' | 'image' | 'areaId'> {}

export interface StaffFilter extends PagingProps, Pick<Staff, 'firstName' | 'lastName' | 'email' | 'phoneNumber'> {}

export const StationAPI = {
    getAll: async () => {
        const res = await http.get('/admin/stations', {
            params: {
                pageSize: 999,
            },
        });
        return _get(res, 'data.payload');
    },
    getById: async (id: string) => {
        const res = await http.get(`/admin/station/${id}`, {});
        return _get(res, 'data');
    },
    update: async (id: string, data: UpdateStation) => {
        const res = await http.put(`/admin/station/${id}`, data);
        return _get(res, 'data');
    },
    createOne: async (body: CreateStation) => {
        const res = await http.post('/admin/station', body);
        return _get(res, 'data');
    },

    getStaff: async (id: string) => {
        const res = await http.get(`/admin/station/${id}/staffs-filter`, {
            params: {
                pageSize: 999,
            },
        });
        return _get(res, 'data');
    },

    deleteOne: async (id: string) => {
        const res = await http.delete(`/admin/station/${id}`, {});
        return res;
    },
};
