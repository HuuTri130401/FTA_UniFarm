import { ExpertItem } from '@models/expert';
import { PagingProps, ResponseList } from '@models/interface';
import { JobLevel, JobLevelItem } from '@models/jobLevel';
import _get from 'lodash.get';

import { IV1AddCriteria } from './criteria.api';
import { http } from './http';

export interface IV1UpdateJobLevel extends Pick<JobLevel, 'id' | 'title' | 'description' | 'shortBrief' | 'price' | 'duration'> {
    skillLevels: string[];
}

export interface IV1GetFilterJobLevel extends Pick<JobLevel, 'title'>, PagingProps {
    priceFrom: number;
    priceTo: number;
    enable: boolean;
}

export interface IV1JobLevelCreateInterview {
    cvId: string;
    date: string;
    time: string;
    expert: string;
    jobLevel: string;
}

export interface IV1JobLevelCreateCriterias {
    jobLevelId: string;
    criteria: IV1AddCriteria[];
}

export interface IV1JobLevelUpdateEnable extends Pick<JobLevel, 'id' | 'enable'> {}

export const jobLevelApi = {
    //NOTE - job endpoint
    v1GetId: async (id: string) => {
        const res = await http.get(`/job-levels/${id}`);
        return _get(res, 'data') as JobLevelItem;
    },

    v1Put: async (input: IV1UpdateJobLevel) => {
        const { id, ...rest } = input;
        const res = await http.put(`/job-levels/${id}`, rest);
        return _get(res, 'data') as JobLevelItem;
    },

    v1PutEnable: async (dto: IV1JobLevelUpdateEnable) => {
        const { id, enable } = dto;
        const res = await http.put(`/job-levels/${id}/status`, { enable });
        return _get(res, 'data') as JobLevelItem;
    },

    v1PostInterview: async (input: IV1JobLevelCreateInterview) => {
        const { cvId, ...rest } = input;
        const res = await http.post(`/job-levels/${cvId}/interview`, rest);
        return _get(res, 'data');
    },

    v1PostCriterias: async (input: IV1JobLevelCreateCriterias) => {
        const { jobLevelId, ...rest } = input;
        const res = await http.post(`/job-levels/${jobLevelId}/criteria`, rest);
        return _get(res, 'data') as JobLevelItem;
    },

    //NOTE - jobs endpoint

    v1GetFilter: async (filter: Partial<IV1GetFilterJobLevel>) => {
        const res = await http.get('/job-levels', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<JobLevelItem>;
    },

    //NOTE - job-levels get expert
    v1GetExpert: async (id: string) => {
        const res = await http.get(`/job-levels/${id}/experts`);
        return _get(res, 'data') as ResponseList<ExpertItem>;
    },

    v1Delete: async (id: string) => {
        const res = await http.delete(`/job-levels/${id}`);
        return _get(res, 'data');
    },
};
