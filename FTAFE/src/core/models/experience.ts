import { BaseModel } from './interface';

export interface Experience extends BaseModel {
    id: string;
    title: string;
    jobPosition: string;
    description: string;
    startTime: string;
    endTime: string | null;
    company: string | null;
    type: string;
}

export interface Project extends BaseModel {
    id: string | null;
    name: string;
    description: string;
    startTime: string | undefined;
    endTime: string | null | undefined;
    linkProject: string | null;
    role: string;
    skills: string;
}

export interface ProjectForm extends Pick<Project, 'name' | 'description' | 'startTime' | 'endTime' | 'linkProject' | 'role' | 'skills'> {
    isCurrentProject?: boolean;
}

export interface ExperienceForm extends Pick<Experience, 'title' | 'jobPosition' | 'description' | 'startTime' | 'endTime' | 'company' | 'type'> {
    projects: ProjectForm[];
}

export interface ProjectFormSchema {
    projects: ProjectForm[];
}

export interface ExperienceItem extends Experience {
    projects: Project[];
}

export const experienceDefaultValues: ExperienceItem = {
    id: '',
    title: '',
    jobPosition: '',
    description: '',
    startTime: '',
    endTime: '',
    company: '',
    type: '',
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
    projects: [],
};

export interface ExperienceFormUpdate
    extends Pick<Experience, 'id' | 'title' | 'jobPosition' | 'description' | 'startTime' | 'endTime' | 'company' | 'type'> {
    projects: ProjectFormUpdate[];
}

export interface ProjectFormUpdate
    extends Pick<Project, 'id' | 'name' | 'description' | 'startTime' | 'endTime' | 'linkProject' | 'role' | 'skills'> {
    isCurrentProject?: boolean;
}

export interface ExperienceSchemaUpdate extends ExperienceFormUpdate {
    projects: ProjectFormUpdate[];
}
