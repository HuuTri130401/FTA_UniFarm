import { Certificate, CertificateForm, CertificateFormUpdate } from '@models/certificate';
import _get from 'lodash.get';

import { http } from './http';

export const certificateApi = {
    v1GetCertificates: async () => {
        const res = await http.get('/user/certificates');
        return _get(res, 'data') as Certificate[];
    },
    v1PostCertificate: async (data: CertificateForm) => {
        const res = await http.post('/certificates', data);
        return res;
    },

    v1PutCertificate: async (data: CertificateFormUpdate) => {
        const { id, ...rest } = data;
        const res = await http.put(`/certificates/${id}`, rest);
        return _get(res, 'data') as Certificate;
    },
    v1DeleteCertificate: async (id: string) => {
        const res = await http.delete(`/certificates/${id}`);
        return res;
    },

    v1GetCertificatesById: async (id: string) => {
        const res = await http.get(`/certificates/${id}`);
        return _get(res, 'data') as Certificate;
    },
};
