import { WarningOutlined } from '@ant-design/icons';
import { TableActionCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { MenuAPI } from '@core/api/menu.api';
import { routes } from '@core/routes';
import { useQueryGetAllMenus } from '@hooks/api/farmhub.hook';
import { Menu } from '@models/menu';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Modal, Tag } from 'antd';
import clsx from 'clsx';
import { PlusIcon } from 'lucide-react';
import Link from 'next/link';
import React from 'react';
import { toast } from 'react-toastify';
import CreateMenuModal from './CreateMenuModal';
import UpdateMenuModal from './UpdateMenuModal';

interface MenuListProps {}

const MenuList: React.FC<MenuListProps> = () => {
    const { data, isLoading } = useQueryGetAllMenus();

    const [openCreateModalState, setOpenCreateModalState] = React.useState<boolean>(false);
    const [openUpdateModalState, setOpenUpdateModalState] = React.useState<boolean>(false);

    const [productValue, setProductValue] = React.useState<Menu>({
        id: '',
        name: '',
        tag: '',
        status: '',
        createdAt: '',
        updatedAt: '',
        businessDayId: '',
        farmHubId: '',
    });

    const deleteMenu = useMutation(async (id: string) => await MenuAPI.deleteMenu(id));

    const queryClient = useQueryClient();
    const handleDeleteMenu = (id: string) => {
        Modal.confirm({
            title: 'Bạn có muốn xoá?',
            content: 'Hành động này sẽ không thể hoàn tác lại được!',
            okText: 'Xoá!',
            okType: 'danger',
            icon: <WarningOutlined style={{ color: 'red' }} />,
            cancelText: 'Trở lại',
            onOk: async () => {
                try {
                    await deleteMenu.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['menus']);
                            toast.success('Xoá thành công!');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting Menu:', error);
                }
            },
        });
    };

    return (
        <>
            <div className="flex flex-col w-full gap-10">
                <div className="flex flex-col items-end w-full gap-2 ">
                    <button
                        type="button"
                        onClick={() => {
                            setOpenCreateModalState(!openCreateModalState);
                        }}
                        className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                    >
                        <PlusIcon className="w-5 h-5 text-white" />
                        <span>
                            <strong>Thêm Menu</strong>
                        </span>
                    </button>
                </div>

                <TableBuilder<Menu>
                    rowKey="id"
                    isLoading={isLoading}
                    data={data?.payload || []}
                    columns={[
                        {
                            title: () => <TableHeaderCell key="name" sortKey="name" label="Tên Menu" />,
                            width: 400,
                            key: 'name',
                            render: ({ ...props }: Menu) => <span>{props.name}</span>,
                        },
                        {
                            title: () => <TableHeaderCell key="tag" sortKey="tag" label="Mã Tag" />,
                            width: 400,
                            key: 'code',
                            render: ({ ...props }: Menu) => <span>{props.tag}</span>,
                        },
                        {
                            title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
                            width: 400,
                            key: 'status',
                            render: ({ ...props }: Menu) => {
                                return (
                                    <Tag
                                        className={clsx(`text-sm whitespace-normal`)}
                                        color={typeof props.status === 'string' && props.status === 'Active' ? 'geekblue' : 'volcano'}
                                    >
                                        {props.status === 'Active' ? 'Hoạt động' : props.status === 'PENDING' ? 'Đang chờ' : 'Không hoạt động'}
                                    </Tag>
                                );
                            },
                        },
                        //action delete
                        {
                            title: () => <TableHeaderCell key="action" sortKey="action" label="Hành động" />,
                            width: 400,
                            key: 'action',
                            render: ({ ...props }: Menu) => {
                                return (
                                    <TableActionCell
                                        label="Chỉnh Sửa"
                                        actions={[
                                            {
                                                label: <div className="py-2 text-center text-white cursor-pointer bg-primary">Thay Đổi</div>,
                                                onClick: () => {
                                                    setOpenUpdateModalState(!openUpdateModalState);
                                                    setProductValue(props);
                                                },
                                            },
                                            {
                                                label: <div className="py-2 text-center text-white bg-red-500 cursor-pointer">Xoá</div>,
                                                onClick: () => handleDeleteMenu(props.id),
                                            },
                                        ]}
                                    />
                                );
                            },
                        },
                        {
                            title: () => <TableHeaderCell key="" sortKey="" label="" />,
                            width: 400,
                            key: 'detail',
                            render: ({ ...props }: Menu) => {
                                return (
                                    <div>
                                        <Link
                                            href={routes.farmhub.menu.detail(props.id)}
                                            className="py-2 text-center text-white cursor-pointer bg-primary"
                                        >
                                            Xem sản phẩm
                                        </Link>
                                    </div>
                                );
                            },
                        },
                    ]}
                />
            </div>
            <CreateMenuModal open={openCreateModalState} closeMenuModal={() => setOpenCreateModalState(false)} />
            <UpdateMenuModal open={openUpdateModalState} closeMenuModal={() => setOpenUpdateModalState(false)} currentMenu={productValue} />
        </>
    );
};

export default MenuList;
