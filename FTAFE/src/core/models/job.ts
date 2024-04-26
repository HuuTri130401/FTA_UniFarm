import { BaseModel } from './interface';
import { JobLevelItem } from './jobLevel';

export interface Job extends BaseModel {
    id: string;
    title: string;
    description: string;
    shortBrief: string;
    status: string;
}

export interface JobItem extends Job {
    jobLevels: JobLevelItem[];
}

export const jobDefaultValues: Job = {
    id: '',
    title: '',
    description: '',
    shortBrief: '',
    status: '',
    createdAt: '',
    updatedAt: '',
    isDeleted: false,
};

export const jobItemDefaultValues: JobItem = {
    ...jobDefaultValues,
    jobLevels: [],
};
