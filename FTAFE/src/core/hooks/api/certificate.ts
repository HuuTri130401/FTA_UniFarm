import { certificateApi } from '@core/api/certificate';
import { certificateDefaultValues, CertificateForm, CertificateFormUpdate } from '@models/certificate';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryCertificates = () => {
    return useQuery(['certificates'], async () => await certificateApi.v1GetCertificates(), {
        initialData: [],
    });
};

const useQueryCertificatesById = (id: string) => {
    return useQuery(['certificates', id], async () => await certificateApi.v1GetCertificatesById(id), {
        initialData: certificateDefaultValues,
        enabled: !!id,
    });
};

const useMutationAddCertificate = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: CertificateForm) => {
            return certificateApi.v1PostCertificate(input);
        },
        {
            onSuccess: () => {
                toast.success('Certificate Add Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateAddCertificate: mutate, mutateAddCertificateAsync: mutateAsync, ...rest };
};

const useMutationUpdateCertificate = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: CertificateFormUpdate) => {
            return certificateApi.v1PutCertificate(input);
        },
        {
            onSuccess: () => {
                toast.success('Certificate Update Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateUpdateCertificate: mutate, mutateUpdateCertificateAsync: mutateAsync, ...rest };
};

const useMutationDeleteCertificate = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return certificateApi.v1DeleteCertificate(id);
        },
        {
            onSuccess: () => {
                toast.success('Certificate Delete Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateDeleteCertificate: mutate, mutateDeleteCertificateAsync: mutateAsync, ...rest };
};

export { useMutationAddCertificate, useMutationDeleteCertificate, useMutationUpdateCertificate, useQueryCertificates, useQueryCertificatesById };
