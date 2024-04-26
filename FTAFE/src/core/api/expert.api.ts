import { CVItem } from '@models/cv';
import { Expert, ExpertItem } from '@models/expert';
import { PagingProps, ResponseList } from '@models/interface';
import { Interview, InterviewItem } from '@models/interview';
import { SkillLevelApproveRequest } from '@models/skillLevel';
import { User } from '@models/user';
import _get from 'lodash.get';

import { http } from './http';
import { IV1GetFilterInterview } from './interview.api';

export interface IV1UpdateExpert extends Pick<User, 'email'> {}

export interface IV1GetFilterExpert extends Pick<User, 'email'>, PagingProps {
    name: string;
    skillLevels: string[];
}
export interface IV1ExpertUpdateSkillLevel {
    skillLevels: string[];
}

export interface IV1ExpertUpdateSkillLevelApprove extends Pick<Expert, 'id'> {
    skillLevels: string[];
}

export interface IV1ExpertUpdateSkillLevelReject extends Pick<Expert, 'id'> {
    skillLevels: string[];
}

export interface IV1ExpertUpdateSkillLevelCancel {
    skillLevels: string[];
}

export interface IV1ExpertGetSkillLevelFilter extends PagingProps {
    skillLevelIds: string[];
}

export interface IV1ExpertDeleteSkillLevel {
    skillLevels: string[];
}

export const expertApi = {
    v1Put: async (data: IV1UpdateExpert) => {
        const res = await http.put('/expert/profile', data);
        return _get(res, 'data') as Expert;
    },

    v1GetInterviewsByExpert: async (filter: Partial<IV1GetFilterInterview>) => {
        const res = await http.get('/experts/interviews', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<Interview>;
    },
    v1Get: async (id: string) => {
        const res = await http.get(`/experts/${id}`);
        return _get(res, 'data') as ExpertItem;
    },

    v1PutSkillLevels: async (data: IV1ExpertUpdateSkillLevel) => {
        const { skillLevels } = data;
        const res = await http.put(`/experts/skill-levels`, { skillLevels });
        return _get(res, 'data') as ExpertItem;
    },

    v1PutSkillLevelsApprove: async (data: IV1ExpertUpdateSkillLevelApprove) => {
        const { id, skillLevels } = data;
        const res = await http.put(`/experts/${id}/skill-levels/approve`, { skillLevels });
        return _get(res, 'data') as ExpertItem;
    },

    v1GetFilter: async (filter: Partial<IV1GetFilterExpert>) => {
        const res = await http.get('/experts', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<ExpertItem>;
    },

    v1GetInterviewFilter: async (filter: Partial<IV1GetFilterInterview>) => {
        const res = await http.get('/experts/interviews', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<InterviewItem>;
    },

    v1PutSkillLevelApprove: async (data: IV1ExpertUpdateSkillLevelApprove) => {
        const { id, skillLevels } = data;
        const res = await http.put(`/experts/${id}/skill-levels/approve`, { skillLevels });
        return _get(res, 'data') as ExpertItem;
    },

    v1PutSkillLevelReject: async (data: IV1ExpertUpdateSkillLevelReject) => {
        const { id, skillLevels } = data;
        const res = await http.put(`/experts/${id}/skill-levels/reject`, { skillLevels });
        return _get(res, 'data') as ExpertItem;
    },

    v1PutSkillLevelCancel: async (data: IV1ExpertUpdateSkillLevelCancel) => {
        const res = await http.put(`/experts/skill-levels/cancel`, { ...data });
        return _get(res, 'data') as ExpertItem;
    },

    v1GetExpertCV: async (id: string) => {
        const res = await http.get(`/experts/${id}/cv`);
        return _get(res, 'data') as CVItem;
    },

    v1GetExpertFilterRequestSkillLevel: async (dto: Partial<IV1ExpertGetSkillLevelFilter>) => {
        const res = await http.get(`/experts/skill-levels-requests`, { params: { ...dto } });
        return _get(res, 'data') as ResponseList<SkillLevelApproveRequest>;
    },

    v1DeleteSkillLevel: async (data: IV1ExpertDeleteSkillLevel) => {
        const res = await http.delete(`/experts/skill-levels`, { data: { ...data } });
        return _get(res, 'data') as ExpertItem;
    },
};
