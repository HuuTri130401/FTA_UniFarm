import { PagingProps } from './interface';

export type Area = {
    id: string;
    province: string;
    district: string;
    commune: string;
    address: string;
    status: string;
    code: string;
};
export interface CreateAreaForm extends Area {}

export interface UpdateAreaForm extends Area {}

export interface AreaFilter extends PagingProps, Pick<Area, 'address' | 'commune' | 'district' | 'province'> {}
