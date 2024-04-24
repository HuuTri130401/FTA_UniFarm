import { routes } from '@core/routes';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { Button } from 'antd';
import clsx from 'clsx';
import Link from 'next/link';
import { useRouter } from 'next/router';
import * as React from 'react';

interface UserSettingLayoutProps {}

const profileMenu = [
    {
        title: 'Profile',
        path: routes.user.profile.detail(),
    },
    {
        title: 'Update Profile',
        path: routes.user.profile.update(),
    },
];

const transactionMenu = [
    {
        title: 'Transaction',
        path: routes.user.transaction.list(),
    },
    {
        title: 'Deposit',
        path: routes.user.transaction.deposit(),
    },
];

const notificationMenu = [
    {
        title: 'Notification History',
        path: routes.user.notification.list(),
    },
    // {
    //     title: 'Notification Setting',
    //     path: routes.user.notification.setting(),
    // },
];

const UserSettingLayout: React.FunctionComponent<UserSettingLayoutProps & React.PropsWithChildren> = ({ children }) => {
    const router = useRouter();
    const { roleName } = useStoreUser();

    return (
        <div className="flex items-center justify-center py-10 bg-primary/10">
            <div className="flex w-full h-full gap-4 p-4  max-w-[1440px]">
                <div className="w-[240px] flex-shrink-0 rounded bg-white overflow-hidden shadow-lg">
                    <div className="flex flex-col h-full gap-2 px-4 py-10">
                        <p className="m-0 mb-2 text-2xl font-bold text-black">Setting</p>
                        <Button type={router.pathname.startsWith(routes.user.profile.detail()) ? 'primary' : 'default'}>
                            <Link href={routes.user.detail()}>Profile</Link>
                        </Button>
                        {roleName === UserRole.ADMIN && (
                            <>
                                <Button
                                    type={
                                        router.pathname.startsWith(routes.user.transaction.deposit()) ||
                                        router.pathname.startsWith(routes.user.transaction.list())
                                            ? 'primary'
                                            : 'default'
                                    }
                                >
                                    <Link href={routes.user.transaction.list()}>Transaction</Link>
                                </Button>
                                <Button type={router.pathname.startsWith(routes.user.notification.list()) ? 'primary' : 'default'}>
                                    <Link href={routes.user.notification.list()}>Notification</Link>
                                </Button>
                            </>
                        )}
                    </div>
                </div>
                <div className="flex flex-col w-full h-full p-4 bg-white">
                    <ul className="flex gap-10 px-4 py-2 text-base font-semibold bg-gray-100 rounded shadow">
                        {router.pathname.startsWith(routes.user.profile.detail()) && roleName === UserRole.ADMIN ? (
                            profileMenu.map((menu) => (
                                <li key={menu.title}>
                                    <Link href={menu.path}>
                                        <a
                                            className={clsx({
                                                'text-primary': router.asPath === menu.path,
                                                'text-black': router.asPath !== menu.path,
                                            })}
                                        >
                                            {menu.title}
                                        </a>
                                    </Link>
                                </li>
                            ))
                        ) : (
                            <li>
                                <Link href={routes.user.profile.detail()}>
                                    <a
                                        className={clsx({
                                            'text-primary': router.asPath === routes.user.profile.detail(),
                                            'text-black': router.asPath !== routes.user.profile.detail(),
                                        })}
                                    >
                                        Profile
                                    </a>
                                </Link>
                            </li>
                        )}
                        {router.pathname.startsWith(routes.user.transaction.list()) &&
                            transactionMenu.map((menu) => (
                                <li key={menu.title}>
                                    <Link href={menu.path}>
                                        <a
                                            className={clsx({
                                                'text-primary': router.asPath === menu.path,
                                                'text-black': router.asPath !== menu.path,
                                            })}
                                        >
                                            {menu.title}
                                        </a>
                                    </Link>
                                </li>
                            ))}

                        {router.pathname.startsWith(routes.user.notification.list()) &&
                            notificationMenu.map((menu) => (
                                <li key={menu.title}>
                                    <Link href={menu.path}>
                                        <a
                                            className={clsx({
                                                'text-primary': router.asPath === menu.path,
                                                'text-black': router.asPath !== menu.path,
                                            })}
                                        >
                                            {menu.title}
                                        </a>
                                    </Link>
                                </li>
                            ))}
                    </ul>
                    <div className="bg-gray-100 rounded shadow ">{children}</div>
                </div>
            </div>
        </div>
    );
};

export default UserSettingLayout;
