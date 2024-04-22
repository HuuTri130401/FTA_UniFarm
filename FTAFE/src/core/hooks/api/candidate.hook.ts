import { useTableUtil } from '@context/tableUtilContext';
import { candidateApi, IV1CandidateFilterInterview, IV1CandidateGetTotalStatistic, IV1GetFilterCandidate } from '@core/api/candidate';
import { candidateItemDefaultValue } from '@models/candidate';
import { useQuery, useQueryClient } from '@tanstack/react-query';

const useQueryCandidateFilterInterview = (filter: Partial<IV1CandidateFilterInterview>) => {
    const { setTotalItem } = useTableUtil();
    return null;
};

const useCandidatePutProfileMutation = () => {
    const queryClient = useQueryClient();
    // const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1CandidateUpdateProfile) => {
    //     const res = await candidateApi.v1UpdateCandidateProfile(input);
    //     store.dispatch(userThunk.getCurrentUser());
    //     queryClient.invalidateQueries();
    //     toast.success('Update profile successfully');
    //     return res;
    // });

    return null;
};

const useQueryTotalCandidate = (dto: IV1CandidateGetTotalStatistic) => {
    return useQuery(
        ['candidate', 'statistic', dto],
        async () => {
            const res = await candidateApi.v1GetGeCandidateTotalStatistic(dto);
            return res;
        },
        {
            initialData: {
                total: 0,
            },
        }
    );
};

const useQueryCandidateFilter = (filter: Partial<IV1GetFilterCandidate>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['candidate', 'filter', filter],
        async () => {
            const res = await candidateApi.v1GetFilterCandidate(filter);

            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useQueryCandidateById = (id: string) => {
    return useQuery(
        ['candidate', 'id', id],
        async () => {
            const res = await candidateApi.v1GetCandidateById(id);
            return res;
        },
        {
            initialData: candidateItemDefaultValue,
        }
    );
};

export { useCandidatePutProfileMutation, useQueryCandidateById, useQueryCandidateFilter, useQueryCandidateFilterInterview, useQueryTotalCandidate };
