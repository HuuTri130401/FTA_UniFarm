import { BaseModel } from './interface';

export interface CriteriaResult extends BaseModel {
    id: string;
    name: string;
    description: string;
    mark: number;
    comment: string;
}
