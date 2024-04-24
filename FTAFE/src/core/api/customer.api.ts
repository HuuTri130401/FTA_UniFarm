import { CustomerFilter } from '@models/customer';
import _get from 'lodash.get';

import { http } from './http';

export const CustomerAPI = {
    getAll: async (filter: Partial<CustomerFilter>) => {
        const res = await http.get('/admin/manage/customers', {
            params: {
                ...filter,
            },
        });
        return _get(res, 'data');
    },
};
