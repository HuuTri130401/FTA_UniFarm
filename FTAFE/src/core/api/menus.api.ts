import { CreateMenus, UpdateMenus } from '@models/menus';
import _get from 'lodash.get';

import { http } from './http';

export const MenusAPI = {
    getAllMenus: async () => {
        const res = await http.get('/menus');
        return _get(res, 'data');
    },
    createMenu: async (body: CreateMenus) => {
        const res = await http.post('/menu', body);
        return _get(res, 'data');
    },
    getMenuById: async (id: string) => {
        const res = await http.get(`/menu/${id}`);
        return _get(res, 'data');
    },
    updateMenu: async (id: string, body: UpdateMenus) => {
        const res = await http.put(`/menu/${id}`, body);
        return _get(res, 'data');
    },
    deleteMenu: async (id: string) => {
        const res = await http.delete(`/menu/${id}`);
        return _get(res, 'data');
    },
    // New method for assigning a menu to a business day
    assignMenuToBusinessDay: async (businessDayId: string, menuId: string) => {
        const res = await http.post(`/api/v1/business-day/${businessDayId}/menu/${menuId}`);
        return _get(res, 'data');
    },
};
