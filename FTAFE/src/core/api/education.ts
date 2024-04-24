import { Education, EducationForm, EducationFormUpdate } from '@models/education';
import _get from 'lodash.get';

import { http } from './http';

export const educationApi = {
    v1GetEducations: async () => {
        const res = await http.get('/user/educations');
        return _get(res, 'data') as Education[];
    },
    v1PostEducation: async (data: EducationForm) => {
        const res = await http.post('/educations', data);
        return res;
    },
    v1PutEducation: async (data: EducationFormUpdate) => {
        const { id, ...rest } = data;
        const res = await http.put(`/educations/${id}`, rest);
        return _get(res, 'data') as Education;
    },
    v1DeleteEducation: async (id: string) => {
        const res = await http.delete(`/educations/${id}`);
        return res;
    },
    v1GetEducationsById: async (id: string) => {
        const res = await http.get(`/educations/${id}`);
        return _get(res, 'data') as Education;
    },
};
