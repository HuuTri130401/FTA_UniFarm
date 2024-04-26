import { PagingProps, ResponseList } from '@models/interface';
import { Job, JobItem } from '@models/job';
import { JobLevel } from '@models/jobLevel';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1AddJob extends Pick<Job, 'title' | 'description' | 'shortBrief'> {}
export interface IV1UpdateJob extends Pick<Job, 'title' | 'description' | 'shortBrief'> {}
export interface IV1GetJob extends Pick<Job, 'id'> {}

export interface IV1GetFilterJob extends Pick<Job, 'title'>, PagingProps {}

export interface IV1JobCreateJobLevel extends Pick<JobLevel, 'title' | 'description' | 'shortBrief' | 'price' | 'duration'> {
    skillLevels: string[];
    jobId: string;
}

export const jobApi = {
    //NOTE - job endpoint
    v1GetId: async (input: IV1GetJob) => {
        const res = await http.get(`/jobs/${input.id}`);
        return _get(res, 'data') as JobItem;
    },

    v1Put: async (id: string, input: IV1UpdateJob) => {
        const res = await http.put(`/jobs/${id}`, input);
        return _get(res, 'data');
    },

    v1PostJobCreateJobLevel: async (input: IV1JobCreateJobLevel) => {
        const { jobId, ...rest } = input;
        const res = await http.post(`/jobs/${jobId}/job-level`, rest);
        return _get(res, 'data');
    },

    v1Post: async (input: IV1AddJob) => {
        const res = await http.post(`/jobs`, input);
        return _get(res, 'data');
    },

    v1PutEnable: async (id: string, enable: boolean) => {
        const res = await http.put(`/jobs/${id}/status`, { enable });
        return _get(res, 'data');
    },

    //NOTE - jobs endpoint

    v1GetFilter: async (filter: Partial<IV1GetFilterJob>) => {
        const res = await http.get('/jobs', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<JobItem>;
    },

    v1Delete: async (id: string) => {
        const res = await http.delete(`/jobs/${id}`);
        return _get(res, 'data');
    },
};
