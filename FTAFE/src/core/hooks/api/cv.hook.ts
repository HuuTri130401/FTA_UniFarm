import { cvApi, IV1AddCV, IV1CVCreateInterview, IV1CVGetTotalStatistic, IV1CVPublicCV, IV1UpdateCV } from '@core/api/cv';
import { cvItemDefaultValues } from '@models/cv';
import { store } from '@store/index';
import { userThunk } from '@store/user/thunks';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryCVs = () => {
    return useQuery(['cvs'], async () => await cvApi.v1GetCVs(), {
        initialData: [],
    });
};

const useQueryCVById = (id: string) => {
    return useQuery(['cv', id], async () => await cvApi.v1GetCvById(id), {
        initialData: cvItemDefaultValues,
        enabled: !!id,
    });
};

const useQueryCVBySlug = (slug: string) => {
    return useQuery(['cv', slug], async () => await cvApi.v1GetCvBySlug(slug), {
        initialData: cvItemDefaultValues,
        enabled: !!slug,
    });
};

const useMutationCreateCV = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: IV1AddCV) => {
            return cvApi.v1PostCV(input);
        },
        {
            onSuccess: () => {
                toast.success('CV Create Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateCreateCV: mutate, mutateCreateCVAsync: mutateAsync, ...rest };
};

const useMutationUpdateCV = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: IV1UpdateCV) => {
            return cvApi.v1PutCV(input);
        },
        {
            onSuccess: () => {
                toast.success('CV Update Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateUpdateCV: mutate, mutateUpdateCVAsync: mutateAsync, ...rest };
};

const useMutationDeleteCV = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return cvApi.v1DeleteCV(id);
        },
        {
            onSuccess: () => {
                toast.success('CV Delete Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateDeleteCV: mutate, mutateDeleteCVAsync: mutateAsync, ...rest };
};

const useMutationCVCreateInterview = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: IV1CVCreateInterview) => {
            return cvApi.v1PostInterview(input);
        },
        {
            onSuccess: () => {
                toast.success('Interview Create Success!');
                store.dispatch(userThunk.getCurrentUser());
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateCreateInterview: mutate, mutateCreateInterviewAsync: mutateAsync, ...rest };
};

const useMutationCVUpdateIsPublic = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: IV1CVPublicCV) => {
            return cvApi.v1PutIsPublic(input);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries();
                toast.success('Update CV success!');
            },
        }
    );
    return {
        mutateUpdateIsPublic: mutate,
        mutateUpdateIsPublicAsync: mutateAsync,
        ...rest,
    };
};

// NOTE Statistic

const useQueryTotalCV = (dto: IV1CVGetTotalStatistic) => {
    return useQuery(['total-cv', dto], async () => await cvApi.v1GetTotalCV(dto), {
        initialData: {
            total: 0,
        },
    });
};

export {
    useMutationCreateCV,
    useMutationCVCreateInterview,
    useMutationCVUpdateIsPublic,
    useMutationDeleteCV,
    useMutationUpdateCV,
    useQueryCVById,
    useQueryCVBySlug,
    useQueryCVs,
    useQueryTotalCV,
};
