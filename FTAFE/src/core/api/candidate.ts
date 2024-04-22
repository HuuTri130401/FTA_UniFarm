import { CandidateItem } from '@models/candidate';
import { PagingProps, ResponseList } from '@models/interface';
import { Interview, InterviewItem } from '@models/interview';
import { User } from '@models/user';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1CandidateFilterInterview extends PagingProps, Pick<Interview, 'status'> {
    dateFrom: string;
    dateTo: string;
}

export interface IV1CandidateGetTotalStatistic {
    from: string;
    to: string;
}

export interface IV1GetFilterCandidate extends Pick<User, 'email'>, PagingProps {
    name: string;
}

export const candidateApi = {
    v1GetInterviewFilter: async (filter: Partial<IV1CandidateFilterInterview>) => {
        const res = await http.get('/candidate/interviews', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<InterviewItem>;
    },

    // NOTE - Statistic

    v1GetGeCandidateTotalStatistic: async (dto: IV1CandidateGetTotalStatistic) => {
        const res = await http.get('/candidate/statistic', {
            params: { ...dto },
        });
        return _get(res, 'data') as {
            total: number;
        };
    },

    v1GetFilterCandidate: async (filter: Partial<IV1GetFilterCandidate>) => {
        const res = await http.get('/candidate', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<CandidateItem>;
    },

    v1GetCandidateById: async (id: string) => {
        const res = await http.get(`/candidate/${id}`);
        return _get(res, 'data') as CandidateItem;
    },
};
