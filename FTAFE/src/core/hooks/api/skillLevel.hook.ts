import { useTableUtil } from '@context/tableUtilContext';
import { IV1GetSkillLevelApproveRequestFilter, IV1GetSkillLevelFilter, IV1UpdateSkillLevel, skillLevelApi } from '@core/api/skill-level.api';
import { skillLevelDefaultValues } from '@models/skillLevel';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQuerySkillLevelById = (id: string) => {
    return useQuery(
        ['skill-level', id],
        async () => {
            return skillLevelApi.v1GetId(id);
        },
        {
            enabled: !!id,
            initialData: skillLevelDefaultValues,
        }
    );
};

const useSkillLevelUpdateMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1UpdateSkillLevel) => {
            return skillLevelApi.v1Put(input);
        },
        {
            onSuccess: (data) => {
                toast.success('Update skill level successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateUpdateSkillLevel: mutate, mutateUpdateSkillLevelAsync: mutateAsync, ...rest };
};

const useQuerySkillLevelFilter = (filter: IV1GetSkillLevelFilter) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['skill-level', filter],
        async () => {
            const res = await skillLevelApi.v1GetFilter(filter);
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};
const useQuerySkillLevelApproveRequestFilter = (filter: IV1GetSkillLevelApproveRequestFilter) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['skill-level-approve-request', filter],
        async () => {
            const res = await skillLevelApi.v1GetFilterApproveRequest(filter);
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

export { useQuerySkillLevelApproveRequestFilter, useQuerySkillLevelById, useQuerySkillLevelFilter, useSkillLevelUpdateMutation };
