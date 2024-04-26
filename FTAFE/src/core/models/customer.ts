import { PagingProps } from './interface';

export type Customer = {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string | null;
    avatar: string | null;
    code: string | null;
    address: string | null;
    createdAt: string | null;
    updatedAt: string | null;
    collectedHub: any | null;
    farmHub: any | null;
    station: any | null;
};

export interface CustomerFilter extends PagingProps, Pick<Customer, 'firstName' | 'lastName' | 'phoneNumber' | 'email'> {}
