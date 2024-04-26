import { ApartmentFilter, CreateApartmentForm, UpdateApartmentForm } from '@models/apartment';
import _get from 'lodash.get';

import { http } from './http';

export const ApartmentAPI = {
    getAll: async (filter: Partial<ApartmentFilter>) => {
        const res = await http.get('/apartments', {
            params: {
                ...filter,
            },
        });
        return _get(res, 'data.payload');
    },
    getOne: async (id: string) => {
        const res = await http.get(`/apartment/${id}`);
        return _get(res, 'data');
    },

    createOne: async (body: CreateApartmentForm) => {
        const res = await http.post('/admin/apartment/create', body);
        return _get(res, 'data');
    },
    updateOne: async (id: string, body: UpdateApartmentForm) => {
        const res = await http.put(`/admin/apartment/update/${id}`, body);
        return _get(res, 'data');
    },
    deleteOne: async (id: string) => {
        const res = await http.delete(`/admin/apartment/delete/${id}`, {
            params: {
                id: id,
            },
        });
        return res;
    },
};
