import { PagingProps, ResponseList } from '@models/interface';
import { Skill, SkillItem } from '@models/skill';
import { SkillLevel } from '@models/skillLevel';
import _get from 'lodash.get';

import { http } from '.';

export interface IV1UpdateSkill extends Pick<Skill, 'id' | 'name' | 'description' | 'status'> {
    skillGroupId: string;
}
export interface IV1GetFilterSkill extends Pick<Skill, 'name'>, PagingProps {}

export interface IVGetSkill extends Pick<Skill, 'id'> {}
export interface IV1SkillPostSkillLevel extends Pick<SkillLevel, 'name' | 'weight'> {
    skillId: string;
}

export const skillApi = {
    v1Put: async (input: IV1UpdateSkill) => {
        const { id, ...rest } = input;
        const res = await http.put(`/skills/${id}`, rest);
        return _get(res, 'data') as Skill;
    },

    v1PutEnable: async (id: string, enable: boolean) => {
        const res = await http.put(`/skills/${id}/status`, { enable });
        return _get(res, 'data') as Skill;
    },

    v1GetId: async (input: IVGetSkill) => {
        const res = await http.get(`/skills/${input.id}`);
        return _get(res, 'data') as SkillItem;
    },

    v1GetFilter: async (filter: Partial<IV1GetFilterSkill>) => {
        const res = await http.get('/skills', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<SkillItem>;
    },
    v1SkillPostSkillLevel: async (input: IV1SkillPostSkillLevel) => {
        const { skillId, ...rest } = input;
        const res = await http.post(`/skills/${skillId}/skill-levels`, rest);
        return _get(res, 'data') as SkillLevel;
    },
};
