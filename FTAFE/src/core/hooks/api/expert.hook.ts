import { useTableUtil } from '@context/tableUtilContext';
import {
    expertApi,
    IV1ExpertDeleteSkillLevel,
    IV1ExpertGetSkillLevelFilter,
    IV1ExpertUpdateSkillLevel,
    IV1ExpertUpdateSkillLevelApprove,
    IV1ExpertUpdateSkillLevelCancel,
    IV1ExpertUpdateSkillLevelReject,
    IV1GetFilterExpert,
} from '@core/api/expert.api';
import { IV1GetFilterInterview } from '@core/api/interview.api';
import { cvItemDefaultValues } from '@models/cv';
import { expertItemDefaultValues } from '@models/expert';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useExpertFilterInterviews = (filter: Partial<IV1GetFilterInterview>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(['expert-interview', filter], async () => {
        const res = await expertApi.v1GetInterviewsByExpert(filter);
        setTotalItem(res.total);
        return res.data;
    });
};

const useQueryExpertById = (id: string) => {
    return useQuery(
        ['expert', id],
        async () => {
            const res = await expertApi.v1Get(id);
            return res;
        },
        {
            initialData: expertItemDefaultValues,
        }
    );
};

const useExpertUpdateSkillLevelsApproveMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1ExpertUpdateSkillLevelApprove) => {
        const res = await expertApi.v1PutSkillLevelsApprove(input);
        queryClient.invalidateQueries();
        toast.success('Update Expert Skill Levels Success');
        return res;
    });

    return {
        mutateExpertUpdateSkillLevelsApprove: mutate,
        mutateExpertUpdateSkillLevelsApproveAsync: mutateAsync,
        ...rest,
    };
};

const useExpertUpdateSkillLevelsMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1ExpertUpdateSkillLevel) => {
        const res = await expertApi.v1PutSkillLevels(input);
        queryClient.invalidateQueries();
        toast.success('Update Expert Skill Levels Success');
        return res;
    });

    return {
        mutateExpertUpdateSkillLevels: mutate,
        mutateExpertUpdateSkillLevelsAsync: mutateAsync,
        ...rest,
    };
};

const useQueryExpertFilter = (filter: Partial<IV1GetFilterExpert>, isSetTotal: boolean = true) => {
    const { setTotalItem } = useTableUtil();

    return useQuery(
        ['expert', filter],
        async () => {
            const res = await expertApi.v1GetFilter(filter);
            isSetTotal && setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useQueryExpertInterviewFilter = (filter: Partial<IV1GetFilterInterview>) => {
    const { setTotalItem } = useTableUtil();

    return useQuery(
        ['interviews-expert', filter],
        async () => {
            const res = await expertApi.v1GetInterviewFilter(filter);

            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useExpertApproveSkillLevelsMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1ExpertUpdateSkillLevelApprove) => {
        const res = await expertApi.v1PutSkillLevelsApprove(input);

        return res;
    });

    return {
        mutateExpertApproveSkillLevels: mutate,
        mutateExpertApproveSkillLevelsAsync: mutateAsync,
        ...rest,
    };
};

const useExpertRejectSkillLevelsMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1ExpertUpdateSkillLevelReject) => {
        const res = await expertApi.v1PutSkillLevelReject(input);

        return res;
    });

    return {
        mutateExpertRejectSkillLevels: mutate,
        mutateExpertRejectSkillLevelsAsync: mutateAsync,
        ...rest,
    };
};

const useExpertCancelSkillLevelsMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1ExpertUpdateSkillLevelCancel) => {
        const res = await expertApi.v1PutSkillLevelCancel(input);
        return res;
    });

    return {
        mutateExpertCancelSkillLevels: mutate,
        mutateExpertCancelSkillLevelsAsync: mutateAsync,
        ...rest,
    };
};

const useQueryExpertCV = (id: string) => {
    return useQuery(
        ['expert-cv', id],
        async () => {
            const res = await expertApi.v1GetExpertCV(id);
            return res;
        },
        {
            initialData: cvItemDefaultValues,
        }
    );
};

const useQueryExpertGetSkillLevelRequestFilter = (filter: Partial<IV1ExpertGetSkillLevelFilter>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['skill-level-approve-request', filter],
        async () => {
            const res = await expertApi.v1GetExpertFilterRequestSkillLevel(filter);
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useExpertDeleteSkillLevelMutation = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(async (input: IV1ExpertDeleteSkillLevel) => {
        const res = await expertApi.v1DeleteSkillLevel(input);
        queryClient.invalidateQueries();
        toast.success('Delete Expert Skill Levels Success');
        return res;
    });

    return {
        mutateExpertDeleteSkillLevel: mutate,
        mutateExpertDeleteSkillLevelAsync: mutateAsync,
        ...rest,
    };
};

export {
    useExpertApproveSkillLevelsMutation,
    useExpertCancelSkillLevelsMutation,
    useExpertDeleteSkillLevelMutation,
    useExpertFilterInterviews,
    useExpertRejectSkillLevelsMutation,
    useExpertUpdateSkillLevelsApproveMutation,
    useExpertUpdateSkillLevelsMutation,
    useQueryExpertById,
    useQueryExpertCV,
    useQueryExpertFilter,
    useQueryExpertGetSkillLevelRequestFilter,
    useQueryExpertInterviewFilter,
};
