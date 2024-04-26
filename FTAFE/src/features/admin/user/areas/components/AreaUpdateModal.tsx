import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { AreaAPI } from '@core/api/area.api';
import { Area, UpdateAreaForm } from '@models/area';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateAreaModalProps extends ModalProps {
    currentValue: Area;
}

const defaultValues: UpdateAreaForm = {
    id: '',
    province: '',
    district: '',
    commune: '',
    address: '',
    status: '',
    code: '',
};

const UpdateAreaModal: React.FC<UpdateAreaModalProps> = ({ currentValue, ...rest }) => {
    const updateHubMutation = useMutation({
        mutationFn: async (data: UpdateAreaForm) => await AreaAPI.updateOne(currentValue.id, data),
    });

    const methods = useForm({
        defaultValues,
    });

    React.useEffect(() => {
        methods.setValue('code', currentValue.code);
        methods.setValue('id', currentValue.id);
        methods.setValue('province', currentValue.province);
        methods.setValue('district', currentValue.district);
        methods.setValue('commune', currentValue.commune);
        methods.setValue('address', currentValue.address);
        methods.setValue('status', currentValue.status);
    }, [currentValue]);

    const queryClient = useQueryClient();

    const onSubmit = (data: UpdateAreaForm) => {
        updateHubMutation.mutateAsync(data, {
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
                title="Cập nhật thông tin khu vực"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={updateHubMutation.isLoading} onClick={rest.onCancel}>
                        HỦy
                    </Button>,
                    <Button key="edit" type="primary" loading={updateHubMutation.isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="address" label="Địa chỉ" required />
                    <TextInput name="province" label="Tỉnh" required />
                    <TextInput name="district" label="Huyện" required />
                    <TextInput name="commune" label="Xã" required />
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
export default UpdateAreaModal;
