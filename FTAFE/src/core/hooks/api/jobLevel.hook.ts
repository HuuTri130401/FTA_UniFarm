import { useTableUtil } from '@context/tableUtilContext';
import { IV1JobLevelCreateCriterias, IV1JobLevelCreateInterview, IV1UpdateJobLevel, jobLevelApi } from '@core/api/job-level.api';
import { IV1GetFilterSkill } from '@core/api/skill.api';
import { jobLevelItemDefaultValue } from '@models/jobLevel';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryJobLevelById = (id: string) => {
    return useQuery(['job-level', id], async () => await jobLevelApi.v1GetId(id), {
        initialData: jobLevelItemDefaultValue,
    });
};

const useJobLevelCreateCriteriasMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1JobLevelCreateCriterias) => {
            return await jobLevelApi.v1PostCriterias(input);
        },
        {
            onSuccess: () => {
                toast.success('Create criteria successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutateCreateCriterias: mutate,
        mutateCreateCriteriasAsync: mutateAsync,
        ...rest,
    };
};

const useUpdateJobLevelMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1UpdateJobLevel) => {
            return await jobLevelApi.v1Put(input);
        },
        {
            onSuccess: () => {
                toast.success('Update job level successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutateUpdateJobLevel: mutate,
        mutateUpdateJobLevelAsync: mutateAsync,
        ...rest,
    };
};

const useQueryJobLevelFilter = (filter: Partial<IV1GetFilterSkill>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['job-levels', filter],
        async () => {
            const res = await jobLevelApi.v1GetFilter(filter);
            setTotalItem(res.total);
            return res;
        },
        {
            initialData: {
                total: 0,
                data: [],
            },
        }
    );
};

const useJobLevelCreateInterviewMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1JobLevelCreateInterview) => {
            return await jobLevelApi.v1PostInterview(input);
        },
        {
            onSuccess: () => {
                toast.success('Create interview successfully');
            },
        }
    );

    return {
        mutateCreateInterview: mutate,
        mutateCreateInterviewAsync: mutateAsync,
        ...rest,
    };
};

const useQueryJobLevelGetExpert = (id: string) => {
    return useQuery(
        ['job-level-expert', id],
        async () => {
            const res = await jobLevelApi.v1GetExpert(id);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useMutationJobLevelUpdateEnable = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: { id: string; enable: boolean }) => {
            return await jobLevelApi.v1PutEnable(input);
        },
        {
            onSuccess: () => {
                toast.success('Update job level successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutateJobLevelUpdateEnable: mutate,
        mutateJobLevelUpdateEnableAsync: mutateAsync,
        ...rest,
    };
};

const useJobLevelDeleteByIdMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (id: string) => {
            return await jobLevelApi.v1Delete(id);
        },
        {
            onSuccess: () => {
                toast.success('Delete job level successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutateDeleteJobLevel: mutate,
        mutateDeleteJobLevelAsync: mutateAsync,
        ...rest,
    };
};

export {
    useJobLevelCreateCriteriasMutation,
    useJobLevelCreateInterviewMutation,
    useJobLevelDeleteByIdMutation,
    useMutationJobLevelUpdateEnable,
    useQueryJobLevelById,
    useQueryJobLevelFilter,
    useQueryJobLevelGetExpert,
    useUpdateJobLevelMutation,
};
