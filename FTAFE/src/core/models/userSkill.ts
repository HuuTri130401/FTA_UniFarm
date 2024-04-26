import { BaseModel } from './interface';

export interface UserSkill extends BaseModel {
    id: string;
    name: string;
}

export interface UserSkillForm extends Pick<UserSkill, 'name'> {}

export interface UserSkillFormUpdate extends Pick<UserSkill, 'id' | 'name'> {}

export const userSkillDefaultValues = {
    id: '',
    name: '',
};
