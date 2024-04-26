import { CriteriaResult } from '@models/criteriaResult';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1UpdateCriteriaResultItem extends Pick<CriteriaResult, 'mark' | 'comment' | 'id'> {}

export interface IV1UpdateCriteriaResult {
    criteriaResults: IV1UpdateCriteriaResultItem[];
}

export const criteriaResultApi = {
    v1PutCriteriaResult: async (input: IV1UpdateCriteriaResult) => {
        const res = await http.put(`/criteria-result`, input);
        return _get(res, 'data');
    },
};
