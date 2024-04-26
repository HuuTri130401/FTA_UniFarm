import { BaseModel } from './interface';

export interface Certificate extends BaseModel {
    id: string;
    name: string;
    referenceLink: string;
    validFrom: string;
    validTo: string;
}

export interface CertificateForm extends Pick<Certificate, 'name' | 'referenceLink' | 'validFrom' | 'validTo'> {}

export interface CertificateFormUpdate extends Pick<Certificate, 'id' | 'name' | 'referenceLink' | 'validFrom' | 'validTo'> {}

export const certificateDefaultValues: Certificate = {
    id: '',
    name: '',
    referenceLink: '',
    validFrom: '',
    validTo: '',
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};

export const certificateDefaultValuesUpdate = {
    id: '',
    name: '',
    referenceLink: '',
    validFrom: '',
    validTo: '',
};
