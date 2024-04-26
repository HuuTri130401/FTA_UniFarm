import { BaseModel } from './interface';
import { SkillLevel, SkillLevelItem } from './skillLevel';
import { User, userDefaultValues } from './user';

export interface Expert extends BaseModel {
    id: string;
}

export const expertDefaultValues: Expert = {
    id: '',
    createdAt: '',
    updatedAt: '',
    isDeleted: false,
};

export interface ExpertItem extends Expert {
    user: User;
    skillLevels: SkillLevelItem[];
}

export const expertItemDefaultValues: ExpertItem = {
    ...expertDefaultValues,
    user: userDefaultValues,
    skillLevels: [],
};
