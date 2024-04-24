import { criteriaResultApi, IV1UpdateCriteriaResult } from '@core/api/criteria-result.api';
import { useMutation, useQueryClient } from '@tanstack/react-query';

const useCriteriaResultUpdateResultMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(async (data: IV1UpdateCriteriaResult) => {
        const res = criteriaResultApi.v1PutCriteriaResult(data);
        return res;
    });

    return {
        mutationCriteriaResultUpdateResult: mutate,
        mutationCriteriaResultUpdateResultAsync: mutateAsync,
        ...rest,
    };
};

export { useCriteriaResultUpdateResultMutation };
