import { CreateProduct, UpdateProduct } from '@models/product';
import _get from 'lodash.get';

import { http } from './http';
export const productAPI = {
    getProducts: async () => {
        const res = await http.get('/products');

        return _get(res, 'data');
    },
    getProductsByCategoryId: async (categoryId: string) => {
        const res = await http.get(`/category/${categoryId}/products`);

        return _get(res, 'data');
    },

    getProductById: async (id: string) => {
        const res = await http.get(`/product/${id}`);

        return _get(res, 'data');
    },

    updateProduct: async (id: string, body: UpdateProduct) => {
        const res = await http.put(`/product/${id}`, body);

        return _get(res, 'data');
    },

    deleteProduct: async (id: string) => {
        const res = await http.delete(`/product/${id}`);
        return res;
    },

    createProduct: async (body: CreateProduct) => {
        const res = await http.post('/product', body);

        return _get(res, 'data');
    },
};
