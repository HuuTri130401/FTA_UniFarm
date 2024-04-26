import { FormWrapper, TextInput } from '@components/forms';
import { AvatarUploadInput } from '@components/forms/AvatarUploadInput';
/* import { IV1UserUpdate } from '@core/api';
import { useUpdateUserProfileMutation } from '@hooks/api/user.hook'; */
import { useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

export interface IV1UserUpdate {
    avatar: string | null;
    fullName: string;
    id: string;
    phone: string;
}
interface UpdateUserModalProps extends ModalProps {
    currentValue: IV1UserUpdate;
}
// dùng tạm
const useUpdateUserProfileMutation = () => {
    const [isLoading, setIsLoading] = React.useState<boolean>(false);

    const mutateUpdateProfile = async (data: IV1UserUpdate, options: { onSuccess: () => void }) => {
        setIsLoading(true);
        try {
            // Call api here
            console.log('Update user', data);
            options.onSuccess();
        } catch (error) {
            console.error('Update user error', error);
        } finally {
            setIsLoading(false);
        }
    };

    return { mutateUpdateProfile, isLoading };
};

const defaultValues: IV1UserUpdate = {
    avatar: '',
    fullName: '',
    id: '',
    phone: '',
};

const UpdateUserModal: React.FunctionComponent<UpdateUserModalProps> = ({ currentValue, ...rest }) => {
    const methods = useForm<IV1UserUpdate>({ defaultValues });

    const { mutateUpdateProfile, isLoading } = useUpdateUserProfileMutation();

    React.useEffect(() => {
        methods.setValue('phone', currentValue.phone);
        methods.setValue('fullName', currentValue.fullName);
        methods.setValue('avatar', currentValue.avatar);
        methods.setValue('id', currentValue.id);
    }, [currentValue]);

    const queryClient = useQueryClient();

    const onSubmit = (data: IV1UserUpdate) => {
        mutateUpdateProfile(data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('Update user successfully');
                queryClient.invalidateQueries();
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Update User"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={isLoading} onClick={rest.onCancel}>
                        Cancel
                    </Button>,
                    <Button key="edit" type="primary" loading={isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Save
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <AvatarUploadInput name="avatar" label="Avatar" className="col-span-full" />
                    <TextInput name="fullName" label="Fullname" placeholder="Phạm Văn A" required />
                    <TextInput name="phone" label="Phone" placeholder="0987654321" required />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default UpdateUserModal;
