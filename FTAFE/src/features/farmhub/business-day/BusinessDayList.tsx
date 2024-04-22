import { TableBuilder, TableHeaderCell } from '@components/tables';
import { BusinessDayAPI } from '@core/api/business-day.api';
import { BusinessDay } from '@models/business-day';
import { useStoreUser } from '@store/index';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, Tag } from 'antd';
import clsx from 'clsx';
import moment from 'moment';
import { useRouter } from 'next/router';
import { useState } from 'react';
import { toast } from 'react-toastify';

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

    return (
        <div className="flex flex-col w-full gap-2">
            {/* <div className="flex flex-col items-end w-full gap-2 ">
            <button
                    onClick={() => {
                        setCreateModalState(!createModalState);
                    }}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Tạo ngày bán</span>
                </button>
            </div> */}
            <TableBuilder<BusinessDay>
                rowKey="id"
                isLoading={isLoading}
                data={data?.payload}
                columns={[
                    {
                        title: () => <TableHeaderCell key="name" sortKey="name" label="Tên sự kiện" />,
                        width: 400,
                        key: 'name',
                        render: ({ ...props }: BusinessDay) => <p className="m-0">{props.name}</p>,
                    },

                    {
                        title: () => <TableHeaderCell key="regiterDay" sortKey="regiterDay" label="Ngày đăng ký" />,
                        width: 400,
                        key: 'regiterDay',
                        render: ({ ...props }: BusinessDay) => {
                            return <p className="m-0">{moment(props.regiterDay).format('DD/MM/YYYY')}</p>;
                        },
                    },
                    {
                        title: () => <TableHeaderCell key="endOfRegister" sortKey="endOfRegister" label="Ngày kết thúc đăng ký" />,
                        width: 400,
                        key: 'endOfRegister',
                        render: ({ ...props }: BusinessDay) => {
                            return <p className="m-0">{moment(props.endOfRegister).format('DD/MM/YYYY')}</p>;
                        },
                    },
                    {
                        title: () => <TableHeaderCell key="openDay" sortKey="openDay" label="Ngày mở bán" />,
                        width: 400,
                        key: 'openDay',
                        render: ({ ...props }: BusinessDay) => {
                            return <p className="m-0">{moment(props.openDay).format('DD/MM/YYYY')}</p>;
                        },
                    },
                    {
                        title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
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
                    },
                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'action',
                        render: ({ ...props }: BusinessDay) => {
                            // return (
                            //     <Dropdown
                            //         overlay={
                            //             <Menu>
                            //                 <Menu.Item key="1">
                            //                     <Button
                            //                         onClick={() => {
                            //                             router.push(`/admin/business-day/${props.id}`);
                            //                         }}
                            //                     >
                            //                         Menu
                            //                     </Button>
                            //                 </Menu.Item>
                            //                 <Menu.Item key="2">
                            //                     <Button onClick={() => {}}>Edit</Button>
                            //                 </Menu.Item>

                            //                 <Menu.Item key="3">
                            //                     <Button onClick={() => handleDelete(props.id)}>Delete</Button>
                            //                 </Menu.Item>
                            //             </Menu>
                            //         }
                            //         trigger={['click']}
                            //     >
                            //         <DashOutlined />
                            //     </Dropdown>
                            // );
                            return (
                                <Button
                                    type="primary"
                                    onClick={() => {
                                        router.push(`/farmhub/${props.id}`);
                                    }}
                                >
                                    Xem chi tiết
                                </Button>
                            );
                        },
                    },
                ]}
            />
        </div>
    );
};
export default BusinessDayList;
