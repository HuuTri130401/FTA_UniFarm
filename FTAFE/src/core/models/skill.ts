import { BaseModel } from './interface';
import { SkillGroup, skillGroupDefaultValues } from './skillGroup';
import { SkillLevel } from './skillLevel';

export interface Skill extends BaseModel {
    id: string;
    name: string;
    description: string;
    status: string;
}

export interface SkillItem extends Skill {
    skillLevels: SkillLevel[];
    skillGroup: SkillGroup;
}

export const skillDefaultValues: Skill = {
    id: '',
    name: '',
    status: '',
    createdAt: '',
    updatedAt: '',
    description: '',
    isDeleted: false,
};

export const skillItemDefaultValues: SkillItem = {
    ...skillDefaultValues,
    skillLevels: [],
    skillGroup: skillGroupDefaultValues,
};
