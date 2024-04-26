import { criteriaApi } from '@core/api/criteria.api';
import { criteriaDefaultValues } from '@models/criteria';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryCriteriaById = (id: string) => {
    return useQuery(['criteria', id], async () => await criteriaApi.v1GetId(id), {
        initialData: criteriaDefaultValues,
    });
};

const useUpdateCriteriaMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(criteriaApi.v1Put, {
        onSuccess: () => {
            toast.success('Criteria update success!');
            queryClient.invalidateQueries();
        },
    });

    return { mutateUpdateCriteria: mutate, mutateUpdateCriteriaAsync: mutateAsync, ...rest };
};
export { useQueryCriteriaById, useUpdateCriteriaMutation };
