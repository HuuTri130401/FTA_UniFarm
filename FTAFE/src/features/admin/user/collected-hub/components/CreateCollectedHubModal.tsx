import { FormWrapper, TextInput } from '@components/forms';
import { AvatarUploadInput } from '@components/forms/AvatarUploadInput';
import { CollectedHubAPI, CreateCollectedHub } from '@core/api/collected-hub.api';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateCollectedHubModalProps extends ModalProps {}
const defaultValues = {
    name: '',
    description: '',
    image: '',
    code: '',
    address: '',
};
const CreateCollectedHubModal: React.FC<CreateCollectedHubModalProps> = ({ ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();

    const { errors } = methods.formState;

    const createCategoryMutation = useMutation(async (data: CreateCollectedHub) => await CollectedHubAPI.createOne(data), {
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
    const onSubmit = async (data: CreateCollectedHub) => {
        createCategoryMutation.mutate(data);
    };
    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo kho hàng"
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
                    <AvatarUploadInput label="Hình ảnh" name="image" path="collected-hubs" />
                    <TextInput name="name" label="Tên" required />
                    <TextInput name="address" label="Địa chỉ" required />
                    <TextInput name="description" label="Mô tả" placeholder="Mô tả ..." required />
                    <TextInput name="code" label="Mã" required />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateCollectedHubModal;
