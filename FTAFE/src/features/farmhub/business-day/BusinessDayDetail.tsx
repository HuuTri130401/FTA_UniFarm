import { EyeInvisibleOutlined, EyeOutlined, WarningOutlined } from '@ant-design/icons';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { routes } from '@core/routes';
import {
    useMutationConfirmOrder,
    useQueryGetListBatchByBusinessDayFarmHub,
    useQueryGetListOrderInBusinessDayForFarmHub,
} from '@hooks/api/batch.hook';
import { Batch, OrderDetail } from '@models/batch';
import { BusinessDay } from '@models/business-day';
import { FarmHubMenu } from '@models/farmhub-menu';
import { useStoreUser } from '@store/index';
import { Badge, Descriptions, Modal, Spin, Tabs, Tag } from 'antd';
import clsx from 'clsx';
import { PlusIcon } from 'lucide-react';
import moment from 'moment';
import Link from 'next/link';
import React from 'react';
import CreateBatchModal from '../batch/components/CreateBatchModal';
import CreateMenuInBusinessDayModal from './components/CreateMenuInBusinessDayModal';
import DetailOrderModal from './components/DetailOrderModal';
import ProductItemSelling from './components/ProductItemSellingChart';
interface BusinessDayDetailProps {
    value?: BusinessDay;
}

const BusinessDayDetail: React.FC<BusinessDayDetailProps> = ({ value }) => {
    const menu: FarmHubMenu[] = value?.menus || [];
    const [openCreateModalState, setOpenCreateModalState] = React.useState<boolean>(false);
    const [createModalState, setCreateModalState] = React.useState(false);
    const [openDetailOrderModal, setOpenDetailOrderModal] = React.useState<boolean>(false);
    const user = useStoreUser();
    const { data } = useQueryGetListOrderInBusinessDayForFarmHub(user.farmHub?.id as string, value?.id as string);
    const { data: dataListOrder, isLoading: isLoadingListOrder } = useQueryGetListBatchByBusinessDayFarmHub(
        user.farmHub?.id as string,
        value?.id as string
    );
    const [orderDetail, setOrderDetail] = React.useState<OrderDetail[] | null>(null);
    const [customerNameDetail, setCustomerNameDetail] = React.useState<string>('');
    const [orderDetailStatus, setOrderDetailStatus] = React.useState<string>('' as string);
    const [batchId, setBatchId] = React.useState<string>('');
    const { mutateConfirmOrder, isLoading } = useMutationConfirmOrder();

    const onConfirmOrder = (id: string) => {
        mutateConfirmOrder({
            orderId: id,
            confirmStatus: 'Confirmed',
        });
    };

    const onCancelOrder = (id: string) => {
        mutateConfirmOrder({
            orderId: id,
            confirmStatus: 'Canceled',
        });
    };

    const handleCancelOrder = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc chắn muốn hủy đơn hàng này?',
            content: 'Hành động này sẽ không thể hoàn tác!',
            okText: 'Hủy đơn hàng',
            okType: 'danger',
            cancelText: 'Trở lại',
            icon: <WarningOutlined style={{ color: 'red' }} />,
            onOk: () => onCancelOrder(id),
        });
    };

    return (
        <>
            <div className="flex flex-col w-full gap-4">
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={'Thông tin liên quan đến ngày đăng bán'}
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item label="Tên sự kiện" span={2}>
                        {value?.name}
                    </Descriptions.Item>
                    <Descriptions.Item label="Trạng thái" span={1}>
                        <Badge
                            status={value?.status === 'Active' ? 'success' : 'error'}
                            color={
                                value?.status === 'Active'
                                    ? 'blue'
                                    : value?.status === 'Completed'
                                    ? 'green'
                                    : value?.status === 'PaymentConfirm'
                                    ? 'magenta'
                                    : 'red' // Mặc định cho các trạng thái khác
                            }
                            text={
                                value?.status === 'Active'
                                    ? 'Hoạt động'
                                    : value?.status === 'Completed'
                                    ? 'Hoàn thành'
                                    : value?.status === 'PaymentConfirm'
                                    ? 'Chờ thanh toán'
                                    : 'Không hoạt động'
                            }
                        />
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày đăng ký" span={1}>
                        {value?.regiterDay && moment(value?.regiterDay).format('DD/MM/YYYY')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày kết thúc đăng ký" span={1}>
                        {value?.endOfRegister && moment(value?.endOfRegister).format('DD/MM/YYYY')}
                    </Descriptions.Item>

                    <Descriptions.Item label="Ngày tạo" span={1}>
                        {moment(value?.createdAt).format('DD/MM/YYYY')}
                    </Descriptions.Item>
                </Descriptions>
                <Tabs
                    defaultActiveKey="1"
                    centered
                    className="bg-white rounded-lg"
                    items={[
                        {
                            label: <p className="p-0 m-0 text-black dark:text-white">Thống kê doanh thu</p>,
                            key: '1',
                            children: (
                                <div className="flex items-center justify-center">
                                    {value?.id ? <ProductItemSelling id={value?.id} /> : <Spin size="large" />}
                                </div>
                            ),
                        },
                        {
                            label: <p className="p-0 m-0 text-black dark:text-white">Danh sách lô hàng</p>,
                            key: '2',
                            children: (
                                <>
                                    <Descriptions
                                        labelStyle={{
                                            fontWeight: 'bold',
                                        }}
                                        bordered
                                        className="p-4 bg-white rounded-lg"
                                        title={
                                            <div className="flex items-center justify-between">
                                                <p className="m-0">Danh sách lô hàng</p>
                                                <div className="flex flex-col items-end w-full gap-2 ">
                                                    <button
                                                        onClick={() => {
                                                            setCreateModalState(!createModalState);
                                                        }}
                                                        className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                                                    >
                                                        <PlusIcon className="w-5 h-5 text-white" />
                                                        <span className="text-base font-medium">Tạo lô hàng</span>
                                                    </button>
                                                </div>
                                            </div>
                                        }
                                    >
                                        <div className="flex flex-col w-full gap-2">
                                            <TableBuilder<Batch>
                                                rowKey="id"
                                                isLoading={isLoadingListOrder}
                                                data={dataListOrder}
                                                columns={[
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell
                                                                key="collectedHubName"
                                                                sortKey="collectedHubName"
                                                                label="Tên kho nhập hàng"
                                                            />
                                                        ),
                                                        width: 300,
                                                        sorter: (a, b) => a.collectedHubName.localeCompare(b.collectedHubName),
                                                        key: 'collectedHubName',
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.collectedHubName}</p>,
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell
                                                                key="collectedHubAddress"
                                                                sortKey="collectedHubAddress"
                                                                label="Địa chỉ kho nhập hàng"
                                                            />
                                                        ),
                                                        width: 400,
                                                        sorter: (a, b) => a.collectedHubAddress.localeCompare(b.collectedHubAddress),
                                                        key: 'collectedHubAddress',
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.collectedHubAddress}</p>,
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell key="businessDayName" sortKey="businessDayName" label="Ngày đăng bán" />
                                                        ),
                                                        width: 300,
                                                        sorter: (a, b) => a.businessDayName.localeCompare(b.businessDayName),
                                                        key: 'businessDayName',
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.businessDayName}</p>,
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell key="businessDayOpen" sortKey="businessDayOpen" label="Ngày mở bán" />
                                                        ),
                                                        width: 200,
                                                        key: 'businessDayOpen',
                                                        sorter: (a, b) => a.businessDayOpen.localeCompare(b.businessDayOpen),
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">{moment(props.businessDayOpen).format('DD/MM/YYYY')}</p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell key="farmShipDate" sortKey="farmShipDate" label="Ngày giao hàng" />
                                                        ),
                                                        width: 200,
                                                        key: 'farmShipDate',
                                                        sorter: (a, b) => a.farmShipDate.localeCompare(b.farmShipDate),
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">{moment(props.farmShipDate).format('DD/MM/YYYY')}</p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell
                                                                key="collectedHubReceiveDate"
                                                                sortKey="collectedHubReceiveDate"
                                                                label="Ngày nhận hàng"
                                                            />
                                                        ),
                                                        width: 200,
                                                        key: 'collectedHubReceiveDate',
                                                        sorter: (a, b) => a.collectedHubReceiveDate.localeCompare(b.collectedHubReceiveDate),
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">{moment(props.collectedHubReceiveDate).format('DD/MM/YYYY')}</p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái giao hàng" />,
                                                        width: 200,
                                                        key: 'status',
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">
                                                                {props.status === 'Processed' ? (
                                                                    <Tag color="blue">Đã xử lý</Tag>
                                                                ) : props.status === 'Pending' ? (
                                                                    <Tag color="yellow">Đang chờ</Tag>
                                                                ) : (
                                                                    <Tag color="success">Đã Nhận</Tag>
                                                                )}
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
                                                                        href={routes.farmhub.businessDay.batchDetail(props.id)}
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
                                    </Descriptions>
                                </>
                            ),
                        },
                        {
                            label: <p className="p-0 m-0 text-black dark:text-white">Các danh sách bán</p>,
                            key: '3',
                            children: (
                                <>
                                    <Descriptions
                                        labelStyle={{
                                            fontWeight: 'bold',
                                        }}
                                        bordered
                                        className="p-4 bg-white rounded-lg"
                                        title={
                                            <div className="flex items-center justify-between">
                                                <p className="m-0">Danh sách bán</p>
                                                <div className="flex flex-col items-end w-full gap-2 ">
                                                    <button
                                                        onClick={() => {
                                                            setOpenCreateModalState(!openCreateModalState);
                                                        }}
                                                        className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                                                    >
                                                        <PlusIcon className="w-5 h-5 text-white" />
                                                        <span className="text-base font-medium">Thêm danh sách sản phẩm vào ngày bán</span>
                                                    </button>
                                                </div>
                                            </div>
                                        }
                                    >
                                        <div className="flex flex-col w-full gap-2">
                                            <TableBuilder<FarmHubMenu>
                                                rowKey="id"
                                                isLoading={false}
                                                data={menu}
                                                columns={[
                                                    {
                                                        title: () => <TableHeaderCell key="name" sortKey="name" label="Menu" />,
                                                        width: 400,
                                                        key: 'name',
                                                        render: ({ ...props }: FarmHubMenu) => <p className="m-0">{props.name}</p>,
                                                    },
                                                    // {
                                                    //     title: () => <TableHeaderCell key="productOrigin" sortKey="productOrigin" label="Nơi sản xuất" />,
                                                    //     width: 400,
                                                    //     key: 'name',
                                                    //     render: ({ ...props }: FarmHubMenu) => <p className="m-0">{props.name}</p>,
                                                    // },
                                                    {
                                                        title: () => <TableHeaderCell key="tag" sortKey="tag" label="Mã tag" />,
                                                        width: 400,
                                                        key: 'tag',
                                                        render: ({ ...props }: FarmHubMenu) => <p className="m-0">{props.tag}</p>,
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="createdAt" sortKey="createdAt" label="Ngày tạo" />,
                                                        width: 400,
                                                        key: 'createdAt',
                                                        render: ({ ...props }: FarmHubMenu) => (
                                                            <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY')}</p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
                                                        width: 400,
                                                        key: 'status',
                                                        render: ({ ...props }: FarmHubMenu) => {
                                                            return (
                                                                <Tag
                                                                    className={clsx(`text-sm whitespace-normal`)}
                                                                    color={
                                                                        typeof props.status === 'string' && props.status === 'Active'
                                                                            ? 'geekblue'
                                                                            : 'volcano'
                                                                    }
                                                                >
                                                                    {props.status === 'Active'
                                                                        ? 'Hoạt động'
                                                                        : props.status === 'PENDING'
                                                                        ? 'Đang chờ'
                                                                        : 'Không hoạt động'}
                                                                </Tag>
                                                            );
                                                        },
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                                        width: 400,
                                                        key: 'detail',
                                                        render: ({ ...props }: FarmHubMenu) => {
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
                                    </Descriptions>
                                </>
                            ),
                        },
                        {
                            label: <p className="p-0 m-0 text-black dark:text-white">Danh sách đơn hàng</p>,
                            key: '4',
                            children: (
                                <>
                                    <Descriptions
                                        labelStyle={{
                                            fontWeight: 'bold',
                                        }}
                                        bordered
                                        className="p-4 bg-white rounded-lg"
                                        title={'Danh sách đơn hàng'}
                                    >
                                        <div className="flex flex-col w-full gap-2">
                                            <TableBuilder<Batch>
                                                rowKey="id"
                                                isLoading={false}
                                                data={data}
                                                columns={[
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell key="customername" sortKey="customername" label="Tên khách hàng" />
                                                        ),
                                                        width: 300,
                                                        sorter: (a, b) => a.customerName.localeCompare(b.customerName),
                                                        key: 'customername',
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.customerName}</p>,
                                                    },

                                                    {
                                                        title: () => <TableHeaderCell key="address" sortKey="address" label="Địa chỉ" />,
                                                        width: 350,
                                                        key: 'address',
                                                        sorter: (a, b) => a.shipAddress.localeCompare(b.shipAddress),
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.shipAddress}</p>,
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="code" sortKey="code" label="Mã" />,
                                                        width: 150,
                                                        key: 'createdAt',
                                                        sorter: (a, b) => a.code.localeCompare(b.code),
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.code}</p>,
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell key="totalAmount" sortKey="totalAmount" label="Tổng tiền / VND" />
                                                        ),
                                                        width: 200,
                                                        key: 'totalAmount',
                                                        sorter: (a, b) => a.totalAmount - b.totalAmount,
                                                        render: ({ ...props }: Batch) => <p className="m-0">{props.totalAmount}</p>,
                                                    },

                                                    {
                                                        title: () => (
                                                            <TableHeaderCell
                                                                key="customerStatus"
                                                                sortKey="customerStatus"
                                                                label="Trạng thái khách hàng"
                                                            />
                                                        ),
                                                        width: 200,
                                                        key: 'customerStatus',
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">
                                                                {props.customerStatus == 'Confirmed' ? (
                                                                    <Tag color="success">Chấp nhận</Tag>
                                                                ) : props.customerStatus == 'CanceledByFarmHub' ? (
                                                                    <Tag color="red">Đã huỷ bởi FarmHub</Tag>
                                                                ) : props.customerStatus == 'AtCollectedHub' ? (
                                                                    <Tag color="yellow">Đã tới kho phân loại</Tag>
                                                                ) : props.customerStatus == 'CanceledByCollectedHub' ? (
                                                                    <Tag color="red">Đã huỷ bởi kho phân loại</Tag>
                                                                ) : props.customerStatus == 'PickedUp' ? (
                                                                    <Tag color="success">Đã nhận hàng</Tag>
                                                                ) : props.customerStatus == 'OnDelivery' ? (
                                                                    <Tag color="blue">Đang giao</Tag>
                                                                ) : (
                                                                    <Tag color="blue">Đang chờ</Tag>
                                                                )}
                                                            </p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => (
                                                            <TableHeaderCell
                                                                key="deliveryStatus"
                                                                sortKey="deliveryStatus"
                                                                label="Trạng thái giao hàng"
                                                            />
                                                        ),
                                                        width: 200,
                                                        key: 'deliveryStatus',
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">
                                                                {props.deliveryStatus == 'Confirmed' ? (
                                                                    <Tag color="success">Đã giao</Tag>
                                                                ) : props.deliveryStatus == 'CollectedHubNotReceived' ? (
                                                                    <Tag color="error">Không nhận hàng</Tag>
                                                                ) : props.deliveryStatus == 'OnTheWayToStation' ? (
                                                                    <Tag color="yellow">Đang trên đường đến trạm</Tag>
                                                                ) : props.deliveryStatus === 'CanceledByFarmHub' ? (
                                                                    <Tag color="red">Đã bị hủy bởi cửa hàng</Tag>
                                                                ) : props.deliveryStatus == 'AtCollectedHub' ? (
                                                                    <Tag color="yellow">Đã tới kho phân loại</Tag>
                                                                ) : props.deliveryStatus == 'PickedUp' ? (
                                                                    <Tag color="success">Đã nhận hàng</Tag>
                                                                ) : props.deliveryStatus == 'CanceledByCollectedHub' ? (
                                                                    <Tag color="error">Đã hủy bởi kho phân loại</Tag>
                                                                ) : props.deliveryStatus == 'OnTheWayToCollectedHub' ? (
                                                                    <Tag color="yellow">Đang trên đường đến kho phân loại</Tag>
                                                                ) : (
                                                                    <Tag color="magenta">Chưa giao</Tag>
                                                                )}
                                                            </p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="isPaid" sortKey="isPaid" label="Thanh toán" />,
                                                        width: 200,
                                                        key: 'isPaid',
                                                        render: ({ ...props }: Batch) => (
                                                            <p className="m-0">
                                                                {props.isPaid ? (
                                                                    <Tag color="success">Đã thanh toán</Tag>
                                                                ) : (
                                                                    <Tag color="blue">Chưa thanh toán</Tag>
                                                                )}
                                                            </p>
                                                        ),
                                                    },
                                                    {
                                                        title: () => <TableHeaderCell key="action" sortKey="action" label="Hành động" />,
                                                        width: 400,
                                                        key: 'action',
                                                        render: ({ ...props }: Batch) => {
                                                            return (
                                                                <>
                                                                    {props.orderDetails.length > 0 ? (
                                                                        <EyeOutlined
                                                                            className="w-10 h-10 text-blue-500 cursor-pointer"
                                                                            style={{ fontSize: '1.5rem' }}
                                                                            onClick={() => {
                                                                                setOpenDetailOrderModal(true);
                                                                                setOrderDetail(props.orderDetails);
                                                                                setCustomerNameDetail(props.customerName);
                                                                                setBatchId(props.id);
                                                                                setOrderDetailStatus(props.customerStatus);
                                                                            }}
                                                                        />
                                                                    ) : (
                                                                        <EyeInvisibleOutlined
                                                                            className="w-10 h-10 text-gray-500 cursor-not-allowed"
                                                                            style={{ fontSize: '1.5rem' }}
                                                                        />
                                                                    )}
                                                                </>
                                                            );
                                                        },
                                                    },
                                                ]}
                                            />
                                        </div>
                                    </Descriptions>
                                </>
                            ),
                        },
                    ]}
                />
            </div>
            <CreateMenuInBusinessDayModal
                open={openCreateModalState}
                closeMenuModal={() => setOpenCreateModalState(false)}
                businessDayId={value?.id as string}
            />

            <CreateBatchModal
                open={createModalState}
                closeBatchModal={() => setCreateModalState(false)}
                businessDayId={value?.id as string}
                busisnessDayName={value?.name as string}
            />

            {orderDetail && (
                <DetailOrderModal
                    open={openDetailOrderModal}
                    closeModal={() => {
                        setOpenDetailOrderModal(false);
                        setOrderDetail(null);
                    }}
                    orderDetail={orderDetail}
                    customerName={customerNameDetail}
                    status={orderDetailStatus}
                    confirmButton={() => onConfirmOrder(batchId)}
                    cancelButton={() => handleCancelOrder(batchId)}
                />
            )}
        </>
    );
};

export default BusinessDayDetail;
