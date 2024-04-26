import { BaseModel } from './interface';

export enum ContactType {
    Email = 'Email',
    Phone = 'Phone',
    Address = 'Address',
    Facebook = 'Facebook',
    Twitter = 'Twitter',
    Github = 'Github',
    Linkedin = 'Linkedin',
}

export interface Contact extends BaseModel {
    id: string;
    value: string;
    type: string;
}

export interface ContactForm {
    value: string;
    type: string;
}

export interface ContactUpdate extends Pick<Contact, 'id' | 'value' | 'type'> {}

export const contactFormDefaultValues = {
    value: '',
    type: '',
};

export interface ContactFormSchema {
    contacts: ContactForm[];
}
export const contactDefaultValues = {
    id: '',
    value: '',
    type: '',
};
