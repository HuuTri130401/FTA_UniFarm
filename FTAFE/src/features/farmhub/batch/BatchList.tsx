import { TableBuilder, TableHeaderCell } from '@components/tables';
import { BusinessDayAPI } from '@core/api/business-day.api';
import { routes } from '@core/routes';
import { useQueryGetListOrderByFarmHub } from '@hooks/api/batch.hook';
import { Batch } from '@models/batch';
import { useStoreUser } from '@store/index';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Modal, Tag } from 'antd';
import { PlusIcon } from 'lucide-react';
import moment from 'moment';
import Link from 'next/link';
import { useRouter } from 'next/router';
import { useState } from 'react';
import { toast } from 'react-toastify';

interface BatchListFarmHubProps {}
const BatchListFarmHub: React.FC<BatchListFarmHubProps> = () => {
    // const { data, isLoading } = useQuery({ queryKey: ['businessDays'], queryFn: async () => await BusinessDayAPI.getAll() });
    const user = useStoreUser();

    const { data, isLoading } = useQueryGetListOrderByFarmHub(user.farmHub?.id as string);
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

    return (
        <>
            <div className="flex flex-col w-full gap-2">
                <div className="flex flex-col items-end w-full gap-2 ">
                    <button
                        onClick={() => {
                            setCreateModalState(!createModalState);
                        }}
                        className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                    >
                        <PlusIcon className="w-5 h-5 text-white" />
                        <span>Tạo lô hàng</span>
                    </button>
                </div>

                <TableBuilder<Batch>
                    rowKey="id"
                    isLoading={false}
                    data={data}
                    columns={[
                        {
                            title: () => <TableHeaderCell key="collectedHubName" sortKey="collectedHubName" label="Tên kho nhập hàng" />,
                            width: 300,
                            sorter: (a, b) => a.collectedHubName.localeCompare(b.collectedHubName),
                            key: 'collectedHubName',
                            render: ({ ...props }: Batch) => <p className="m-0">{props.collectedHubName}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="collectedHubAddress" sortKey="collectedHubAddress" label="Địa chỉ kho nhập hàng" />,
                            width: 400,
                            sorter: (a, b) => a.collectedHubAddress.localeCompare(b.collectedHubAddress),
                            key: 'collectedHubAddress',
                            render: ({ ...props }: Batch) => <p className="m-0">{props.collectedHubAddress}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="businessDayName" sortKey="businessDayName" label="Ngày đăng bán" />,
                            width: 300,
                            sorter: (a, b) => a.businessDayName.localeCompare(b.businessDayName),
                            key: 'businessDayName',
                            render: ({ ...props }: Batch) => <p className="m-0">{props.businessDayName}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="businessDayOpen" sortKey="businessDayOpen" label="Ngày mở bán" />,
                            width: 200,
                            key: 'businessDayOpen',
                            sorter: (a, b) => a.businessDayOpen.localeCompare(b.businessDayOpen),
                            render: ({ ...props }: Batch) => <p className="m-0">{moment(props.businessDayOpen).format('DD/MM/YYYY')}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="farmShipDate" sortKey="farmShipDate" label="Ngày giao hàng" />,
                            width: 200,
                            key: 'businessDayOpen',
                            sorter: (a, b) => a.farmShipDate.localeCompare(b.farmShipDate),
                            render: ({ ...props }: Batch) => <p className="m-0">{moment(props.farmShipDate).format('DD/MM/YYYY')}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="collectedHubReceiveDate" sortKey="collectedHubReceiveDate" label="Ngày nhận hàng" />,
                            width: 200,
                            key: 'collectedHubReceiveDate',
                            sorter: (a, b) => a.collectedHubReceiveDate.localeCompare(b.collectedHubReceiveDate),
                            render: ({ ...props }: Batch) => <p className="m-0">{moment(props.collectedHubReceiveDate).format('DD/MM/YYYY')}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái giao hàng" />,
                            width: 200,
                            key: 'status',
                            render: ({ ...props }: Batch) => (
                                <p className="m-0">
                                    {props.status == 'Processed' ? <Tag color="blue">Đã xử lý</Tag> : <Tag color="success">Đã Nhận</Tag>}
                                </p>
                            ),
                        },

                        {
                            title: () => <TableHeaderCell key="" sortKey="" label="" />,
                            width: 200,
                            key: 'detail',
                            render: ({ ...props }: Batch) => {
                                return (
                                    <div>
                                        <Link
                                            href={routes.farmhub.batch.detail(props.id)}
                                            className="py-2 text-center text-white cursor-pointer bg-primary"
                                        >
                                            Chi tiết
                                        </Link>
                                    </div>
                                );
                            },
                        },
                    ]}
                />
            </div>
        </>
    );
};
export default BatchListFarmHub;
