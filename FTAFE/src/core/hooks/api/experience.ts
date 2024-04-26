import { experienceApi } from '@core/api/experience';
import { experienceDefaultValues, ExperienceForm, ExperienceFormUpdate } from '@models/experience';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryExperiences = () => {
    return useQuery(['experience'], async () => await experienceApi.v1GetExperience(), {
        initialData: [],
    });
};

const useQueryExperienceById = (id: string) => {
    return useQuery(['experience', id], async () => await experienceApi.v1GetExperienceById(id), {
        initialData: experienceDefaultValues,
        enabled: !!id,
    });
};

const useQueryExperienceTypes = () => {
    return useQuery(['experience', 'types'], async () => await experienceApi.v1GetExperienceTypes(), {
        initialData: [],
    });
};

const useMutationAddExperience = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: ExperienceForm) => {
            return experienceApi.v1PostExperience(input);
        },
        {
            onSuccess: () => {
                toast.success('Experience Add Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateAddExperience: mutate, mutateAddExperienceAsync: mutateAsync, ...rest };
};

const useMutationUpdateExperience = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: ExperienceFormUpdate) => {
            return experienceApi.v1PutExperience(input);
        },
        {
            onSuccess: () => {
                toast.success('Experience Update Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateUpdateExperience: mutate, mutateUpdateExperienceAsync: mutateAsync, ...rest };
};

const useMutationDeleteExperience = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return experienceApi.v1DeleteExperience(id);
        },
        {
            onSuccess: () => {
                toast.success('Experience Delete Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateDeleteExperience: mutate, mutateDeleteExperienceAsync: mutateAsync, ...rest };
};

export {
    useMutationAddExperience,
    useMutationDeleteExperience,
    useMutationUpdateExperience,
    useQueryExperienceById,
    useQueryExperiences,
    useQueryExperienceTypes,
};
