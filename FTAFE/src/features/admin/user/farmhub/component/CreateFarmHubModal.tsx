import { FormWrapper, PasswordInput, TextInput } from '@components/forms';
import CustomImageUpload from '@components/forms/CustomImageInput';
import { FarmHubAPI } from '@core/api/farmhub.api';
import { CreateFarmHubFormData } from '@models/farmhub';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateFarmHubModalProps extends ModalProps {}
const defaultValues = {
    FirstName: '',
    LastName: '',
    FarmHubName: '',
    FarmHubCode: '',
    FarmHubAddress: '',
    UserName: '',
    PhoneNumber: '',
    Password: '',
    Email: '',
    Description: '',
    FarmHubImage: null,
};
const CreateFarmHubModal: React.FC<CreateFarmHubModalProps> = ({ ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();
    const { errors } = methods.formState;

    const createCategoryMutation = useMutation(async (data: CreateFarmHubFormData) => await FarmHubAPI.createShop(data), {
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
    const onSubmit = async (data: CreateFarmHubFormData) => {
        if (data.LastName == '') {
            methods.setError('LastName', {
                type: 'manual',
                message: 'Họ không thể để trống',
            });
        }
        if (data.LastName == '') {
            methods.setError('FirstName', {
                type: 'manual',
                message: 'Tên không thể để trống',
            });
        }
        if (data.UserName == '') {
            methods.setError('FirstName', {
                type: 'manual',
                message: 'Tên người dùng không thể để trống',
            });
        }
        if (data.Email == '') {
            methods.setError('Email', {
                type: 'manual',
                message: 'Email không thể để trống',
            });
        }
        if (!data.Email.match(/^\w+([-_.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/)) {
            methods.setError('Email', { type: 'manual', message: 'Email không hợp lệ' });
            return;
        }
        if (data.PhoneNumber === '') {
            methods.setError('PhoneNumber', { type: 'manual', message: 'Số điện thoại không được để trống' });
            return;
        }
        if (data.PhoneNumber.length !== 10) {
            methods.setError('PhoneNumber', { type: 'manual', message: 'Số điện thoại phải có 10 số' });
            return;
        }
        if (data.PhoneNumber.charAt(0) !== '0') {
            methods.setError('PhoneNumber', { type: 'manual', message: 'Số điện thoại phải bắt đầu từ 0' });
            return;
        }
        if (data.Password === '') {
            methods.setError('Password', { type: 'manual', message: 'Mật khẩu không được để trống' });
            return;
        }
        if (data.Password.length < 8) {
            methods.setError('Password', { type: 'manual', message: 'Mật khẩu phải có ít nhất 9 kí tự' });
            return;
        }
        if (data.FarmHubName === '') {
            methods.setError('FarmHubName', { type: 'manual', message: 'Tên trang trại không được để trống' });
            return;
        }
        if (data.FarmHubCode === '') {
            methods.setError('FarmHubCode', { type: 'manual', message: 'Mã số trang trại không được để trống' });
            return;
        }
        if (data.FarmHubAddress === '') {
            methods.setError('FarmHubAddress', { type: 'manual', message: 'Địa chỉ không được để trống' });
            return;
        }
        if (data.FarmHubImage == null) {
            methods.setError('FarmHubImage', { type: 'manual', message: 'Hình ảnh không được để trống' });
            return;
        }
        createCategoryMutation.mutateAsync(data, {
            onSuccess: (e) => {
                methods.reset();
                toast.success('Create success');
                queryClient.invalidateQueries(['farm-hubs']);
                rest.afterClose && rest.afterClose();
            },
            onError: (e) => {
                console.log('onSubmit ~ e:', e);
                toast.error('created fail');
            },
        });
    };

    return (
        <>
            <FormWrapper methods={methods}>
                <Modal
                    {...rest}
                    title="Tạo mới nông trại"
                    width={600}
                    footer={[
                        <Button key="reset" type="default" onClick={() => methods.reset()}>
                            Reset
                        </Button>,
                        <Button key="close" type="default" onClick={rest.onCancel}>
                            Cancel
                        </Button>,
                        <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                            Tạo
                        </Button>,
                    ]}
                >
                    <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                        <TextInput name="LastName" label="Họ và tên đệm" required />
                        <TextInput name="FirstName" label="Tên" required />
                        <TextInput name="UserName" label="Tên tài khoản" required />
                        <TextInput name="Email" label="Email" required />
                        <TextInput name="PhoneNumber" label="Số điện thoại" required />
                        <PasswordInput name="Password" label="Mật khẩu" />
                        <TextInput name="FarmHubName" label="Tên nông trại" required />
                        <TextInput name="FarmHubCode" label="Mã nông trại" required />
                        <TextInput name="FarmHubAddress" label="Địa chỉ nông trại" required />
                        <TextInput name="Description" label="Mô tả nông trại" />
                        {errors.FarmHubImage && (
                            <span
                                style={{
                                    color: 'red',
                                }}
                            >
                                {errors.FarmHubImage.message}
                            </span>
                        )}
                        <CustomImageUpload name="FarmHubImage" label="Ảnh đại diện" />
                    </form>
                </Modal>
            </FormWrapper>
        </>
    );
};

export default CreateFarmHubModal;
