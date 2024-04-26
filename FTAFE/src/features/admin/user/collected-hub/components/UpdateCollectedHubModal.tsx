import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { AvatarUploadInput } from '@components/forms/AvatarUploadInput';
import { CollectedHubAPI, UpdateCollectedHub } from '@core/api/collected-hub.api';
import { CollectedHub } from '@models/staff';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateCollectedHubModalProps extends ModalProps {
    currentValue: CollectedHub;
}

const defaultValues: UpdateCollectedHub = {
    id: '',
    name: '',
    description: '',
    image: '',
    code: '',
    status: '',
    address: '',
};

const UpdateCollectedHubModal: React.FC<UpdateCollectedHubModalProps> = ({ currentValue, ...rest }) => {
    const updateHubMutation = useMutation({
        mutationFn: async (data: UpdateCollectedHub) => {
            const res = await CollectedHubAPI.update(currentValue.id, data);
            return res;
        },
    });

    const methods = useForm({
        defaultValues,
    });

    React.useEffect(() => {
        methods.setValue('code', currentValue.code);
        methods.setValue('id', currentValue.id);
        methods.setValue('name', currentValue.name);
        methods.setValue('address', currentValue.address);
        methods.setValue('description', currentValue.description);
        methods.setValue('image', currentValue.image);
        methods.setValue('status', currentValue.status);
    }, [currentValue]);

    const queryClient = useQueryClient();

    const onSubmit = (data: UpdateCollectedHub) => {
        updateHubMutation.mutateAsync(data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('Cập nhật kho hàng thành công');
                queryClient.invalidateQueries(['collected-hub-list']);
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Cập nhật kho"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={updateHubMutation.isLoading} onClick={rest.onCancel}>
                        Hủy
                    </Button>,
                    <Button key="edit" type="primary" loading={updateHubMutation.isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <AvatarUploadInput name="image" label="Ảnh đại diện" className="col-span-full" path="collected-hubs" />
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
export default UpdateCollectedHubModal;
