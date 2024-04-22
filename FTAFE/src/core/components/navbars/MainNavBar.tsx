import { constant } from '@core/constant';
import { routes } from '@core/routes';
import { BellIcon } from '@heroicons/react/24/outline';
import { BellAlertIcon } from '@heroicons/react/24/solid';
import { useLogoutMutation } from '@hooks/api/auth.hook';
import { UserRole } from '@models/user';
import WhiteLogo from '@public/images/logo_white.png';
import { useStoreUser, useStoreWallet } from '@store/index';
import { isChildrenPageActive } from '@utils/dynamicNavigation';
import { Person, PersonCheck, SignOut } from 'akar-icons';
import { Empty } from 'antd';
import clsx from 'clsx';
import moment from 'moment';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/router';
import * as React from 'react';
import { v4 as uuidv4 } from 'uuid';

interface MainNavbarProps {}

interface NavLink {
    id: string;
    name: string;
    path: string;
}

const defaultLinks: NavLink[] = [
    {
        id: uuidv4(),
        name: 'Trang chủ',
        path: routes.home(),
    },
    // {
    //     id: uuidv4(),
    //     name: 'Dashboard',
    //     path: routes.home(),
    // },
    {
        id: uuidv4(),
        name: 'Dịch vụ',
        path: routes.home(),
    },
    {
        id: uuidv4(),
        name: 'Liên hệ',
        path: routes.home(),
    },
];

const MainNavbar: React.FunctionComponent<MainNavbarProps> = () => {
    const route = useRouter();
    const user = useStoreUser();

    const { mutationLogout } = useLogoutMutation();

    const notifications: any[] = [];

    const wallet = useStoreWallet();
    const [links, setLinks] = React.useState<NavLink[]>(defaultLinks);

    const [isOpen, setIsOpen] = React.useState(false);
    const [token, setToken] = React.useState<string>('');
    React.useEffect(() => {
        setToken(localStorage.getItem(constant.TOKEN_KEY) || '');
    }, []);

    React.useEffect(() => {
        const newLinks = [...defaultLinks];

        if (user.roleName === UserRole.ADMIN) {
            newLinks.push({
                id: uuidv4(),
                name: 'Bảng điều khiển',
                path: routes.admin.home(),
            });
        }
        if (user.roleName === UserRole.FARM_HUB) {
            newLinks.push({
                id: uuidv4(),
                name: 'Bảng điều khiển',
                path: routes.farmhub.home(),
            });
        }
        if (user.roleName === UserRole.COLLECTED_STAFF) {
            newLinks.push({
                id: uuidv4(),
                name: 'Bảng điều khiển',
                path: routes.staff.home(),
            });
        }
        setLinks(newLinks);
    }, [user.roleName]);

    React.useEffect(() => {
        window.addEventListener('scroll', (_) => setIsOpen(false));
    }, []);
    const userLinks = [
        {
            id: 'link-manage',
            name: 'Quản lí',
            path:
                user.roleName === UserRole.ADMIN
                    ? routes.admin.home()
                    : user.roleName === UserRole.FARM_HUB
                    ? routes.farmhub.home()
                    : routes.staff.home(),
            icon: <Person strokeWidth={2} size={24} />,
        },
        {
            id: 'link-logout',
            name: 'Đăng xuất',
            path: '',
            onClick: mutationLogout,
            icon: <SignOut strokeWidth={2} size={24} />,
        },
    ];

    return (
        <>
            <header className="fixed top-0 z-20 w-full transition-colors js-page-header drop-shadow-md !bg-[#ffffffe6] dark:!bg-transparent dark:backdrop-blur">
                <div className="flex items-center px-6 py-4 xl:px-24">
                    <Link className="shrink-0" href={routes.home()}>
                        <a>
                            <div className="dark:hidden">
                                {/* <Image src={'/assets/images/logo/logo-new.png'} height={28} width={130} alt="Live CV" className="h-auto max-h-7 " /> */}
                                <img
                                    src={'/assets/images/logo/logo-main.png'}
                                    alt="Uni Farm"
                                    className="object-cover h-auto max-h-14"
                                    style={{ filter: 'drop-shadow(0px 0px 10px rgba(0, 0, 0, 0.1))' }}
                                />
                            </div>
                            <div className="hidden dark:block">
                                <Image src={WhiteLogo} height={28} width={130} alt="Live CV" />
                            </div>
                        </a>
                    </Link>
                    {/* End  logo */}

                    <nav className="flex justify-end w-full lg:justify-center navbar">
                        <ul className="flex flex-row gap-10 m-0 lg:gap-0">
                            {/* create */}
                            {links.map((link) => (
                                <li className="group" key={link.id}>
                                    <Link href={link.path}>
                                        <a>
                                            <button className="text-jacarta-700 font-display hover:text-primary/80 focus:text-accent dark:hover:text-primary/80 dark:focus:text-accent flex items-center justify-between py-3.5 text-base dark:text-white lg:px-5 capitalize">
                                                <span
                                                    className={clsx({
                                                        'dark:text-accent-50':
                                                            isChildrenPageActive(route.asPath, link.path) ||
                                                            (route.asPath.includes(link.path) && link.path !== '/'),
                                                    })}
                                                >
                                                    {link.name}
                                                </span>
                                            </button>
                                        </a>
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    </nav>

                    <div className="z-10 flex items-center ml-auto bg-white opacity-100 js-mobile-menu dark:bg-jacarta-800 lg:relative lg:inset-auto lg:bg-transparent lg:opacity-100 dark:lg:bg-transparent ">
                        <div className="flex items-center gap-4 ml-8 xl:ml-12">
                            <div className="relative js-nav-dropdown group-dropdown">
                                {token ? (
                                    <>
                                        <button className="flex items-center gap-2 hover:scale-105" onClick={() => setIsOpen(false)}>
                                            <span className="text-base font-semibold capitalize whitespace-nowrap">
                                                {user.firstName + ' ' + user.lastName}
                                            </span>
                                            <PersonCheck strokeWidth={2} className="w-6 h-6 text-jacarta-700 dark:text-white " />
                                        </button>

                                        <div className="dropdown-menu dark:bg-jacarta-800 group-dropdown-hover:opacity-100 group-dropdown-hover:visible !-right-4 !top-[95%] !left-auto min-w-[14rem] whitespace-nowrap rounded-xl bg-white transition-all will-change-transform before:absolute before:-top-3 before:h-3 before:w-full lg:absolute lg:grid lg:!translate-y-4 lg:py-4 lg:px-2 lg:shadow-2xl hidden lg:invisible lg:opacity-0 z-[50]">
                                            {userLinks.map((link) => (
                                                <Link href={link.path} key={link.id}>
                                                    {link.name === 'Đăng xuất' ? (
                                                        <div
                                                            className="flex items-center px-5 py-2 space-x-2 transition-colors cursor-pointer dark:hover:bg-jacarta-600 hover:text-primary/80 focus:text-accent hover:bg-jacarta-50 rounded-xl group"
                                                            key={link.id}
                                                            onClick={() => mutationLogout()}
                                                        >
                                                            {link.icon}
                                                            <span className="mt-1 text-sm transition-colors font-display text-jacarta-700 dark:text-white group-hover:text-primary/80">
                                                                {link.name}
                                                            </span>
                                                        </div>
                                                    ) : (
                                                        <div className="flex items-center px-5 py-2 space-x-2 transition-colors cursor-pointer dark:hover:bg-jacarta-600 hover:text-primary/80 focus:text-accent hover:bg-jacarta-50 rounded-xl group">
                                                            {link.icon}
                                                            <span className="mt-1 text-sm transition-colors font-display text-jacarta-700 dark:text-white group-hover:text-primary/80">
                                                                {link.name}
                                                            </span>
                                                        </div>
                                                    )}
                                                </Link>
                                            ))}
                                        </div>
                                    </>
                                ) : (
                                    <Link href={routes.auth.login()}>
                                        <button className="bg-primary whitespace-nowrap hover:bg-primary/90 focus:bg-accent group rounded-full py-2 px-8 text-center font-semibold text-white cursor-pointer transition-all hover:border-transparent focus:border-transparent dark:border-transparent dark:bg-white/[.15] hover:scale-105 ">
                                            Đăng nhập
                                        </button>
                                    </Link>
                                )}
                            </div>
                            {user.isLogin && (
                                <div className="relative z-50 noti-history">
                                    <button className="flex items-center gap-2 hover:scale-105 noti-button" onClick={() => setIsOpen(!isOpen)}>
                                        {isOpen ? (
                                            <BellAlertIcon className="w-6 h-6 text-primary-700 dark:text-white noti-button" />
                                        ) : (
                                            <BellIcon className="w-6 h-6 text-jacarta-700 dark:text-white noti-button" />
                                        )}
                                    </button>
                                    {isOpen && (
                                        <div className="!-right-4 !top-[95%] !left-auto min-w-[14rem] whitespace-nowrap rounded-xl bg-white transition-all will-change-transform before:absolute before:-top-3 before:h-3 before:w-full lg:absolute lg:grid lg:!translate-y-4 lg:py-4 lg:px-2 lg:shadow-2xl !z-[9999] overflow-y-auto w-[450px] h-[600px] custom-scrollbar_notihistory">
                                            <div className="flex flex-col gap-2 pl-4 pr-2">
                                                <h3 className="text-xl font-bold">Notifications History</h3>
                                                <div className="flex items-center justify-between">
                                                    <span className="pb-1.5 text-base font-semibold text-black">Before that</span>
                                                    {/* {user.type === UserRole.EXPERT && (
                                                        <Button
                                                            type="link"
                                                            style={{
                                                                fontSize: '14px',
                                                                fontWeight: 700,
                                                                lineHeight: '16px',
                                                                textDecoration: 'underline',
                                                            }}
                                                            onClick={() => {
                                                                setIsOpen(false);
                                                                route.push(routes.expert.notification.list());
                                                            }}
                                                        >
                                                            View All
                                                        </Button>
                                                    )}
                                                    {user.type === UserRole.CANDIDATE && (
                                                        <Button
                                                            type="link"
                                                            style={{
                                                                fontSize: '14px',
                                                                fontWeight: 700,
                                                                lineHeight: '16px',
                                                                textDecoration: 'underline',
                                                            }}
                                                            onClick={() => {
                                                                setIsOpen(false);
                                                                route.push(routes.user.notification.list());
                                                            }}
                                                        >
                                                            View All
                                                        </Button>
                                                    )} */}
                                                </div>
                                                {Boolean(notifications?.length) ? (
                                                    notifications?.map((item, idx) => (
                                                        <div
                                                            className={`flex flex-col gap-1 rounded-md px-2 ${
                                                                idx % 2 === 0 ? 'bg-[#eeeaf9]' : 'bg-white'
                                                            }`}
                                                            key={item.id}
                                                        >
                                                            <span className="text-base font-semibold text-black capitalize">{item.title}</span>
                                                            <span className="px-3 text-sm">{item.body}</span>
                                                            <span className="px-3 text-xs">{moment(item.createdAt).fromNow()}</span>
                                                            <hr className="h-px my-2 bg-gray-300 border-0 dark:bg-gray-700" />
                                                        </div>
                                                    ))
                                                ) : (
                                                    <Empty
                                                        image={Empty.PRESENTED_IMAGE_SIMPLE}
                                                        description={<span className="text-gray-900">No notification</span>}
                                                    />
                                                )}
                                            </div>
                                        </div>
                                    )}
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </header>
        </>
    );
};

export default MainNavbar;
