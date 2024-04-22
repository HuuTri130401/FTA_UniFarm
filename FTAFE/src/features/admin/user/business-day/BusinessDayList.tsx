import { DashOutlined } from '@ant-design/icons';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { ADMIN_API } from '@core/api/admin.api';
import { BusinessDayAPI } from '@core/api/business-day.api';
import { routes } from '@core/routes';
import { BusinessDay } from '@models/business-day';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Dropdown, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import { PlusIcon } from 'lucide-react';
import moment from 'moment';
import { useRouter } from 'next/router';
import { useState } from 'react';
import { toast } from 'react-toastify';

import CreateBusinessDayModal from './components/CreateBusinessDayModal';

interface BusinessDayListProps {}
const BusinessDayList: React.FC<BusinessDayListProps> = () => {
    const { data, isLoading } = useQuery({ queryKey: ['businessDays'], queryFn: async () => await BusinessDayAPI.getAll() });
    const [createModalState, setCreateModalState] = useState(false);

    const deleteMutation = useMutation(async (id: string) => await BusinessDayAPI.deleteOne(id));

    const queryClient = useQueryClient();
    const router = useRouter();

    const handleDelete = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc chắn không?',
            content: 'Bạn không thể phục hồi dữ liệu này!',
            okText: 'Xoá!',
            okType: 'danger',
            cancelText: 'Huỷ',
            onOk: async () => {
                try {
                    await deleteMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['businessDays']);
                            toast.success('deleted successfully!');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting FarmHub:', error);
                }
            },
        });
    };

    const user = useStoreUser();

    const createTransactionMutation = useMutation(async (id: string) => await ADMIN_API.createTransactionForFarmHubInBusinessDay(id), {
        onSuccess: () => {
            queryClient.invalidateQueries(['businessDays']);
            toast.success('Gửi tiền cho nông trại thành công');
        },
    });
    const listOpenDay: string[] = data?.payload.map((i: BusinessDay) => i.openDay) || [];

    return (
        <div className="flex flex-col w-full gap-2">
            {user.roleName === UserRole.ADMIN && (
                <div className="flex flex-col items-end w-full gap-2 ">
                    <button
                        onClick={() => {
                            setCreateModalState(!createModalState);
                        }}
                        className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                    >
                        <PlusIcon className="w-5 h-5 text-white" />
                        <span>Tạo ngày bán</span>
                    </button>
                </div>
            )}
            <TableBuilder<BusinessDay>
                rowKey="id"
                isLoading={isLoading}
                data={data?.payload || []}
                columns={[
                    {
                        title: () => <TableHeaderCell key="name" label="Tên Sự kiện" />,
                        width: 400,
                        key: 'name',
                        render: ({ ...props }: BusinessDay) => <p className="m-0">{props.name}</p>,
                        sorter: (a, b) => a.name.localeCompare(b.name),
                    },

                    {
                        title: () => <TableHeaderCell key="regiterDay" sortKey="regiterDay" label="Ngày đăng ký" />,
                        width: 400,
                        key: 'regiterDay',
                        render: ({ ...props }: BusinessDay) => {
                            return <p className="m-0">{moment(props.regiterDay).format('DD/MM/YYYY HH:mm:ss')}</p>;
                        },
                        sorter: (a, b) => moment(a.regiterDay).valueOf() - moment(b.regiterDay).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="endOfRegister" sortKey="endOfRegister" label="Ngày Kết thúc đăng ký" />,
                        width: 400,
                        key: 'endOfRegister',
                        render: ({ ...props }: BusinessDay) => {
                            return <p className="m-0">{moment(props.endOfRegister).format('DD/MM/YYYY HH:mm:ss')}</p>;
                        },
                        sorter: (a, b) => moment(a.endOfRegister).valueOf() - moment(b.endOfRegister).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="openDay" sortKey="openDay" label="Ngày mở bán" />,
                        width: 400,
                        key: 'openDay',
                        render: ({ ...props }: BusinessDay) => {
                            return <p className="m-0">{moment(props.openDay).format('DD/MM/YYYY HH:mm:ss')}</p>;
                        },
                        sorter: (a, b) => moment(a.openDay).valueOf() - moment(b.openDay).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                        width: 400,
                        key: 'status',
                        render: ({ ...props }: BusinessDay) => {
                            return (
                                <Tag
                                    className={clsx(`text-sm whitespace-normal`)}
                                    color={
                                        typeof props.status === 'string' && props.status === 'Active'
                                            ? 'geekblue'
                                            : props.status === 'Completed'
                                            ? 'green'
                                            : props.status === 'PaymentConfirm'
                                            ? 'magenta'
                                            : 'volcano'
                                    }
                                >
                                    {props.status === 'Active'
                                        ? 'Hoạt động'
                                        : props.status === 'Completed'
                                        ? 'Hoàn thành'
                                        : props.status === 'PaymentConfirm'
                                        ? 'Chờ thanh toán'
                                        : 'Không hoạt động'}
                                </Tag>
                            );
                        },
                        filters: [
                            { text: 'Active', value: 'Active' },
                            { text: 'Completed', value: 'Completed' },
                            { text: 'PaymentConfirm', value: 'PaymentConfirm' },
                            // Add more filters if needed
                        ],
                        onFilter: (value, record) => record.status === value,
                    },
                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'action',
                        render: ({ ...props }: BusinessDay) => {
                            return user.roleName === UserRole.ADMIN ? (
                                <Dropdown
                                    overlay={
                                        <Menu>
                                            <Menu.Item key="1">
                                                <Button
                                                    style={{
                                                        background: '#bae0ff',
                                                    }}
                                                    onClick={() => {
                                                        router.push(`/admin/business-day/${props.id}`);
                                                    }}
                                                >
                                                    Thống kê
                                                </Button>
                                            </Menu.Item>
                                            <Menu.Item key="2">
                                                <Button
                                                    type="primary"
                                                    disabled={props.status !== 'PaymentConfirm'}
                                                    onClick={() => {
                                                        createTransactionMutation.mutateAsync(props.id);
                                                    }}
                                                >
                                                    Chuyển tiền cho nông trại
                                                </Button>
                                            </Menu.Item>

                                            <Menu.Item key="3">
                                                <Button type="primary" danger onClick={() => handleDelete(props.id)}>
                                                    Xoá
                                                </Button>
                                            </Menu.Item>
                                        </Menu>
                                    }
                                    trigger={['click']}
                                >
                                    <DashOutlined />
                                </Dropdown>
                            ) : (
                                <Button
                                    onClick={() =>
                                        router.push({
                                            pathname: routes.staff.businessDay.batchList(props.id),
                                            query: {
                                                name: props.name,
                                            },
                                        })
                                    }
                                >
                                    Xem danh sách chuyến hàng
                                </Button>
                            );
                        },
                    },
                ]}
            />

            {user.roleName === UserRole.ADMIN && (
                <CreateBusinessDayModal
                    listOpenDate={listOpenDay}
                    open={createModalState}
                    onCancel={() => setCreateModalState(false)}
                    afterClose={() => setCreateModalState(false)}
                />
            )}
        </div>
    );
};
export default BusinessDayList;
