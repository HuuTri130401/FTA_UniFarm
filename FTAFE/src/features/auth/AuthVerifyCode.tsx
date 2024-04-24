import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/router';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

import { authApi, IV1AuthVerify } from '../../core/api';
import { FormBtn, FormWrapper, TextInput } from '../../core/components';
import { routes } from '../../core/routes';

const defaultValues: IV1AuthVerify = {
    otp: '',
};

interface AuthVerifyCodeProps {}

export const AuthVerifyCode: React.FC<AuthVerifyCodeProps> = () => {
    const methods = useForm<IV1AuthVerify>({ defaultValues });
    const router = useRouter();
    const { isLoading, mutate } = useMutation<any, any, IV1AuthVerify>(async (input) => await authApi.v1VerifyOtp(input), {
        onSuccess: () => {
            toast.success('Xác thực thành công, Vui lòng đăng nhập');
            router.push(routes.auth.login());
        },
        onError: (error) => {
            toast.error(error.data.errorMessage);
        },
    });
    return (
        <div>
            <div className="mb-8 font-semibold">Xác Thật OTP</div>
            <FormWrapper methods={methods}>
                <form onSubmit={methods.handleSubmit((data) => mutate(data))}>
                    <div className="space-y-4">
                        <TextInput label="Mã OTP" name="otp" />
                    </div>
                    <FormBtn label="Xác Thật" className="w-full mt-8" isLoading={isLoading} />
                </form>
            </FormWrapper>
        </div>
    );
};
