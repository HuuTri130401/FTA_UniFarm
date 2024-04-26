import FormCommonErrorMessage from '@components/forms/FormCommonErrorMessage';
import { useAuthLoginMutation } from '@hooks/api/auth.hook';
import { Button, Tabs } from 'antd';
import { useRouter } from 'next/router';
import React from 'react';
import { useForm } from 'react-hook-form';

import { IV1AuthLogin } from '../../core/api';
import { FormWrapper, TextInput } from '../../core/components';

const defaultValues: IV1AuthLogin = {
    email: '',
    password: '',
};

interface AuthLoginProps {}

export const AuthLogin: React.FC<AuthLoginProps> = ({}) => {
    const methods = useForm<IV1AuthLogin>({ defaultValues });

    const { mutationAuthLogin, isLoading } = useAuthLoginMutation();

    const router = useRouter();

    const onSubmit = (data: IV1AuthLogin) => {
        if (data.email === '') {
            methods.setError('email', { type: 'manual', message: 'Email không được để trống' });
            return;
        }
        if (data.password === '') {
            methods.setError('password', { type: 'manual', message: 'Mật khẩu không được để trống' });
            return;
        }
        mutationAuthLogin(data);
    };

    const TabAuth = () => {
        return (
            <Tabs
                defaultActiveKey="1"
                className="login-tabs"
                items={[
                    {
                        label: (
                            <div className="flex items-center space-x-2">
                                <p className="m-0 text-lg font-semibold text-gray-800 dark:text-white">ADMIN / STAFF</p>
                                <span className="mt-1 text-sm font-medium text-gray-500">(CollectHub, FarmHub)</span>
                            </div>
                        ),
                        key: '1',
                        children: (
                            <>
                                <FormWrapper methods={methods}>
                                    <form onSubmit={methods.handleSubmit(onSubmit)}>
                                        <div className="space-y-4">
                                            <div>
                                                <TextInput label="Email" name="email" type="email" placeholder="user@gmail.com" />
                                                <FormCommonErrorMessage />
                                            </div>
                                            <TextInput label="Mật Khẩu" name="password" type="password" placeholder="password" />
                                        </div>
                                        <Button className="w-full mt-4" type="primary" htmlType="submit">
                                            Đăng Nhập
                                        </Button>
                                    </form>
                                </FormWrapper>
                                <div className="mt-6">
                                    <div className="space-y-6">
                                        <div className="flex justify-center ">
                                            {/* <Link href={routes.auth.register()}>
                                                <a>
                                                    <div className="space-x-1 text-sm font-medium">
                                                        <div className="space-x-1 text-sm font-medium">
                                                            <span className="text-black">Tạo Tài Khoản Mới?</span>
                                                            <span className="text-blue-600">Đăng Ký</span>
                                                        </div>
                                                    </div>
                                                </a>
                                            </Link> */}
                                        </div>
                                    </div>
                                </div>
                            </>
                        ),
                    },
                ]}
            />
        );
    };

    return (
        <>
            <section
                className="relative flex items-center justify-center w-full h-screen"
                style={{
                    backgroundImage: `url('https://i.upanh.org/2024/04/09/imagec211877acc8276c5.png')`,
                    backgroundPosition: 'center',
                    backgroundSize: 'cover',
                    backgroundRepeat: 'no-repeat',
                }}
            >
                <div className="w-full max-w-[25.625rem] bg-white bg-opacity-80 rounded p-10 shadow-lg">
                    <div className="mb-6 text-center">
                        <h1 className="text-3xl font-bold text-jacarta-700 dark:text-white">
                            Chào mừng đến với{' '}
                            <span className="inline-block align-middle">
                                <img src="/assets/images/logo/logo-main1.png" className="h-13 lg:h-17" alt="Brand | Logo" />
                            </span>
                        </h1>
                    </div>
                    {TabAuth()}
                </div>
            </section>
        </>
    );
};
