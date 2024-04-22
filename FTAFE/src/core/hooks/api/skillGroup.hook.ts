import { useTableUtil } from '@context/tableUtilContext';
import { IV1SkillGroupFilter, IV1SkillGroupPost, IV1SkillGroupPostSkill, IV1SkillGroupPut, skillGroupApi } from '@core/api/skill-group.api';
import { SkillGroupItem, skillGroupItemDefaultValues } from '@models/skillGroup';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useSkillGroupCreateSkillMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1SkillGroupPostSkill) => {
            return skillGroupApi.v1SkillGroupPostSkill(input);
        },
        {
            onSuccess: (data) => {
                toast.success('Create skill successfully');
                queryClient.invalidateQueries(['skill-group', 'skill-groups', 'skill', 'skills']);
            },
        }
    );

    return { mutateCreateSkill: mutate, mutateCreateSkillAsync: mutateAsync, ...rest };
};

const useQuerySkillGroupById = (id: string) => {
    return useQuery<SkillGroupItem>(
        ['skill-group', id],
        async () => {
            return skillGroupApi.v1GetId({ id });
        },
        {
            enabled: !!id,
            initialData: skillGroupItemDefaultValues,
        }
    );
};

const useSkillGroupCreateMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1SkillGroupPost) => {
            return skillGroupApi.v1Post(input);
        },
        {
            onSuccess: (data) => {
                toast.success('Create skill group successfully');
                queryClient.invalidateQueries(['skill-group', 'skill-groups']);
            },
        }
    );

    return { mutateCreateSkillGroup: mutate, mutateCreateSkillGroupAsync: mutateAsync, ...rest };
};

const useSkillGroupUpdateMutation = () => {
    const queryClient = useQueryClient();

    const { mutate, mutateAsync, ...rest } = useMutation(
        async (input: IV1SkillGroupPut) => {
            return skillGroupApi.v1Put(input);
        },
        {
            onSuccess: (data) => {
                toast.success('Update skill group successfully');
                queryClient.invalidateQueries(['skill-group', data.id, 'skill-groups']);
            },
        }
    );

    return { mutateUpdateSkillGroup: mutate, mutateUpdateSkillGroupAsync: mutateAsync, ...rest };
};

const useQuerySkillGroupFilter = (filter: Partial<IV1SkillGroupFilter>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['skill-groups', filter],
        async () => {
            const res = await skillGroupApi.v1GetFilter(filter);
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

//

export {
    useQuerySkillGroupById,
    useQuerySkillGroupFilter,
    useSkillGroupCreateMutation,
    useSkillGroupCreateSkillMutation,
    useSkillGroupUpdateMutation,
};
