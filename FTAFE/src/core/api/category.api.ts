import { CreateCategory, UpdateCategory } from '@models/category';
import _get from 'lodash.get';

import { http } from './http';

export const CategoryAPI = {
    getAllCategories: async () => {
        const res = await http.get('/categories');
        return _get(res, 'data');
    },
    createCategory: async (body: CreateCategory) => {
        const res = await http.post('/category', body);

        return _get(res, 'data');
    },
    getCategoryById: async (id: string) => {
        const res = await http.get(`/category/${id}`);
        return _get(res, 'data');
    },
    updateCategory: async (id: string, body: UpdateCategory) => {
        const res = await http.put(`/category/${id}`, body);

        return _get(res, 'data');
    },
    deleteCategory: async (id: string) => {
        const res = await http.delete(`/category/${id}`);

        return _get(res, 'data');
    },
};
