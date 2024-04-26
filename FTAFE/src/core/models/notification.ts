import { BaseModel } from './interface';

export interface Notifications extends BaseModel {
    id: string;
    title: string;
    body: string;
}
