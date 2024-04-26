import { useTableUtil } from '@context/tableUtilContext';
import { IV1GetFilterSkill, IV1SkillPostSkillLevel, IV1UpdateSkill, skillApi } from '@core/api/skill.api';
import { skillDefaultValues, SkillItem, skillItemDefaultValues } from '@models/skill';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useSkillCreateSkillLevelMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1SkillPostSkillLevel) => {
            return skillApi.v1SkillPostSkillLevel(input);
        },
        {
            onSuccess: (data) => {
                toast.success('Create skill levels successfully');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateCreateSkillLevel: mutate, mutateCreateSkillLevelAsync: mutateAsync, ...rest };
};

const useQuerySkillFilter = (filter: Partial<IV1GetFilterSkill>, isSetTotal: boolean = true) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['skills', filter],
        async () => {
            const res = await skillApi.v1GetFilter(filter);
            if (isSetTotal) setTotalItem(res.total);

            return res.data;
        },
        {
            initialData: [],
        }
    );
};

const useSkillEnableMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async ({ enable, id }: { id: string; enable: boolean }) => {
            return skillApi.v1PutEnable(id, enable);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries(['skill', 'skills']);
                toast.success('Update display status successfully');
            },
        }
    );

    return { mutateEnableSkill: mutate, mutateEnableSkillAsync: mutateAsync, ...rest };
};

const useSkillUpdateMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1UpdateSkill) => {
            return skillApi.v1Put(input);
        },
        {
            onSuccess: () => {
                queryClient.invalidateQueries();
                toast.success('Update skill successfully');
            },
        }
    );

    return { mutateUpdateSkill: mutate, mutateUpdateSkillAsync: mutateAsync, ...rest };
};

const useQuerySKillById = (id: string) => {
    return useQuery(
        ['skill', id],
        async () => {
            const res = await skillApi.v1GetId({ id });
            return res;
        },
        {
            initialData: skillItemDefaultValues,
        }
    );
};

export { useQuerySKillById, useQuerySkillFilter, useSkillCreateSkillLevelMutation, useSkillEnableMutation, useSkillUpdateMutation };
