import { Breadcrumb } from 'antd';
import Link from 'next/link';
import * as React from 'react';

interface DashboardHeaderLayoutProps {
    title: string;
    children: React.ReactNode;
    breadcrumbs?: Breadcrumb[];
}

interface Breadcrumb {
    path?: string;
    key: string;
    element: React.ReactNode;
}

export const DashboardHeaderLayout: React.FC<DashboardHeaderLayoutProps> = ({ title, children, breadcrumbs }) => {
    return (
        <div className="flex flex-col items-start justify-between w-full">
            <div className="flex items-center justify-between w-full p-4 text-xl bg-white">
                <h1 className="font-medium text-gray-700">{title}</h1>
                {breadcrumbs && breadcrumbs.length > 0 && (
                    <Breadcrumb>
                        {breadcrumbs.map((breadcrumb) => (
                            <Breadcrumb.Item key={breadcrumb.key}>
                                {breadcrumb.path ? (
                                    <Link legacyBehavior href={breadcrumb.path}>
                                        <a>{breadcrumb.element}</a>
                                    </Link>
                                ) : (
                                    breadcrumb.element
                                )}
                            </Breadcrumb.Item>
                        ))}
                    </Breadcrumb>
                )}
            </div>

            <div className="flex items-center w-full p-4 space-x-2 rounded-lg">{children}</div>
        </div>
    );
};
