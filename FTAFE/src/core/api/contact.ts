import { Contact, ContactFormSchema, ContactUpdate } from '@models/contact';
import _get from 'lodash.get';

import { http } from './http';

export const contactApi = {
    v1GetContacts: async () => {
        const res = await http.get('/user/contacts');
        return _get(res, 'data') as Contact[];
    },

    v1PostContacts: async (data: ContactFormSchema) => {
        const res = await http.post('/contacts', data);
        return res;
    },

    v1GetContactsTypes: async () => {
        const res = await http.get('/contacts/types');
        return _get(res, 'data');
    },

    v1PutContacts: async (input: ContactUpdate) => {
        const { id, ...rest } = input;
        const res = await http.put(`/contacts/${id}`, rest);
        return _get(res, 'data') as Contact;
    },

    v1DeleteContacts: async (id: string) => {
        const res = await http.delete(`/contacts/${id}`);
        return res;
    },

    v1GetContactsById: async (id: string) => {
        const res = await http.get(`contacts/${id}`);
        return _get(res, 'data') as Contact;
    },
};
