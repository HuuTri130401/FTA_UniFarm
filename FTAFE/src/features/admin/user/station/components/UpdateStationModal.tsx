import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { AvatarUploadInput } from '@components/forms/AvatarUploadInput';
import { StationAPI, UpdateStation } from '@core/api/station.api';
import { Station } from '@models/staff';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateStationModalProps extends ModalProps {
    currentValue: Station;
}

const defaultValues: UpdateStation = {
    id: '',
    name: '',
    description: '',
    image: '',
    code: '',
    status: '',
    address: '',
    areaId: '',
};

const UpdateStationModal: React.FC<UpdateStationModalProps> = ({ currentValue, ...rest }) => {
    const updateHubMutation = useMutation({
        mutationFn: async (data: UpdateStation) => {
            const res = await StationAPI.update(currentValue.id, data);
            return res;
        },
    });

    const methods = useForm({
        defaultValues,
    });

    React.useEffect(() => {
        methods.setValue('code', currentValue.code);
        methods.setValue('id', currentValue.id);
        methods.setValue('areaId', currentValue.areaId);
        methods.setValue('name', currentValue.name);
        methods.setValue('address', currentValue.address);
        methods.setValue('description', currentValue.description);
        methods.setValue('image', currentValue.image);
        methods.setValue('status', currentValue.status);
    }, [currentValue]);

    const queryClient = useQueryClient();

    const onSubmit = (data: UpdateStation) => {
        updateHubMutation.mutateAsync(data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('cập nhật thành công');
                queryClient.invalidateQueries();
            },
            onError: (error) => {
                console.log('onSubmit ~ error:', error);

                toast.error('Update Station failed');
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Cập nhật trạm hàng hóa"
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
                    <AvatarUploadInput name="image" label="Ảnh Đại diện" className="col-span-full" path="stations" />
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
export default UpdateStationModal;
