export interface CollectedHub {
    id: string;
    code: string;
    name: string;
    description: string;
    image: string;
    address: string;
    status: string;
    createdAt: string;
    updatedAt: string;
}
export interface Station {
    id: string;
    areaId: string;
    code: string;
    name: string;
    description: string;
    image: string;
    address: string;
    status: string;
    area: any | null;
    createdAt: string;
    updatedAt: string;
}
export interface Staff {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    avatar: string | null;
    code: string | null;
    address: string | null;
    createdAt: string | null;
    updatedAt: string;
}
