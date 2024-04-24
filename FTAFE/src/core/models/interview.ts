import { Moment } from 'moment';

import { Candidate, CandidateItem, candidateItemDefaultValue } from './candidate';
import { CriteriaResult } from './criteriaResult';
import { ExpertItem, expertItemDefaultValues } from './expert';
import { BaseModel, SelectOption } from './interface';
import { JobLevelItem, jobLevelItemDefaultValue } from './jobLevel';
import { Skill } from './skill';

export interface Interview extends BaseModel {
    id: string;
    startDate: string;
    summary: string;
    rejectReason: string;
    strength: string;
    weakness: string;
    status: InterviewStatus;
    recordUrl: string;
    price: number;
    duration: number;
    point: number;
    isShowed: boolean;
}

export interface InterviewItem extends Interview {
    jobLevel: JobLevelItem;
    candidate: CandidateItem;
    expert: ExpertItem;
    criteriaResults: CriteriaResult[];
}

export enum InterviewStatus {
    PENDING = 'PENDING',
    ACCEPTED = 'ACCEPTED',
    REJECTED = 'REJECTED',
    DONE = 'DONE',
    CANCEL = 'CANCEL',
    NONE = '',
}

export const interviewItemDefaultValue: InterviewItem = {
    rejectReason: '',
    id: '',
    startDate: '',
    summary: '',
    strength: '',
    weakness: '',
    status: InterviewStatus.NONE,
    recordUrl: '',
    price: 0,
    duration: 0,
    point: 0,
    isShowed: true,
    candidate: candidateItemDefaultValue,
    expert: expertItemDefaultValues,
    criteriaResults: [],
    jobLevel: jobLevelItemDefaultValue,
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};

export const interviewStatusOptions: SelectOption[] = [
    {
        label: 'Pending',
        value: InterviewStatus.PENDING,
    },
    {
        label: 'Accept',
        value: InterviewStatus.ACCEPTED,
    },
    {
        label: 'Done',
        value: InterviewStatus.DONE,
    },
    {
        label: 'Reject',
        value: InterviewStatus.REJECTED,
    },
    {
        label: 'Cancel',
        value: InterviewStatus.CANCEL,
    },
];

export const interviewStatusFilter: SelectOption[] = [
    {
        label: 'All',
        value: '',
    },
    ...interviewStatusOptions,
];

export enum InterviewResultStatus {
    PASS = 'PASS',
    FAIL = 'FAIL',
    NOT_GRADED = 'NOT_GRADED',
}

export const interviewResultStatusOptions: SelectOption[] = [
    {
        label: 'Pass',
        value: InterviewResultStatus.PASS,
    },
    {
        label: 'Fail',
        value: InterviewResultStatus.FAIL,
    },
];

export const interviewResultStatusFilter: SelectOption[] = [
    {
        label: 'All',
        value: '',
    },
    {
        label: 'Not Graded',
        value: InterviewResultStatus.NOT_GRADED,
    },
    ...interviewResultStatusOptions,
];

export interface BookSkillInterview {
    date: Moment;
    skill: Skill;
    candidate: Candidate;
    id: string;
    createAt: string;
    updateAt: string;
    status: InterviewStatus;
}

export interface CancelInterviewFormValues {
    id: string;
    rejectReason: string;
}

export interface DelayInterviewFormValues {
    id: string;
    date: string;
    time: string;
}

export const defaultCancelInterviewFormValues: CancelInterviewFormValues = {
    id: '',
    rejectReason: '',
};
