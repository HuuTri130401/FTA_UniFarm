import { FormWrapper, PasswordInput, TextInput } from '@components/forms';
import { ADMIN_API, CreateStaffForm } from '@core/api/admin.api';
import { UserRole } from '@models/user';
import { useMutation } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateStaffModalProps extends ModalProps {
    role: UserRole;
}
const defaultValues = {
    email: '',
    password: '',
    phoneNumber: '',
    firstName: '',
    lastName: '',
    userName: '',
    role: '',
};
const CreateStaffModal: React.FC<CreateStaffModalProps> = ({ role, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });

    useEffect(() => {
        methods.setValue('role', role);
    }, [role]);

    const createStaffMutation = useMutation(async (data: CreateStaffForm) => await ADMIN_API.createStaff(data));

    const onSubmit = async (data: CreateStaffForm) => {
        if (data.email === '') {
            methods.setError('email', { type: 'manual', message: 'Email không được để trống' });
            return;
        }
        if (!data.email.match(/^\w+([-_.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/)) {
            methods.setError('email', { type: 'manual', message: 'Email không hợp lệ' });
            return;
        }

        if (data.lastName === '') {
            methods.setError('lastName', { type: 'manual', message: 'Họ không được để trống' });
            return;
        }
        if (data.firstName === '') {
            methods.setError('firstName', { type: 'manual', message: 'Tên không được để trống' });
            return;
        }

        if (data.phoneNumber === '') {
            methods.setError('phoneNumber', { type: 'manual', message: 'Số điện thoại không được để trống' });
            return;
        }
        if (data.phoneNumber.length !== 10) {
            methods.setError('phoneNumber', { type: 'manual', message: 'Số điện thoại phải có 10 số' });
            return;
        }
        if (data.phoneNumber.charAt(0) !== '0') {
            methods.setError('phoneNumber', { type: 'manual', message: 'Số điện thoại phải bắt đầu từ 0' });
            return;
        }
        if (data.userName === '') {
            methods.setError('userName', { type: 'manual', message: 'Tên người dùng không để trống' });
            return;
        }
        if (data.password == '') {
            methods.setError('password', { type: 'manual', message: 'password không được để trống' });
            return;
        }
        if (data.password.length < 8) {
            methods.setError('password', { type: 'manual', message: 'password phải có ít nhất 8 kí tự' });
            return;
        }
        console.log(data);
        createStaffMutation.mutateAsync(data, {
            onSuccess: (e) => {
                toast.success('Tạo nhân viên thành công');
                methods.reset();
                rest.afterClose && rest.afterClose();
            },
            onError: (e: any) => {
                console.log('onSubmit ~ e:', e);
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo Nhân Viên Mới"
                width={1000}
                footer={[
                    <Button key="reset" type="default" onClick={() => methods.reset()}>
                        Làm mới
                    </Button>,
                    <Button key="close" type="default" onClick={rest.onCancel}>
                        Trở lại
                    </Button>,
                    <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                        Tạo
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="email" label="Email" placeholder="sophie@example.com" required />
                    <TextInput name="lastName" label="Họ" required />
                    <TextInput name="firstName" label="Tên" required />
                    <TextInput name="phoneNumber" type="number" label="Số điện thoại" placeholder="0*********" required />
                    <TextInput name="userName" label="Tên người dùng" placeholder="userName" />
                    <PasswordInput name="password" label="Mật khẩu" required />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateStaffModal;
