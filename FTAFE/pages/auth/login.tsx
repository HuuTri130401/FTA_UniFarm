import { NextPage } from 'next';

import { CommonSeo, UnProtectWrapper } from '../../src/core/components';
import { AuthLogin } from '../../src/features/auth/AuthLogin';

interface LoginPageProps {
    token: string;
}

const LoginPage: NextPage<LoginPageProps> = ({}) => {
    return (
        <>
            <CommonSeo title="Login" />

            <UnProtectWrapper>
                <AuthLogin />
            </UnProtectWrapper>
        </>
    );
};

export default LoginPage;
