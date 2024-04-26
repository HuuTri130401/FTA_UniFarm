import { FarmHub } from './user';

export type UpdateFarmHubForm = Omit<FarmHub, 'id' | 'createdAt'>;
export type CreateFarmHubForm = Pick<FarmHub, 'name' | 'code' | 'description' | 'image' | 'address'>;

export type CreateFarmHubFormData = {
    FirstName: string | null;
    LastName: string | null;
    FarmHubName: string;
    FarmHubCode: string;
    FarmHubAddress: string;
    UserName: string | null;
    PhoneNumber: string;
    Password: string;
    Email: string;
    Description: string | null;
    FarmHubImage: Blob | null;
};
