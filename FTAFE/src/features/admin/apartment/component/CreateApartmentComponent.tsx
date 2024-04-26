import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { ApartmentAPI } from '@core/api/apartment.api';
import { AreaAPI } from '@core/api/area.api';
import { CreateApartmentForm, UpdateApartmentForm } from '@models/apartment';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateApartmentModalProps extends ModalProps {}
const defaultValues: CreateApartmentForm = {
    id: '',
    address: '',
    status: '',
    code: '',
    areaId: '',
    name: '',
};
const CreateApartmentModal: React.FC<CreateApartmentModalProps> = ({ ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();

    const createCategoryMutation = useMutation(async (data: UpdateApartmentForm) => await ApartmentAPI.createOne(data), {
        onSuccess: () => {
            methods.reset();
            toast.success('Create success');
            queryClient.invalidateQueries(['apartments']);
            rest.afterClose && rest.afterClose();
        },
        onError: () => {
            toast.error('created fail');
        },
    });
    const onSubmit = async (data: CreateApartmentForm) => {
        if (data.code.length < 5) {
            methods.setError('code', {
                type: 'manual',
                message: 'The code must be at least 5 characters long.',
            });
            return;
        }
        const _data = {
            ...data,
            id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        };
        createCategoryMutation.mutate(_data);
    };
    const useAreaQuery = useQuery({
        queryKey: ['areas'],
        queryFn: async () => {
            const res = await AreaAPI.getAll({});
            return res;
        },
    });
    const listArea = useAreaQuery.data || [];
    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo mới căn hộ"
                width={600}
                footer={[
                    <Button key="close" type="default" onClick={rest.onCancel}>
                        Hủy
                    </Button>,
                    <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="name" label="Tên" required />
                    <TextInput name="address" label="Địa chỉ" required />
                    <TextInput name="code" label="Mã" required />
                    <SelectInput
                        name="areaId"
                        label="Khu vực"
                        options={listArea.map((a) => {
                            return {
                                label: a.address,
                                value: a.id,
                                origin: a.id,
                            };
                        })}
                    />
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

export default CreateApartmentModal;
