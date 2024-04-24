import {
    AreaChartOutlined,
    CalendarOutlined,
    ClockCircleOutlined,
    CreditCardOutlined,
    FieldTimeOutlined,
    HomeOutlined,
    MenuFoldOutlined,
    MenuOutlined,
    MenuUnfoldOutlined,
    TagsOutlined,
    UserOutlined,
    WalletOutlined,
} from '@ant-design/icons';
import { routes } from '@core/routes';
import { ArchiveBoxIcon } from '@heroicons/react/24/outline';
import { useLogoutMutation } from '@hooks/api/auth.hook';
import { UserRole } from '@models/user';
import { Person, PersonCheck, SignOut } from 'akar-icons';
import { Button, Dropdown, Layout, Menu } from 'antd';
import { ItemType } from 'antd/lib/menu/hooks/useItems';
import clsx from 'clsx';
import Link from 'next/link';
import { useRouter } from 'next/router';
import * as React from 'react';

import { useStoreUser } from '../../store';
import { CommonSeo } from '../commons';

const { Sider, Content } = Layout;

const menuAdminList: ItemType[] = [
    {
        label: <div style={{ fontSize: 'larger', fontWeight: 'bold', textAlign: 'center', color: 'green', cursor: 'default' }}>Quản Trị Viên</div>,
        key: routes.admin.admin(),
        style: {
            pointerEvents: 'none', // Ensures the item is not clickable
            cursor: 'default', // Changes the cursor to default to indicate non-interactivity
            background: 'none', // Ensures there is no background color or effect
            border: 'none', // Removes any borders that might change on interaction
        },
        // This item is purely decorative and not intended for interaction
    },
    {
        icon: <AreaChartOutlined />,
        label: 'Bảng điều khiển',
        key: routes.admin.home(),
    },
    {
        icon: <FieldTimeOutlined />,
        label: 'Ngày bán',
        key: routes.admin.businessDay(),
    },
    {
        icon: <FieldTimeOutlined />,
        label: 'Lịch sử đặt hàng',
        key: routes.admin.order.list(),
    },
    {
        icon: <UserOutlined />,
        label: 'Tài khoản',
        key: 'user',
        children: [
            // {
            //     key: routes.admin.user.staff.list(),
            //     label: 'Nhân viên',
            // },
            {
                key: routes.admin.user.customer.list(),
                label: 'Khách hàng',
            },
            {
                label: 'Trang trại',
                key: routes.admin.user.farm_hub.list(),
            },
            {
                label: 'Trạm Chuyển Hàng',
                key: routes.admin.user.station.list(),
            },
            {
                label: 'Kho Hàng',
                key: routes.admin.user.collected_hub_staff.list(),
            },
        ],
    },

    {
        icon: <TagsOutlined />,
        label: 'Bán Sản Phẩm',
        key: 'Menu',
        children: [
            {
                key: routes.admin.category.list(),
                label: 'Chuyên mục',
            },
            {
                label: 'Sản phẩm',
                key: routes.admin.product.list(),
            },
            // {
            //     label: 'Đơn hàng',
            //     key: routes.admin.order.list(),
            // },
            // {
            //     label: 'Menu',
            //     key: routes.admin.menu.list(),
            // },
        ],
    },
    {
        icon: <WalletOutlined />,
        label: 'Danh sách rút tiền',
        key: routes.admin.payment.home(),
    },
    {
        icon: <ClockCircleOutlined />,
        label: 'Lịch sử giao dịch',
        key: routes.admin.payment.transaction(),
    },

    {
        icon: <AreaChartOutlined />,
        label: 'Khu vực',
        key: routes.admin.area(),
    },
    {
        icon: <HomeOutlined />,
        label: 'Căn hộ',
        key: routes.admin.apartment(),
    },
];

const menuUndefined: ItemType[] = [];
const menuStaffList: ItemType[] = [
    {
        label: <div style={{ fontSize: 'larger', fontWeight: 'bold', textAlign: 'center', color: 'green', cursor: 'default' }}>Kho</div>,
        key: routes.staff.home1(),
        style: {
            pointerEvents: 'none', // Ensures the item is not clickable
            cursor: 'default', // Changes the cursor to default to indicate non-interactivity
            background: 'none', // Ensures there is no background color or effect
            border: 'none', // Removes any borders that might change on interaction
        },
        // This item is purely decorative and not intended for interaction
    },
    {
        icon: <AreaChartOutlined />,
        label: 'Ngày bán',
        key: routes.staff.businessDay.list(),
    },
    {
        icon: <AreaChartOutlined />,
        label: 'Tình trạng hàng hóa',
        key: routes.staff.home(),
    },
    {
        icon: <AreaChartOutlined />,
        label: 'Danh sách chuyến hàng',
        key: routes.staff.transfer.list(),
    },
    {
        icon: <AreaChartOutlined />,
        label: 'Danh sách trạm vận chuyển',
        key: routes.staff.transfer.station(),
    },
];
const menuFarmHubList: ItemType[] = [
    // {
    //     icon: <AreaChartOutlined />,
    //     label: 'Dashboard',
    //     key: routes.farmhub.home(),
    // },
    {
        label: <div style={{ fontSize: 'larger', fontWeight: 'bold', textAlign: 'center', color: 'green', cursor: 'default' }}>Trang Trại</div>,
        key: routes.farmhub.home1(),
        style: {
            pointerEvents: 'none', // Ensures the item is not clickable
            cursor: 'default', // Changes the cursor to default to indicate non-interactivity
            background: 'none', // Ensures there is no background color or effect
            border: 'none', // Removes any borders that might change on interaction
        },
        // This item is purely decorative and not intended for interaction
    },
    {
        key: routes.farmhub.home(),
        label: 'Ngày bán',
        icon: <CalendarOutlined />,
    },
    {
        label: 'Sản phẩm',
        key: routes.farmhub.product.list(),
        icon: <ArchiveBoxIcon className="w-4 h-4 text-jacarta-700 dark:text-white" strokeWidth={2} />,
    },
    {
        label: 'Danh sách bán',
        key: routes.farmhub.menu.list(),
        icon: <MenuOutlined />,
    },
    {
        icon: <CreditCardOutlined />,
        label: 'Giao dịch',
        key: 'transactionpayment',
        children: [
            {
                icon: <ClockCircleOutlined />,
                key: routes.farmhub.transaction.list(),
                label: 'Lịch sử giao dịch',
            },
            {
                icon: <AreaChartOutlined />,
                key: routes.farmhub.payment.list(),
                label: 'Rút tiền',
            },
        ],
    },
    // {
    //     icon: <DatabaseOutlined />,
    //     label: 'Danh sách sản phẩm',
    //     key: 'product',
    //     children: [
    //         {
    //             label: 'Sản phẩm',
    //             key: routes.farmhub.product.list(),
    //             icon: <ArchiveBoxIcon className="w-4 h-4 text-jacarta-700 dark:text-white" strokeWidth={2} />,
    //         },
    //         {
    //             label: 'Menu',
    //             key: routes.farmhub.menu.list(),
    //             icon: <MenuOutlined />,
    //         },
    //         {
    //             key: routes.farmhub.businessDay.list(),
    //             label: 'Ngày bán',
    //             icon: <CalendarOutlined />,
    //         },
    //         {
    //             label: 'Lô hàng',
    //             key: routes.farmhub.batch.list(),
    //             icon: <OrderedListOutlined />,
    //         },
    //     ],
    // },
];
interface DashboardLayoutProps {
    children: React.ReactNode;
}

export const DashboardLayout: React.FC<DashboardLayoutProps> = ({ children }) => {
    const [collapsed, setCollapsed] = React.useState(false);
    const router = useRouter();
    const user = useStoreUser();

    const { mutationLogout } = useLogoutMutation();
    const [isOpen, setIsOpen] = React.useState(false);
    const notifications: any[] = [];

    React.useEffect(() => {
        window.addEventListener('scroll', (_) => setIsOpen(false));
    }, []);

    const adminMenu: ItemType[] = [
        {
            label: (
                <Link href={`${routes.user.detail()}`}>
                    <div className="flex items-center justify-start w-full gap-2 px-1 py-2 space-x-2 transition-colors cursor-pointer dark:hover:bg-jacarta-600 hover:text-primary/80 focus:text-accent hover:bg-jacarta-50 rounded-xl">
                        <Person strokeWidth={2} size={24} />
                        Quản trị
                    </div>
                </Link>
            ),
            key: 'Manage',
        },

        {
            label: (
                <div className="flex items-center justify-start w-full gap-2 px-1 py-2 space-x-2 transition-colors cursor-pointer dark:hover:bg-jacarta-600 hover:text-primary/80 focus:text-accent hover:bg-jacarta-50 rounded-xl">
                    <SignOut strokeWidth={2} size={24} />
                    Đăng xuất
                </div>
            ),
            key: 'logout',
            onClick: () => mutationLogout(),
        },
    ];

    return (
        <>
            <CommonSeo title="Dashboard" />
            <div className="relative root">
                <Layout hasSider>
                    <Sider
                        width={256}
                        theme="light"
                        trigger={null}
                        collapsible
                        collapsed={collapsed}
                        className="!fixed top-0 bottom-0 left-0 flex flex-col h-screen overflow-auto shadow-lg"
                    >
                        <div className="flex flex-col justify-between w-full bg-white ">
                            <div className="flex flex-col">
                                <Link href={routes.home()}>
                                    <div className="flex items-center justify-center w-full px-2 pb-0 transition-all duration-500 ease-in-out bg-white cursor-pointer logo">
                                        {collapsed ? (
                                            <div className="flex items-center justify-center">
                                                <img
                                                    src="https://media.discordapp.net/attachments/1210447037456195595/1226257940902707240/Artboard_1.png?ex=662d5733&is=661ae233&hm=561dfb8eb355dbab0d61ef6c287c5ad3cd6ca4843006f2532176673338e3eb23&=&format=webp&quality=lossless&width=630&height=593"
                                                    className="w-12 h-12 fade-in"
                                                    alt="LivelyCV"
                                                />
                                            </div>
                                        ) : (
                                            <img src={'/assets/images/logo/logo-main2.png'} className="object-cover h-13 fade-in" alt="LivelyCV" />
                                        )}
                                    </div>
                                </Link>

                                <Menu
                                    mode="inline"
                                    onClick={({ key }) => {
                                        router.push(key);
                                    }}
                                    defaultSelectedKeys={[router.pathname]}
                                    items={
                                        user.roleName === UserRole.ADMIN
                                            ? menuAdminList
                                            : user.roleName === UserRole.FARM_HUB
                                            ? menuFarmHubList
                                            : user.roleName === UserRole.COLLECTED_STAFF
                                            ? menuStaffList
                                            : menuUndefined
                                    }
                                />
                            </div>
                        </div>
                    </Sider>
                    <Layout
                        className={clsx('duration-300', {
                            'ml-20': collapsed,
                            'ml-64': !collapsed,
                        })}
                    >
                        <div className="flex items-center justify-between h-16 p-2 space-x-8 bg-white">
                            <Button
                                onClick={() => setCollapsed(!collapsed)}
                                type="primary"
                                icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                                size="middle"
                            />
                            <div className="flex gap-5 ">
                                <Dropdown
                                    trigger={['hover']}
                                    placement="bottomLeft"
                                    overlay={
                                        <Menu
                                            className="min-w-[140px]"
                                            items={
                                                user.roleName === UserRole.ADMIN
                                                    ? adminMenu
                                                    : [
                                                          {
                                                              label: (
                                                                  <Link href={`${routes.user.detail()}`}>
                                                                      <div className="flex items-center justify-start w-full gap-2 px-1 py-2 space-x-2 transition-colors cursor-pointer dark:hover:bg-jacarta-600 hover:text-primary/80 focus:text-accent hover:bg-jacarta-50 rounded-xl">
                                                                          <Person strokeWidth={2} size={24} />
                                                                          Thông tin
                                                                      </div>
                                                                  </Link>
                                                              ),
                                                              key: 'profile',
                                                          },

                                                          {
                                                              label: (
                                                                  <div className="flex items-center justify-start w-full gap-2 px-1 py-2 space-x-2 transition-colors cursor-pointer dark:hover:bg-jacarta-600 hover:text-primary/80 focus:text-accent hover:bg-jacarta-50 rounded-xl">
                                                                      <SignOut strokeWidth={2} size={24} />
                                                                      Đăng xuất
                                                                  </div>
                                                              ),
                                                              key: 'logout',
                                                              onClick: () => mutationLogout(),
                                                          },
                                                      ]
                                            }
                                        />
                                    }
                                >
                                    <div className="flex items-center justify-center gap-2 duration-300 cursor-pointer text-jacarta-700 ">
                                        <button className="flex items-center gap-2 hover:scale-105" onClick={() => setIsOpen(false)}>
                                            <div className="flex flex-col items-start gap-1">
                                                <p className="m-0 text-base font-semibold capitalize whitespace-nowrap">
                                                    <span>
                                                        {user.roleName === UserRole.ADMIN
                                                            ? 'Admin: '
                                                            : user.roleName === UserRole.FARM_HUB
                                                            ? 'FarmHub: '
                                                            : user.roleName === UserRole.COLLECTED_STAFF
                                                            ? 'Staff: '
                                                            : 'User: '}
                                                    </span>{' '}
                                                    {user.firstName + ' ' + user.lastName}
                                                </p>
                                                {user.roleName === UserRole.FARM_HUB && (
                                                    <p className="m-0 text-base font-bold">
                                                        <span>Số dư: </span>
                                                        {user.wallet?.balance.toLocaleString('en')} VNĐ
                                                    </p>
                                                )}
                                            </div>
                                            <PersonCheck strokeWidth={2} className="w-6 h-6 text-jacarta-700 dark:text-white " />
                                        </button>
                                    </div>
                                </Dropdown>
                            </div>
                        </div>
                        <Content>
                            <div className="min-h-screen">{children}</div>
                        </Content>
                    </Layout>
                </Layout>
            </div>
        </>
    );
};
