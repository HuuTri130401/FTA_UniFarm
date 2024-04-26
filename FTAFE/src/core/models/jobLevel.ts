import { Criteria } from './criteria';
import { BaseModel } from './interface';
import { Job, jobDefaultValues } from './job';
import { SkillLevel, SkillLevelItem } from './skillLevel';

export interface JobLevel extends BaseModel {
    id: string;
    title: string;
    description: string;
    shortBrief: string;
    price: number;
    enable: boolean;
    duration: number;
}

export const jobLevelDefaultValue: JobLevel = {
    id: '',
    title: '',
    description: '',
    shortBrief: '',
    price: 0,
    enable: false,
    duration: 0,
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};

export interface JobLevelItem extends JobLevel {
    skillLevels: SkillLevelItem[];
    criteria: Criteria[];
    job: Job;
}

export const jobLevelItemDefaultValue: JobLevelItem = {
    ...jobLevelDefaultValue,
    skillLevels: [],
    criteria: [],
    job: jobDefaultValues,
};
