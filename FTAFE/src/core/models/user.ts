import { BaseModel, IOption } from './interface';
import { CollectedHub } from './staff';
import { Wallet } from './wallet';

export enum UserRole {
    ADMIN = 'Admin',
    GUESS = 'Guess',
    COLLECTED_STAFF = 'CollectedStaff',
    CUSTOMER = 'Customer',
    // DELIVERED_STAFF = 'Delivered_staff',
    FARM_HUB = 'FarmHub',
    STATION_STAFF = 'StationStaff',
}

export enum UserStatus {
    ACTIVE = 'ACTIVE',
    INACTIVE = 'INACTIVE',
}

export interface User extends BaseModel {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    avatar: string | null;
    code: string | null;
    address: string | null;
    createdAt: string;
    updatedAt: string | null;
    roleName: UserRole;
    wallet: NewWallet | null;
    farmHub: FarmHub | null;
    collectedHub: CollectedHub | null;
}
export interface NewWallet {
    id: string;
    accountId: string;
    balance: number;
    createdAt: string;
    updatedAt: string | null;
    status: string;
    account: any;
    payments: [];
    transactions: [];
}

export const userDefaultValues: User = {
    id: '',
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    avatar: null,
    code: null,
    address: null,
    createdAt: '',
    updatedAt: '',
    isDeleted: false,
    roleName: UserRole.GUESS,
    farmHub: null,
    wallet: null,
    collectedHub: null,
};

export interface UserItem extends User {
    wallet: Wallet | null;
}

export const userItemDefaultValues: UserItem = {
    ...userDefaultValues,
    wallet: null,
};

export const optionsUserStatus: IOption[] = [
    {
        value: UserStatus.ACTIVE,
        label: 'ACTIVE',
        origin: UserStatus.ACTIVE,
    },
    {
        value: UserStatus.INACTIVE,
        label: 'INACTIVE',
        origin: UserStatus.INACTIVE,
    },
];

export const colorsUserStatus: IOption[] = [
    { value: UserStatus.ACTIVE, label: 'ACTIVE', origin: 'green' },
    { value: UserStatus.INACTIVE, label: 'INACTIVE', origin: 'red' },
];

export const colorsUserRole: IOption[] = [{ value: UserRole.ADMIN, label: 'ADMIN', origin: 'green' }];

export interface FarmHub {
    id: string;
    name: string;
    code: string;
    description: string;
    image: string;
    address: string;
    createdAt: string;
    updatedAt: string;
    status: string;
    roleName: UserRole.FARM_HUB;
}
