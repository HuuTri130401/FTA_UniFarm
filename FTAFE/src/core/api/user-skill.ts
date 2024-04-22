import { UserSkill, UserSkillForm } from '@models/userSkill';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1UpdateUserSkill extends Pick<UserSkill, 'id' | 'name'> {}

export const userSkillApi = {
    v1GetUserSkills: async () => {
        const res = await http.get('/user/user-skills');
        return _get(res, 'data') as UserSkill[];
    },
    v1PostUserSkill: async (data: UserSkillForm) => {
        const res = await http.post('/user-skills', data);
        return res;
    },
    v1PutUserSkill: async (input: IV1UpdateUserSkill) => {
        const { id, ...rest } = input;
        const res = await http.put(`/user-skills/${id}`, rest);
        return _get(res, 'data') as UserSkill;
    },
    v1DeleteUserSkill: async (id: string) => {
        const res = await http.delete(`/user-skills/${id}`);
        return res;
    },
};
