import { CVItem } from '@models/cv';
import { PagingProps, ResponseList } from '@models/interface';
import { DelayInterviewFormValues, Interview, InterviewItem } from '@models/interview';
import { Statistic } from 'antd';
import _get from 'lodash.get';

import { http } from '.';

export interface IV1GetFilterInterview extends Pick<Interview, 'status'>, PagingProps {
    dateFrom: string;
    dateTo: string;
}

export interface IV1GetInterviewDetail extends Pick<Interview, 'id'> {}

export interface IV1UpdateInterviewStatus extends Pick<Interview, 'id' | 'status'> {}

export interface IV1UpdateInterviewDelay extends Pick<Interview, 'id' | 'startDate'> {
    expert: string;
    time: string;
}

export interface IV1UpdateInterviewCancel extends Pick<Interview, 'id' | 'rejectReason'> {}

export interface IV1UpdateInterviewResult extends Pick<Interview, 'id' | 'summary' | 'strength' | 'weakness' | 'point'> {}

export interface IV1DelayInterview extends DelayInterviewFormValues {}

export interface IV1InterviewGetTotalPerDayStatistic {
    from: string;
    to: string;
}

export interface IV1InterviewGetTotalStatistic {
    from: string;
    to: string;
}

export const interviewApi = {
    v1PutStatus: async (data: IV1UpdateInterviewStatus) => {
        const res = await http.put(`/interview/${data.id}/status`, { status: data.status });
        return _get(res, 'data') as Interview;
    },
    v1PutAccept: async (id: string) => {
        const res = await http.put(`/interview/${id}/accept`);
        return _get(res, 'data') as Interview;
    },

    v1PutDone: async (id: string) => {
        const res = await http.put(`/interview/${id}/done`);
        return _get(res, 'data') as Interview;
    },

    v1PutCancel: async (dto: IV1UpdateInterviewCancel) => {
        const { id, rejectReason } = dto;
        const res = await http.put(`/interview/${id}/cancel`, { rejectReason });
        return _get(res, 'data');
    },
    v1PutDelay: async (input: IV1UpdateInterviewDelay) => {
        const { startDate, expert, id, time } = input;
        const res = await http.put(`/interview/${id}`, {
            startDate,
            expert,
            time,
        });
        return _get(res, 'data');
    },

    v1PutDelayCandidate: async (input: IV1DelayInterview) => {
        const { date, id, time } = input;
        const res = await http.put(`/interview/${id}`, {
            date,
            time,
        });
        return _get(res, 'data');
    },

    v1PutResult: async (data: IV1UpdateInterviewResult) => {
        const { id, ...rest } = data;
        const res = await http.put(`/interview/${id}/result`, rest);
        return _get(res, 'data') as InterviewItem;
    },

    v1GetId: async (input: IV1GetInterviewDetail) => {
        const res = await http.get(`/interview/${input.id}`);
        return _get(res, 'data') as InterviewItem;
    },

    v1GetInterviewRecordStatus: async (id: string) => {
        const res = await http.get(`/interview/${id}/record-status`);
        return _get(res, 'data');
    },

    v1AgoraGetToken: async (id: string, role: 1 | 2) => {
        const res = await http.get<string>(`/interview/${id}/agora-token/${role}`);
        return _get(res, 'data', null);
    },

    v1GetResult: async (id: string) => {
        const res = await http.get(`/interview/${id}/result`);
        return _get(res, 'data') as Interview;
    },

    v1StartInterviewRecord: async (id: string) => {
        const res = await http.post(`/interview/${id}/start-record`);
        return _get(res, 'data');
    },

    v1StopInterviewRecord: async (id: string) => {
        const res = await http.post(`/interview/${id}/stop-record`);
        return _get(res, 'data');
    },

    v1GetInterviewCV: async (id: string) => {
        const res = await http.get(`/interview/${id}/cv`);
        return _get(res, 'data') as CVItem;
    },

    // NOTE - interviews endpoint
    v1GetFilter: async (filter: Partial<IV1GetFilterInterview>) => {
        const res = await http.get('/interviews', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<InterviewItem>;
    },

    // NOTE - Statistic

    v1GetInterviewPerDayStatistic: async (dto: IV1InterviewGetTotalPerDayStatistic) => {
        const res = await http.get('/interview/statistics-days', { params: { ...dto } });
        return _get(res, 'data') as Array<{
            day: string;
            total: number;
        }>;
    },

    v1GetInterviewTotalStatistic: async (dto: IV1InterviewGetTotalStatistic) => {
        const res = await http.get('/interview/statistics', { params: { ...dto } });

        return _get(res, 'data') as {
            total: number;
        };
    },

    v1StatusCount: async () => {
        const res = await http.get('/interview/statistics-status');
        return _get(res, 'data') as Array<{
            status: string;
            total: number;
        }>;
    },
};
