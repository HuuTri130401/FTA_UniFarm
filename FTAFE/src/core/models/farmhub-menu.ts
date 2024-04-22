export type FarmHubMenu = {
    id: string;
    farmHubId: string;
    businessDayId: string;
    name: string;
    createdAt: string;
    updatedAt: string;
    tag: string;
    status: string;
};
export interface CreateMenu extends Pick<FarmHubMenu, 'name' | 'tag'> {}
export interface UpdateMenu extends Pick<FarmHubMenu, 'farmHubId' | 'businessDayId' | 'name' | 'tag' | 'status'> {}
