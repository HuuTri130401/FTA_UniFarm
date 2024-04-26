import { Criteria } from '@models/criteria';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1AddCriteria extends Pick<Criteria, 'name' | 'description'> {}

export interface IV1UpdateCriteria extends Pick<Criteria, 'id' | 'name' | 'description'> {}

export const criteriaApi = {
    v1GetId: async (id: string) => {
        const res = await http.get(`/criteria/${id}`);
        return _get(res, 'data') as Criteria;
    },

    v1Put: async (input: IV1UpdateCriteria) => {
        const { id, ...rest } = input;
        const res = await http.put(`/criteria/${id}`, rest);
        return _get(res, 'data') as Criteria;
    },
};
