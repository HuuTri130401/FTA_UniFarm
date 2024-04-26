import { useRouter } from 'next/router';
import * as React from 'react';

import { MainFooter } from '../footers';
import MainNavbar from '../navbars/MainNavBar';
import { UnProtectWrapper } from '../wrappers';
import { AuthLayout } from './AuthLayout';
import { DashboardLayout } from './DashboardLayout';

interface DynamicLayoutProps {
    children: React.ReactNode;
}

export const DynamicLayout: React.FC<DynamicLayoutProps> = ({ children }) => {
    const router = useRouter();

    if (
        router.pathname === '/cv/[id]' ||
        router.pathname === '/cv/slug/[slug]' ||
        router.pathname === '/user/transaction/success' ||
        router.pathname === '/user/transaction/fail'
    ) {
        return <>{children}</>;
    }

    if (router.pathname.startsWith('/auth')) {
        return (
            <UnProtectWrapper>
                <AuthLayout>{children}</AuthLayout>
            </UnProtectWrapper>
        );
    }

    // Footless UI

    // if (router.pathname.startsWith('/job')) {
    //     return (
    //         <div className="relative flex flex-col justify-start h-full min-h-screen">
    //             <MainNavbar />
    //             <div className="w-full py-20">{children}</div>
    //         </div>
    //     );
    // }

    // Headless UI

    if (router.pathname.startsWith('/cv')) {
        return (
            <div className="relative flex flex-col justify-start h-full min-h-screen">
                <MainNavbar />
                <div className="w-full py-20">{children}</div>
                <MainFooter />
            </div>
        );
    }

    if (router.pathname.startsWith('/admin') || router.pathname.startsWith('/farmhub') || router.pathname.startsWith('/staff')) {
        return <DashboardLayout>{children}</DashboardLayout>;
    }

    return (
        <div className="relative flex flex-col justify-start h-full min-h-screen">
            <MainNavbar />
            <div className="w-full py-20 pb-0">{children}</div>
            <MainFooter />
        </div>
    );
};
