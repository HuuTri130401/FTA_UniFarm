import { useTableUtil } from '@context/tableUtilContext';
import { IV1AddJob, IV1GetFilterJob, IV1JobCreateJobLevel, jobApi } from '@core/api/job.api';
import { JobItem, jobItemDefaultValues } from '@models/job';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryJobById = (id: string) => {
    return useQuery<JobItem>(
        ['job', id],
        async () => {
            return jobApi.v1GetId({ id });
        },
        {
            initialData: jobItemDefaultValues,
        }
    );
};

const useQueryJobFilter = (filter: Partial<IV1GetFilterJob>) => {
    const { setTotalItem } = useTableUtil();

    return useQuery(
        ['jobs', filter],
        async () => {
            const { data, total } = await jobApi.v1GetFilter(filter);
            setTotalItem(total);
            return data;
        },
        {
            initialData: [],
        }
    );
};

const useJobCreateMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1AddJob) => {
            return jobApi.v1Post(input);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries(['jobs']);
                toast.success('Create job successfully');
            },
        }
    );

    return { mutateJobCreate: mutate, mutateJobCreateAsync: mutateAsync, ...rest };
};

const useJobUpdateMutation = (id: string) => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1AddJob) => {
            return jobApi.v1Put(id, input);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries(['jobs', 'job', id]);
                toast.success('Update job successfully');
            },
        }
    );

    return { mutateJobUpdate: mutate, mutateJobUpdateAsync: mutateAsync, ...rest };
};

const useJobEnableMutation = (id: string) => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (enable: boolean) => {
            return jobApi.v1PutEnable(id, enable);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries(['jobs', 'job', id]);
                toast.success('Update job successfully');
            },
        }
    );

    return { mutateJobEnable: mutate, mutateJobEnableAsync: mutateAsync, ...rest };
};

const useJobCreateJobLevelMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1JobCreateJobLevel) => {
            return await jobApi.v1PostJobCreateJobLevel(input);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries();
                toast.success('Create Job Level Success.');
            },
        }
    );

    return { mutateJobCreateInterview: mutate, mutateJobCreateInterviewAsync: mutateAsync, ...rest };
};

const useJobDeleteByIdMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        async (id: string) => {
            return await jobApi.v1Delete(id);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries();
                toast.success('Delete Job Success.');
            },
        }
    );

    return { mutateJobDelete: mutate, mutateJobDeleteAsync: mutateAsync, ...rest };
};

export {
    useJobCreateJobLevelMutation,
    useJobCreateMutation,
    useJobDeleteByIdMutation,
    useJobEnableMutation,
    useJobUpdateMutation,
    useQueryJobById,
    useQueryJobFilter,
};
