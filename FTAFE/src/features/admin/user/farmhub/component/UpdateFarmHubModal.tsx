import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { AvatarUploadInput } from '@components/forms/AvatarUploadInput';
import { FarmHubAPI } from '@core/api/farmhub.api';
import { UpdateFarmHubForm } from '@models/farmhub';
import { FarmHub, UserRole } from '@models/user';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateFarmHubModalProps extends ModalProps {
    currentValue: FarmHub;
}

const defaultValues: UpdateFarmHubForm = {
    name: '',
    description: '',
    image: '',
    code: '',
    status: '',
    address: '',
    updatedAt: '',
    roleName: UserRole.FARM_HUB,
};

const UpdateFarmHubModal: React.FC<UpdateFarmHubModalProps> = ({ currentValue, ...rest }) => {
    const updateHubMutation = useMutation({
        mutationFn: async (data: UpdateFarmHubForm) => {
            const res = await FarmHubAPI.updateFarmHub(currentValue.id, data);
            return res;
        },
    });

    const methods = useForm({
        defaultValues,
    });

    React.useEffect(() => {
        methods.setValue('code', currentValue.code);
        methods.setValue('name', currentValue.name);
        methods.setValue('address', currentValue.address);
        methods.setValue('description', currentValue.description);
        methods.setValue('image', currentValue.image);
        methods.setValue('status', currentValue.status);
        methods.setValue('updatedAt', currentValue.updatedAt);
    }, [currentValue]);

    const queryClient = useQueryClient();

    const onSubmit = (data: UpdateFarmHubForm) => {
        const _data = {
            ...data,
            updatedAt: new Date().toISOString(),
        };
        updateHubMutation.mutateAsync(_data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('Update Hub successfully');
                queryClient.invalidateQueries();
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Chỉnh sửa thông tin nông trại"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={updateHubMutation.isLoading} onClick={rest.onCancel}>
                        hủy
                    </Button>,
                    <Button key="edit" type="primary" loading={updateHubMutation.isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <AvatarUploadInput name="image" label="Ảnh đại diện" className="col-span-full" path="farm-hub" />
                    <TextInput name="name" label="Tên" required />
                    <TextInput name="address" label="Địa chỉ" required />
                    <TextInput name="description" label="Mô tả" placeholder="Mô tả ..." required />
                    <TextInput name="code" label="Mã" required />
                    <SelectInput
                        label="Trạng thái"
                        name="status"
                        options={[
                            {
                                label: 'Hoạt động',
                                value: 'Active',
                                origin: 'active',
                            },
                            {
                                label: 'Không hoạt động',
                                value: 'Inactive',
                                origin: 'inactive',
                            },
                        ]}
                    />
                </form>
            </Modal>
        </FormWrapper>
    );
};
export default UpdateFarmHubModal;
