import { ExpertItem } from './expert';
import { BaseModel } from './interface';
import { SkillItem } from './skill';

export interface SkillLevel extends BaseModel {
    id: string;
    name: string;
    weight: number;
}

export interface SkillLevelItem extends SkillLevel {
    skill: SkillItem;
}

export const skillLevelDefaultValues: SkillLevel = {
    id: '',
    name: '',
    weight: 0,
    createdAt: '',
    updatedAt: '',
    isDeleted: false,
};

export interface SkillLevelApproveRequest extends BaseModel {
    expert: ExpertItem;
    id: string;
    currentSkillLevel: SkillLevelItem[];
    requestSkillLevel: SkillLevelItem;
}
