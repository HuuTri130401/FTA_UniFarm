import { BaseModel } from './interface';

export interface Criteria extends BaseModel {
    id: string;
    name: string;
    description: string;
}

export const criteriaDefaultValues: Criteria = {
    id: '',
    name: '',
    description: '',
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};
