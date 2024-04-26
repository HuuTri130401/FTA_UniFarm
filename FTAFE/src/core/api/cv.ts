import { CV, CVItem, Layout } from '@models/cv';
import { InterviewItem } from '@models/interview';
import { BookingTime } from '@models/time';
import { Statistic } from 'antd';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1AddCV extends Pick<CV, 'name' | 'title' | 'fullName' | 'avatar' | 'about' | 'yearsOfWorkingExperience' | 'layout'> {
    userSkills: string[];
    experiences: string[];
    certificates: string[];
    educations: string[];
    contacts: string[];
}
export interface IV1UpdateCV extends Pick<CV, 'id' | 'name' | 'title' | 'fullName' | 'avatar' | 'about' | 'yearsOfWorkingExperience' | 'layout'> {
    userSkills: string[];
    experiences: string[];
    certificates: string[];
    educations: string[];
    contacts: string[];
}

export interface IV1CVCreateInterview extends Pick<CV, 'id'> {
    date: string;
    time: BookingTime;
    expert: string;
    jobLevel: string;
}

export interface IV1CVUpdateIsPublic extends Pick<CV, 'id' | 'isPublic'> {}

export interface IV1CVPublicCV extends Pick<CV, 'id' | 'isPublic'> {
    slug: string;
}

export interface IV1CVGetTotalStatistic {
    from: string;
    to: string;
}

export const cvApi = {
    v1GetCVs: async () => {
        const res = await http.get('/user/cvs');
        return _get(res.data, 'data') as CVItem[];
    },

    v1GetCvById: async (id: string) => {
        const res = await http.get(`/cv/${id}`);
        return _get(res, 'data') as CVItem;
    },

    v1GetCvBySlug: async (slug: string) => {
        const res = await http.get(`/cv/slug/${slug}`);
        return _get(res, 'data') as CVItem;
    },
    v1PostCV: async (data: IV1AddCV) => {
        const res = await http.post('/cv', data);
        return _get(res, 'data') as CVItem;
    },
    v1PutCV: async (data: IV1UpdateCV) => {
        const { id, ...rest } = data;
        const res = await http.put(`/cv/${id}`, rest);
        return _get(res, 'data') as CVItem;
    },
    v1DeleteCV: async (id: string) => {
        const res = await http.delete(`/cv/${id}`);
        return res;
    },
    v1PostInterview: async (dto: IV1CVCreateInterview) => {
        const { id, ...rest } = dto;
        const res = await http.post(`/cv/${id}/interview`, rest);
        return _get(res, 'data') as InterviewItem;
    },
    v1PutIsPublic: async (dto: IV1CVPublicCV) => {
        const { id, ...rest } = dto;
        const res = await http.put(`/cv/${id}/is-public`, rest);
        return _get(res, 'data') as CVItem;
    },

    // NOTE Statistic

    v1GetTotalCV: async (dto: IV1CVGetTotalStatistic) => {
        const res = await http.get('/cv/statistic', {
            params: dto,
        });
        return _get(res, 'data') as {
            total: number;
        };
    },
};
