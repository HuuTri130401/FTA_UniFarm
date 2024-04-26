import { useMutation } from '@tanstack/react-query';
import Link from 'next/link';
import { useRouter } from 'next/router';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

import { authApi, IV1AuthRegister } from '../../core/api';
import { FormBtn, FormWrapper, TextInput } from '../../core/components';
import { routes } from '../../core/routes';

const defaultValues: IV1AuthRegister = {
    email: '',
    password: '',
    confirmPassword: '',
};

interface AuthRegisterProps {}

const AuthRegister: React.FC<AuthRegisterProps> = () => {
    const methods = useForm<IV1AuthRegister>({ defaultValues });
    const router = useRouter();

    const { isLoading, mutate } = useMutation<any, any, IV1AuthRegister>(async (input) => await authApi.v1PostRegister(input), {
        onSuccess: () => {
            toast.success('Vui lòng kiểm tra tin nhắn OTP để xác thực tài khoản');
            router.push(routes.auth.verify());
        },
        onError: (error) => {
            toast.error(error.data.errorMessage);
        },
    });

    return (
        <div className="fade-in">
            <div className="mb-8 font-semibold">Đăng Ký</div>
            <FormWrapper methods={methods}>
                <form onSubmit={methods.handleSubmit((data) => mutate(data))}>
                    <div className="space-y-4">
                        <TextInput label="Số Điện Thoại" name="phone" />
                        <TextInput label="Name" name="name" />
                        <TextInput label="Mật khẩu" name="password" type="password" />
                        <TextInput label="Nhập lại mật khẩu" name="confirmPassword" type="password" />
                    </div>
                    <FormBtn label="Đăng Nhập" className="w-full mt-8" isLoading={isLoading} />
                </form>
            </FormWrapper>
            <div className="mt-6 ">
                <div className="space-y-6">
                    <div className="flex justify-center ">
                        <Link href={routes.auth.login()}>
                            <a>
                                <div className="space-x-1 text-sm font-medium ">
                                    <span>Đã Có Tài Khoản?</span>
                                    <span className="text-blue-600 ">Đăng Nhập</span>
                                </div>
                            </a>
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AuthRegister;
