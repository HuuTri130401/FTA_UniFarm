import { contactApi } from '@core/api/contact';
import { Contact, contactDefaultValues, ContactFormSchema, ContactUpdate } from '@models/contact';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryContacts = () => {
    return useQuery(['contacts'], async () => await contactApi.v1GetContacts(), {
        initialData: [contactDefaultValues] as Contact[],
    });
};

const useQueryContactTypes = () => {
    return useQuery(['contact-types'], async () => await contactApi.v1GetContactsTypes(), {
        initialData: ['Facebook', 'Email', 'Github'],
    });
};

const useQueryContactsById = (id: string) => {
    return useQuery(['contacts', id], async () => await contactApi.v1GetContactsById(id), {
        initialData: contactDefaultValues as Contact,
        enabled: !!id,
    });
};

const useMutationAddContact = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: ContactFormSchema) => {
            return contactApi.v1PostContacts(input);
        },
        {
            onSuccess: () => {
                toast.success('Contact Add Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateAddContact: mutate, mutateAddContactAsync: mutateAsync, ...rest };
};

const useMutationUpdateContact = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: ContactUpdate) => {
            return contactApi.v1PutContacts(input);
        },
        {
            onSuccess: () => {
                toast.success('Contact Update Success!');
                queryClient.invalidateQueries();
            },
        }
    );
    return { mutateUpdateContact: mutate, mutateUpdateContactAsync: mutateAsync, ...rest };
};

const useMutationDeleteContact = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return contactApi.v1DeleteContacts(id);
        },
        {
            onSuccess: () => {
                toast.success('Contact Delete Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateDeleteContact: mutate, mutateDeleteContactAsync: mutateAsync, ...rest };
};

export { useMutationAddContact, useMutationDeleteContact, useMutationUpdateContact, useQueryContacts, useQueryContactsById, useQueryContactTypes };
