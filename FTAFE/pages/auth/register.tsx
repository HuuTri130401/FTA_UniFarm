import * as React from 'react';

import { CommonSeo, UnProtectWrapper } from '../../src/core/components';
import AuthRegister from '../../src/features/auth/AuthRegister';

interface RegisterPageProps {}

const RegisterPage: React.FC<RegisterPageProps> = () => {
    return (
        <>
            <CommonSeo title="Register" />
            <AuthRegister />
        </>
    );
};

export default RegisterPage;
