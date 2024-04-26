import { BaseModel } from './interface';

export interface Education extends BaseModel {
    id: string;
    title: string;
    subTitle: string;
    location: string;
    description: string;
    startTime: string;
    endTime: string | null;
}

export interface EducationForm extends Pick<Education, 'title' | 'subTitle' | 'location' | 'description' | 'startTime' | 'endTime'> {}

export interface EducationFormUpdate extends Pick<Education, 'id' | 'title' | 'subTitle' | 'location' | 'description' | 'startTime' | 'endTime'> {}

export const educationDefaultValues: Education = {
    id: '',
    title: '',
    subTitle: '',
    location: '',
    description: '',
    startTime: '',
    endTime: '',
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};
