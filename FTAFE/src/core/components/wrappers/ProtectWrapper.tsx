import { routes } from '@core/routes';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { useRouter } from 'next/router';
import * as React from 'react';

interface ProtectWrapperProps {
    acceptRoles: Array<UserRole>;
    children: React.ReactNode;
}

export const ProtectWrapper: React.FC<ProtectWrapperProps> = ({ children, acceptRoles }) => {
    const user = useStoreUser();
    const router = useRouter();

    React.useEffect(() => {
        if (!user.isLogin && !user.isAuth) {
            router.push(routes.auth.login());
            return;
        }
        if (acceptRoles.length <= 0) {
            return;
        }
        if (user.isAuth && !acceptRoles.includes(user.roleName)) {
            router.push(routes.auth.login());
        }
    }, [acceptRoles, user, router]);

    return <>{children}</>;
};
