import { Certificate } from './certificate';
import { Contact } from './contact';
import { Education } from './education';
import { ExperienceItem } from './experience';
import { BaseModel } from './interface';
import { UserItem, userItemDefaultValues } from './user';
import { UserSkill } from './userSkill';

export interface CV extends BaseModel {
    id: string;
    name: string;
    title: string;
    about: string;
    status: string;
    avatar: string;
    fullName: string;
    yearsOfWorkingExperience: number;
    isPublic: boolean;
    slug: string;
    layout: Layout;
}

export enum Layout {
    'Simple' = 1,
    'Elegant' = 2,
    'Brutalist' = 3,
}

export interface CVItem extends CV {
    userSkills: UserSkill[];
    experiences: ExperienceItem[];
    certificates: Certificate[];
    educations: Education[];
    contacts: Contact[];
    user: UserItem;
}

export const cvDefaultValues: CV = {
    id: '',
    name: '',
    title: '',
    about: '',
    status: '',
    avatar: '',
    fullName: '',
    slug: '',
    isPublic: false,
    yearsOfWorkingExperience: 0,
    layout: Layout.Simple,
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};

export const cvItemDefaultValues: CVItem = {
    ...cvDefaultValues,
    userSkills: [],
    experiences: [],
    certificates: [],
    educations: [],
    contacts: [],
    user: userItemDefaultValues,
};

export const selectLayoutOptions = [
    {
        value: Layout.Simple,
        label: 'Simple',
        origin: Layout.Simple,
    },
    {
        value: Layout.Elegant,
        label: 'Elegant',
        origin: Layout.Elegant,
    },
    {
        value: Layout.Brutalist,
        label: 'Brutalist',
        origin: Layout.Brutalist,
    },
];

export const LAYOUT_PREVIEW_URL: any = {
    [Layout.Simple]: '/assets/images/layout-cv/simple.png',
    [Layout.Elegant]: '/assets/images/layout-cv/elegant.png',
    [Layout.Brutalist]: '/assets/images/layout-cv/brutalist.png',
};
