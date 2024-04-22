import { BaseModel } from './interface';
import { Skill } from './skill';

export interface SkillGroup extends BaseModel {
    id: string;
    name: string;
}

export const skillGroupDefaultValues: SkillGroup = {
    id: '',
    name: '',
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};

export interface SkillGroupItem extends SkillGroup {
    skills: Skill[];
}

export const skillGroupItemDefaultValues: SkillGroupItem = {
    id: '',
    name: '',
    skills: [],
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};
