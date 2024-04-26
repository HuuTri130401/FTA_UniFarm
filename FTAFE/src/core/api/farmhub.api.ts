import { CreateFarmHubForm, CreateFarmHubFormData, UpdateFarmHubForm } from '@models/farmhub';
import _get from 'lodash.get';

import { http } from './http';

export const FarmHubAPI = {
    getFarmHubs: async () => {
        const res = await http.get('/farm-hubs');
        return _get(res, 'data');
    },
    getFarmHubById: async (id: string) => {
        const res = await http.get(`/farm-hub/${id}`);
        return _get(res, 'data');
    },
    updateFarmHub: async (id: string, data: UpdateFarmHubForm) => {
        const res = await http.put(`/farm-hub/${id}`, data);
        return _get(res, 'data');
    },
    deleteFarmHub: async (id: string) => {
        const res = await http.delete(`/farm-hub/${id}`);
        return res;
    },
    createFarmHub: async (data: CreateFarmHubForm) => {
        const res = await http.post(`/farm-hub`, data);
        return _get(res, 'data');
    },

    createShop: async (data: CreateFarmHubFormData) => {
        const res = await http.post('/farm-hub/create-shop', data, {
            headers: {
                'Content-Type': 'multipart/form-data',
                'Access-Control-Allow-Origin': '*',
            },
        });
        return _get(res, 'data');
    },
    getProductItemSelling: async (id: string) => {
        const res = await http.get(`farm-hub/product-item-selling-percent-ratio?businessDayId=${id}`);
        return _get(res, 'data');
    },
};
