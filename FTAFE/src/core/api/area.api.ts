import { Area, AreaFilter, CreateAreaForm, UpdateAreaForm } from '@models/area';
import _get from 'lodash.get';

import { http } from './http';

export const AreaAPI = {
    getAll: async (filter: Partial<AreaFilter>) => {
        const res = await http.get('/areas', {
            params: {
                ...filter,
            },
        });
        return _get(res, 'data.payload') as Area[];
    },
    getOne: async (id: string) => {
        const res = await http.get(`/area/${id}`);
        return _get(res, 'data');
    },

    createOne: async (body: CreateAreaForm) => {
        const res = await http.post('/admin/area/create', body);
        return _get(res, 'data');
    },
    updateOne: async (id: string, body: UpdateAreaForm) => {
        const res = await http.put(`/admin/area/update/${id}`, body);
        return _get(res, 'data');
    },
    deleteOne: async (id: string) => {
        const res = await http.delete(`/admin/area/delete/${id}`);
        return res;
    },
    getStationsInArea: async (areaId: String) => {
        const res = await http.get(`/admin/area/${areaId}/stations`);
        return _get(res, 'data');
    },
    getApartmentsInArea: async (areaId: String) => {
        const res = await http.get(`/admin/area/${areaId}/apartments`);
        return _get(res, 'data');
    },
};
