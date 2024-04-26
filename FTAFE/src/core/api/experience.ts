import { ExperienceForm, ExperienceFormUpdate, ExperienceItem } from '@models/experience';
import _get from 'lodash.get';

import { http } from './http';

export const experienceApi = {
    v1GetExperience: async () => {
        const res = await http.get('/user/experience');
        return _get(res, 'data') as ExperienceItem[];
    },
    v1PostExperience: async (data: ExperienceForm) => {
        const res = await http.post('/experience', data);
        return res;
    },
    v1PutExperience: async (data: ExperienceFormUpdate) => {
        const { id, ...rest } = data;
        const res = await http.put(`/experience/${id}`, rest);
        return _get(res, 'data') as ExperienceItem;
    },
    v1DeleteExperience: async (id: string) => {
        const res = await http.delete(`/experience/${id}`);
        return res;
    },
    v1GetExperienceById: async (id: string) => {
        const res = await http.get(`/experience/${id}`);
        return _get(res, 'data') as ExperienceItem;
    },
    v1GetExperienceTypes: async () => {
        const res = await http.get('/experience/types');
        return _get(res, 'data') as string[];
    },
};
