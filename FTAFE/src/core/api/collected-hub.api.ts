import { PagingProps } from '@models/interface';
import { CollectedHub } from '@models/staff';
import _get from 'lodash.get';

import { http } from './http';

export interface CollectedHubFilter extends PagingProps, Pick<CollectedHub, 'name' | 'description' | 'address'> {}

export interface UpdateCollectedHub extends Omit<CollectedHub, 'createdAt' | 'updatedAt'> {}

export interface CreateCollectedHub extends Pick<CollectedHub, 'code' | 'name' | 'description' | 'address' | 'image'> {}

export const CollectedHubAPI = {
    getAll: async (filter: Partial<CollectedHubFilter>) => {
        const res = await http.get('/admin/collected-hubs', {
            params: {
                ...filter,
            },
        });
        return _get(res, 'data.payload');
    },
    getById: async (id: string) => {
        const res = await http.get(`/admin/collected-hub/${id}/staffs`, {});
        return _get(res, 'data');
    },
    update: async (id: string, data: UpdateCollectedHub) => {
        const res = await http.put(`/admin/collected-hub/${id}`, data, {});
        return _get(res, 'data');
    },
    createOne: async (body: CreateCollectedHub) => {
        const res = await http.post('/admin/collected-hub', body, {});
        return _get(res, 'data');
    },

    getStaff: async (id: string) => {
        const res = await http.get(`/admin/collected-hub/${id}/staffs-filter`, {
            params: {
                pageSize: 999,
            },
        });
        return _get(res, 'data');
    },

    deleteOne: async (id: string) => {
        const res = await http.delete(`/admin/delete/${id}`, {});
        return res;
    },
};
