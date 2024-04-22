import { PagingProps, ResponseList } from '@models/interface';
import { Skill } from '@models/skill';
import { SkillGroup, SkillGroupItem } from '@models/skillGroup';
import _get from 'lodash.get';

import { http } from '.';

export interface IV1SkillGroupPost extends Pick<SkillGroup, 'name'> {}

export interface IV1SkillGroupPut extends Pick<SkillGroup, 'id' | 'name'> {}

export interface IVSkillGroupGet extends Pick<SkillGroup, 'id'> {}

export interface IV1SkillGroupFilter extends Pick<SkillGroup, 'name'>, PagingProps {}

export interface IV1SkillGroupPostSkill extends Pick<Skill, 'name' | 'description'> {
    skillGroupId: string;
}

export const skillGroupApi = {
    v1GetId: async (input: IVSkillGroupGet) => {
        const res = await http.get(`/skill-groups/${input.id}`);
        return _get(res, 'data') as SkillGroupItem;
    },
    v1Post: async (input: IV1SkillGroupPost) => {
        const res = await http.post(`/skill-groups`, input);
        return _get(res, 'data') as SkillGroup;
    },

    v1Put: async (input: IV1SkillGroupPut) => {
        const { id, ...rest } = input;
        const res = await http.put(`/skill-groups/${id}`, rest);
        return _get(res, 'data') as SkillGroup;
    },

    v1GetFilter: async (filter: Partial<IV1SkillGroupFilter>) => {
        const res = await http.get(`/skill-groups`, { params: { ...filter } });
        return _get(res, 'data') as ResponseList<SkillGroup>;
    },

    v1SkillGroupPostSkill: async (input: IV1SkillGroupPostSkill) => {
        const { skillGroupId, ...rest } = input;
        const res = await http.post(`/skill-groups/${skillGroupId}/skills`, rest);
        return _get(res, 'data') as Skill;
    },
};
