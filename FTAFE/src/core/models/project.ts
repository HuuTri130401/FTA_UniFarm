import { BaseModel } from './interface';

export interface Project extends BaseModel {
    startTime: string;
    endTime: string;
    name: string;
    description: string;
}
