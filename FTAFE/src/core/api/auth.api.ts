import { User } from '@models/user';
import _get from 'lodash.get';

import { http } from '.';
import { LoginTokenPayload } from '../../features/auth/components/interface';

export interface IV1AuthLogin extends Pick<User, 'email'> {
    password: string;
}
export interface IV1AuthRegister extends Pick<User, 'email'> {
    password: string;
    confirmPassword: string;
}

export interface IV1AuthVerify {
    otp: string;
}

export const authApi = {
    vLoginGoogle: async (token: LoginTokenPayload) => {
        const res = await http.post(`auth/oAuth`, token);
        return _get(res, 'data.token', res.data) as string;
    },
    v1PostLogin: async (input: IV1AuthLogin) => {
        const res = await http.post('/auth/login', input);
        return _get(res, 'data.token', '');
    },
    v1GetLogout: async () => {
        const res = await http.get('/auth/logout');
        return _get(res, 'data', '');
    },

    v1PostRegister: async (input: IV1AuthRegister) => {
        const res = await http.post('/auth/register', input);
        return _get(res, 'data.token', '');
    },
    v1VerifyOtp: async (input: IV1AuthVerify) => {
        const res = await http.post(`/auth/verify-phone/${input.otp}`, input);

        return _get(res, 'data', '');
    },
};
