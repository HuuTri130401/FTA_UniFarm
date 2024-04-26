import { useTableUtil } from '@context/tableUtilContext';
import {
    interviewApi,
    IV1DelayInterview,
    IV1GetFilterInterview,
    IV1InterviewGetTotalPerDayStatistic,
    IV1InterviewGetTotalStatistic,
    IV1UpdateInterviewCancel,
    IV1UpdateInterviewDelay,
    IV1UpdateInterviewResult,
    IV1UpdateInterviewStatus,
} from '@core/api/interview.api';
import { cvItemDefaultValues } from '@models/cv';
import { InterviewItem, interviewItemDefaultValue } from '@models/interview';
import { store } from '@store/index';
import { userThunk } from '@store/user/thunks';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useInterviewUpdateStatusMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation((data: IV1UpdateInterviewStatus) => {
        return interviewApi.v1PutStatus(data);
    });

    return {
        mutationInterviewUpdateStatus: mutate,
        mutationInterviewUpdateStatusAsync: mutateAsync,
        ...rest,
    };
};

const useInterviewUpdateAcceptStatusMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return interviewApi.v1PutAccept(id);
        },
        {
            onSuccess: () => {
                toast.success('Accept interview successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutationInterviewUpdateAcceptStatus: mutate,
        mutationInterviewUpdateAcceptStatusAsync: mutateAsync,
        ...rest,
    };
};

const useInterviewUpdateCancelStatusMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (dto: IV1UpdateInterviewCancel) => {
            return interviewApi.v1PutCancel(dto);
        },
        {
            onSuccess: () => {
                toast.success('Cancel interview successfully');
                store.dispatch(userThunk.getCurrentUser());
                queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutationInterviewUpdateCancelStatus: mutate,
        mutationInterviewUpdateCancelStatusAsync: mutateAsync,
        ...rest,
    };
};

const useInterviewUpdateDoneStatusMutation = () => {
    // const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return interviewApi.v1PutDone(id);
        },
        {
            onSuccess: () => {
                toast.success('Update done interview successfully');
                // queryClient.invalidateQueries();
            },
        }
    );

    return {
        mutationInterviewUpdateDoneStatus: mutate,
        mutationInterviewUpdateDoneStatusAsync: mutateAsync,
        ...rest,
    };
};

const useInterviewUpdateDelayMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(
        (data: IV1UpdateInterviewDelay) => {
            return interviewApi.v1PutDelay(data);
        },
        {
            onSuccess: () => {
                toast.success('Change Date Interview Success');
            },
        }
    );

    return {
        mutationInterviewUpdateDelay: mutate,
        mutationInterviewUpdateDelayAsync: mutateAsync,
        ...rest,
    };
};

const useInterviewUpdateDelayCandidateMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(
        (data: IV1DelayInterview) => {
            return interviewApi.v1PutDelayCandidate(data);
        },
        {
            onSuccess: () => {
                toast.success('Change Date Interview Success');
            },
        }
    );

    return {
        mutationInterviewUpdateDelayCandidate: mutate,
        mutationInterviewUpdateDelayCandidateAsync: mutateAsync,
        ...rest,
    };
};

const useQueryInterviewById = (id: string) => {
    return useQuery<InterviewItem>(
        ['interview', id],
        async () => {
            return await interviewApi.v1GetId({ id });
        },
        {
            initialData: interviewItemDefaultValue,
            enabled: Boolean(id),
            refetchOnWindowFocus: false,
        }
    );
};

const useQueryInterviewRecordStatus = (id: string) => {
    return useQuery(['interview', id, 'record-status'], async () => {
        return interviewApi.v1GetInterviewRecordStatus(id);
    });
};

const useQueryInterviewAgoraToken = (id: string, role: 1 | 2) => {
    return useQuery(['interview', id, 'agora-token', role], async () => {
        return interviewApi.v1AgoraGetToken(id, role);
    });
};

const useQueryInterviewResult = (id: string) => {
    return useQuery(['interview', id, 'result'], async () => {
        return interviewApi.v1GetResult(id);
    });
};

const useInterviewStartRecordMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation((id: string) => {
        return interviewApi.v1StartInterviewRecord(id);
    });

    return {
        mutationInterviewStartRecord: mutate,
        mutationInterviewStartRecordAsync: mutateAsync,
        ...rest,
    };
};

const useInterviewStopRecordMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation((id: string) => {
        return interviewApi.v1StopInterviewRecord(id);
    });

    return {
        mutationInterviewStopRecord: mutate,
        mutationInterviewStopRecordAsync: mutateAsync,
        ...rest,
    };
};

const useQueryInterviewFilter = (filter: Partial<IV1GetFilterInterview>) => {
    const { setTotalItem } = useTableUtil();

    return useQuery(
        ['interviews', filter],
        async () => {
            const res = await interviewApi.v1GetFilter(filter);

            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useInterviewUpdateResultMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation((data: IV1UpdateInterviewResult) => {
        return interviewApi.v1PutResult(data);
    });

    return {
        mutationInterviewUpdateResult: mutate,
        mutationInterviewUpdateResultAsync: mutateAsync,
        ...rest,
    };
};

const useQueryInterviewCV = (id: string) => {
    return useQuery(
        ['interview', id, 'cv'],
        async () => {
            return interviewApi.v1GetInterviewCV(id);
        },
        {
            initialData: cvItemDefaultValues,
        }
    );
};

const useQueryInterviewPerDay = (dto: IV1InterviewGetTotalPerDayStatistic) => {
    return useQuery(
        ['interview', 'statistic', 'days', dto],
        async () => {
            return interviewApi.v1GetInterviewPerDayStatistic(dto);
        },
        {
            initialData: [],
        }
    );
};

const useQueryTotalInterview = (dto: IV1InterviewGetTotalStatistic) => {
    return useQuery(
        ['interview', 'statistic', dto],
        async () => {
            return interviewApi.v1GetInterviewTotalStatistic(dto);
        },
        {
            initialData: {
                total: 0,
            },
        }
    );
};

const useQueryTotalInterviewStatusCount = () => {
    return useQuery(
        ['interview', 'statistic', 'status-count'],
        async () => {
            return interviewApi.v1StatusCount();
        },
        {
            initialData: [],
        }
    );
};

export {
    useInterviewStartRecordMutation,
    useInterviewStopRecordMutation,
    useInterviewUpdateAcceptStatusMutation,
    useInterviewUpdateCancelStatusMutation,
    useInterviewUpdateDelayCandidateMutation,
    useInterviewUpdateDelayMutation,
    useInterviewUpdateDoneStatusMutation,
    useInterviewUpdateResultMutation,
    useInterviewUpdateStatusMutation,
    useQueryInterviewAgoraToken,
    useQueryInterviewById,
    useQueryInterviewCV,
    useQueryInterviewFilter,
    useQueryInterviewPerDay,
    useQueryInterviewRecordStatus,
    useQueryInterviewResult,
    useQueryTotalInterview,
    useQueryTotalInterviewStatusCount,
};
