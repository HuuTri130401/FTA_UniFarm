import { IV1UpdateUserSkill, userSkillApi } from '@core/api/user-skill';
import { UserSkillForm } from '@models/userSkill';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryUserSkills = () => {
    return useQuery(['userSkills'], async () => await userSkillApi.v1GetUserSkills(), {
        initialData: [],
    });
};

const useMutationAddUserSkill = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: UserSkillForm) => {
            return userSkillApi.v1PostUserSkill(input);
        },
        {
            onSuccess: () => {
                toast.success('UserSkill Add Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateAddUserSkill: mutate, mutateAddUserSkillAsync: mutateAsync, ...rest };
};

const useMutationUpdateUserSkill = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (input: IV1UpdateUserSkill) => {
            return userSkillApi.v1PutUserSkill(input);
        },
        {
            onSuccess: () => {
                toast.success('UserSkill Update Success!');
                queryClient.invalidateQueries();
            },
        }
    );

    return { mutateUpdateUserSkill: mutate, mutateUpdateUserSkillAsync: mutateAsync, ...rest };
};

const useMutationDeleteUserSkill = () => {
    const queryClient = useQueryClient();
    const { mutate, mutateAsync, ...rest } = useMutation(
        (id: string) => {
            return userSkillApi.v1DeleteUserSkill(id);
        },
        {
            onSuccess: () => {
                toast.success('UserSkill Delete Success!');
                queryClient.invalidateQueries();
            },
        }
    );
    return { mutateDeleteUserSkill: mutate, mutateDeleteUserSkillAsync: mutateAsync, ...rest };
};

export { useMutationAddUserSkill, useMutationDeleteUserSkill, useMutationUpdateUserSkill, useQueryUserSkills };
