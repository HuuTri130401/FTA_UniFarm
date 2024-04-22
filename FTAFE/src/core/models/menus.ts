export interface Menus {
    id: string;
    farmHubId: string;
    businessDayId: string;
    name: string;
    createdAt: string;
    updatedAt: string;
    tag: string;
    status: string;
}

export type UpdateMenus = Pick<Menus, 'farmHubId' | 'businessDayId' | 'name' | 'tag' | 'status' | 'id'>;

export type CreateMenus = Pick<Menus, 'farmHubId' | 'name' | 'tag'>;
