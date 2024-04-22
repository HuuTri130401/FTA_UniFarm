import { PagingProps } from './interface';

export type Apartment = {
    id: string;
    areaId: string;
    name: string;
    code: string;
    address: string;
    status: string;
    createdAt: string;
    updatedAt: string | null;
};
export interface CreateApartmentForm extends Omit<Apartment, 'createdAt' | 'updatedAt'> {}
export interface UpdateApartmentForm extends Omit<Apartment, 'updatedAt' | 'createdAt'> {}
export interface ApartmentFilter extends PagingProps, Pick<Apartment, 'address' | 'name'> {}
