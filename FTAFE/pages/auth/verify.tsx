import * as React from 'react';

import { AuthVerifyCode } from '../../src/features/auth/AuthVerifyCode';

interface VerifyCodePageProps {}

const VerifyCodePage: React.FC<VerifyCodePageProps> = () => {
    return (
        <>
            <AuthVerifyCode />
        </>
    );
};

export default VerifyCodePage;
