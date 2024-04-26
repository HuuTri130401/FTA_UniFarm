import { authApi } from '@core/api';
import { routes } from '@core/routes';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { useRouter } from 'next/router';
import * as React from 'react';
import { toast } from 'react-toastify';

interface UnProtectionWrapperProps {
    children: React.ReactNode;
}

export const UnProtectWrapper: React.FC<UnProtectionWrapperProps> = ({ children }) => {
    const user = useStoreUser();
    const router = useRouter();
    React.useEffect(() => {
        if (user.isAuth) {
            // switch (user.type) {
            //     case UserRole.CANDIDATE:
            //         router.push(routes.job.list());
            //         break;
            //     case UserRole.ADMIN:
            //         if (!router.pathname.startsWith('/admin')) router.push(routes.admin.home());
            //         break;
            //     case UserRole.EXPERT:
            //         if (!router.pathname.startsWith('/expert')) router.push(routes.expert.home());
            //         break;
            //     case UserRole.STAFF:
            //         if (!router.pathname.startsWith('/staff')) router.push(routes.staff.home());
            //         break;
            //     default:
            //     }
            // }
            switch (user.roleName) {
                case UserRole.ADMIN:
                    if (!router.pathname.startsWith('/admin')) router.push(routes.admin.home());
                    break;
                case UserRole.FARM_HUB:
                    if (!router.pathname.startsWith('/farmhub')) router.push(routes.farmhub.home());
                    break;
                case UserRole.COLLECTED_STAFF:
                    if (!router.pathname.startsWith('/staff')) router.push(routes.staff.home());
                    break;
                default:
                    toast.error('Bạn không có quyền truy cập');
                    authApi.v1GetLogout().then(() => {
                        setTimeout(() => {
                            window.location.reload();
                        }, 3000);
                    });
                    break;
            }
        }
    }, [user]);

    return <>{children}</>;
};
