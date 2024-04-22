import { educationApi } from '@core/api/education';
import { Education, educationDefaultValues, EducationForm, EducationFormUpdate } from '@models/education';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryEducations = () => {
    return useQuery<Education[]>(['educations'], async () => await educationApi.v1GetEducations(), {
        initialData: [],
    });
};

const useQueryEducationsById = (id: string) => {
    return useQuery(['educations', id], async () => await educationApi.v1GetEducationsById(id), {
        initialData: educationDefaultValues,
        enabled: !!id,
    });
};

const useMutationAddEducation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: EducationForm) => {
            return educationApi.v1PostEducation(input);
        },
        {
            onSuccess: () => {
                toast.success('Education Add Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateAddEducation: mutate, mutateAddEducationAsync: mutateAsync, ...rest };
};

const useMutationUpdateEducation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: EducationFormUpdate) => {
            return educationApi.v1PutEducation(input);
        },
        {
            onSuccess: () => {
                toast.success('Education Update Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateUpdateEducation: mutate, mutateUpdateEducationAsync: mutateAsync, ...rest };
};

const useMutationDeleteEducation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return educationApi.v1DeleteEducation(id);
        },
        {
            onSuccess: () => {
                toast.success('Education Delete Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateDeleteEducation: mutate, mutateDeleteEducationAsync: mutateAsync, ...rest };
};

export { useMutationAddEducation, useMutationDeleteEducation, useMutationUpdateEducation, useQueryEducations, useQueryEducationsById };
