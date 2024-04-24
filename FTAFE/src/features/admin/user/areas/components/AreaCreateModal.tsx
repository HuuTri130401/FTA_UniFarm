import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { AreaAPI } from '@core/api/area.api';
import { CreateAreaForm } from '@models/area';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface AreaCreateModalProps extends ModalProps {}
const defaultValues = {
    id: '',
    province: '',
    district: '',
    commune: '',
    address: '',
    status: '',
    code: '',
};
const AreaCreateModal: React.FC<AreaCreateModalProps> = ({ ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();

    const createCategoryMutation = useMutation(async (data: CreateAreaForm) => await AreaAPI.createOne(data), {
        onSuccess: () => {
            methods.reset();
            toast.success('Create success');
            queryClient.invalidateQueries();
            rest.afterClose && rest.afterClose();
        },
        onError: () => {
            toast.error('created fail');
        },
    });
    const onSubmit = async (data: CreateAreaForm) => {
        const _data = {
            ...data,
            id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        };
        createCategoryMutation.mutate(_data);
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo mới khu vực"
                width={600}
                footer={[
                    <Button key="close" type="default" onClick={rest.onCancel}>
                        Huỷ
                    </Button>,
                    <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="address" label="Địa chỉ" required />
                    <TextInput name="district" label="Quận" required />
                    <TextInput name="province" label="Tỉnh" required />
                    <TextInput name="commune" label="Xã" required />
                    <TextInput name="code" label="Mã" required />
                    <SelectInput
                        name="status"
                        label="Trạng thái"
                        options={[
                            { value: 'Active', label: 'Hoạt động', origin: 'Active' },
                            { value: 'InActive', label: 'Không hoạt động', origin: 'InActive' },
                        ]}
                    />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default AreaCreateModal;
