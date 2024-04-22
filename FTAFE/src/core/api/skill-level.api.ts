import { PagingProps, ResponseList } from '@models/interface';
import { SkillLevel, SkillLevelApproveRequest, SkillLevelItem } from '@models/skillLevel';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1UpdateSkillLevel extends Pick<SkillLevel, 'id' | 'name' | 'weight'> {
    skillId: string;
}

export interface IV1GetSkillLevelFilter extends Pick<SkillLevel, 'name'>, PagingProps {}

export interface IV1GetSkillLevelApproveRequestFilter extends PagingProps {
    expertIds: string[];
    skillLevelIds: string[];
}

export const skillLevelApi = {
    //NOTE - job endpoint
    v1GetId: async (id: string) => {
        const res = await http.get(`/skill-levels/${id}`);
        return _get(res, 'data') as SkillLevel;
    },

    v1Put: async (input: IV1UpdateSkillLevel) => {
        const { id, ...rest } = input;
        const res = await http.put(`/skill-levels/${id}`, rest);
        return _get(res, 'data') as SkillLevel;
    },

    v1GetFilter: async (filter: Partial<IV1GetSkillLevelFilter>) => {
        const res = await http.get('/skill-levels', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<SkillLevelItem>;
    },
    v1GetFilterApproveRequest: async (filter: Partial<IV1GetSkillLevelApproveRequestFilter>) => {
        const res = await http.get('/skill-levels/approve-request', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<SkillLevelApproveRequest>;
    },
};
