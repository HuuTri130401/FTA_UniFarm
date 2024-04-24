import { EyeOutlined } from '@ant-design/icons';
import { TableActionCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { ADMIN_API } from '@core/api/admin.api';
import { useDebounce } from '@hooks/useDebounce';
import { OrderDetail } from '@models/batch';
import { OrderTien } from '@models/order';
import { useQuery } from '@tanstack/react-query';
import { Descriptions, Image, Input, Modal, Tag } from 'antd';
import moment from 'moment';
import * as React from 'react';
import { CustomerStatus, DeliveryStatus } from 'src/constant/enum';

interface OrderListProps {}

const { Search } = Input;

const OrderList: React.FunctionComponent<OrderListProps> = () => {
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['orders'],
        queryFn: async () => {
            const res = await ADMIN_API.getOrders();
            setTotalItem(res?.payload.length);
            return res;
        },
    });

    const statusColorMap = {
        AtCollectedHub: 'blue',
        CanceledByCollectedHub: 'red',
        CollectedHubNotReceived: 'orange',
        Pending: 'gold',
        CanceledByFarmHub: 'red',
        OnTheWayToCollectedHub: 'purple',
        OnTheWayToStation: 'cyan',
        PickedUp: 'green',
        Active: '#87d068',
    };
    const [farmHub, setFarmHub] = React.useState({
        id: '',
        name: '',
        code: '',
        description: '',
        image: '',
        address: '',
        createdAt: '',
        updatedAt: '',
        status: '',
    });
    const [openModalState, setOpenModalState] = React.useState(false);
    // detail modal
    const [detailModalState, setDetailModalState] = React.useState(false);
    const [orderDetail, setOrderDetail] = React.useState<OrderDetail[]>([]);
    // search
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const listOrder: OrderTien[] = data?.payload || [];
    const filterData = listOrder.filter((i) => i.code.includes(debouncedValue.trim()));
    return (
        <div className="flex flex-col w-full gap-10">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Lịch sử đặt hàng của khách`}
                className="p-4 bg-white rounded-lg"
                extra={
                    <div className="flex items-center w-full gap-5">
                        <Search
                            placeholder="Nhập mã đơn hàng..."
                            allowClear
                            enterButton="Tìm kiếm theo code"
                            size="middle"
                            onChange={(e) => setSearchText(e.target.value)} // Update search text
                            style={{ marginBottom: '1rem', marginTop: '1rem', width: '500px' }}
                        />
                    </div>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<OrderTien>
                        rowKey="id"
                        isLoading={isLoading}
                        data={filterData}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="code" label="Mã" />,
                                width: 400,
                                key: 'code',
                                render: ({ ...props }: OrderTien) => <p className="m-0">{props.code}</p>,
                            },
                            {
                                title: () => <TableHeaderCell key="createdAt" label="Ngày tạo" />,
                                width: 400,
                                key: 'createdAt',
                                render: ({ ...props }: OrderTien) => <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}</p>,
                                sorter: (a, b) => moment(a.createdAt).valueOf() - moment(b.createdAt).valueOf(),
                            },
                            {
                                title: () => <TableHeaderCell key="expectedReceiveDate" label="Dự kiến ngày giao" />,
                                width: 400,
                                key: 'expectedReceiveDate',
                                render: ({ ...props }: OrderTien) => (
                                    <p className="m-0">{moment(props.expectedReceiveDate).format('DD/MM/YYYY HH:mm:ss')}</p>
                                ),
                                sorter: (a, b) => moment(a.expectedReceiveDate).valueOf() - moment(b.expectedReceiveDate).valueOf(),
                            },
                            {
                                title: () => <TableHeaderCell key="customerId" label="Tên khách hàng" />,
                                width: 400,
                                key: 'customerId',
                                render: ({ ...props }: OrderTien) => <p className="m-0">{props.customerResponse.userName ?? 'Không có tên'}</p>,
                                sorter: (a, b) => a.customerResponse.userName.localeCompare(b.customerResponse.userName),
                            },
                            {
                                title: () => <TableHeaderCell key="shipAddress" label="Địa chỉ giao hàng" />,
                                width: 400,
                                key: 'shipAddress',
                                render: ({ ...props }: OrderTien) => <p className="m-0">{props.shipAddress ?? 'Không có'}</p>,
                                sorter: (a, b) => {
                                    if (!a?.shipAddress) return 1;
                                    if (!b?.shipAddress) return -1;

                                    return a?.shipAddress.localeCompare(b?.shipAddress);
                                },
                            },
                            {
                                title: () => <TableHeaderCell key="totalAmount" label="Tổng tiền" />,
                                width: 400,
                                key: 'totalAmount',
                                render: ({ ...props }: OrderTien) => <p className="m-0">{props.totalAmount}</p>,
                                sorter: (a, b) => a.totalAmount - b.totalAmount,
                            },
                            {
                                title: () => <TableHeaderCell key="customerStatus" label="Trạng thái khách hàng" />,
                                width: 400,
                                key: 'customerStatus',
                                render: ({ ...props }: OrderTien) => (
                                    <p className="m-0">
                                        {props.customerStatus in CustomerStatus
                                            ? CustomerStatus[props.customerStatus as keyof typeof CustomerStatus]
                                            : props.customerStatus}
                                    </p>
                                ),
                                filters: Object.keys(CustomerStatus).map((key) => {
                                    return {
                                        text: CustomerStatus[key as keyof typeof CustomerStatus],
                                        value: key as CustomerStatus,
                                    };
                                }),
                                onFilter: (value, record) => record.customerStatus === value,
                            },
                            {
                                title: () => <TableHeaderCell key="deliveryStatus" label="Trạng thái giao hàng" />,
                                width: 400,
                                key: 'deliveryStatus',
                                render: ({ ...props }: OrderTien) => (
                                    <p className="m-0">
                                        {props.deliveryStatus in DeliveryStatus ? (
                                            <Tag color={statusColorMap[props.deliveryStatus as keyof typeof statusColorMap]}>
                                                {DeliveryStatus[props.deliveryStatus as keyof typeof DeliveryStatus]}
                                            </Tag>
                                        ) : (
                                            props.deliveryStatus
                                        )}
                                    </p>
                                ),
                                filters: Object.keys(DeliveryStatus).map((key) => {
                                    return {
                                        text: DeliveryStatus[key as keyof typeof DeliveryStatus],
                                        value: key as DeliveryStatus,
                                    };
                                }),
                                onFilter: (value, record) => record.deliveryStatus === value,
                            },
                            {
                                title: () => <TableHeaderCell key="farmhub-name" label="Nhà cung cấp" />,
                                width: 400,
                                key: 'farmhub-name',
                                render: ({ ...props }: OrderTien) => (
                                    <TableActionCell
                                        label={props.farmHubResponse.name}
                                        actions={[
                                            {
                                                label: 'Xem chi tiết',
                                                onClick: () => {
                                                    setOpenModalState(!openModalState);
                                                    setFarmHub(props.farmHubResponse);
                                                },
                                            },
                                        ]}
                                    />
                                ),
                            },
                            {
                                title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                width: 400,
                                key: 'detail',
                                render: ({ ...props }: OrderTien) => {
                                    return (
                                        <EyeOutlined
                                            className="w-10 h-10 mt-4 text-blue-500 cursor-pointer"
                                            style={{ fontSize: '1.5rem' }}
                                            onClick={() => {
                                                setOrderDetail(props.orderDetailResponse);
                                                setDetailModalState(!detailModalState);
                                            }}
                                        />
                                    );
                                },
                            },
                        ]}
                    />
                </div>
            </Descriptions>
            <Modal
                className="w-full"
                onCancel={() => {
                    setOpenModalState(false);
                }}
                open={openModalState}
                footer={null}
                width={1000}
            >
                <Descriptions
                    title="Thông tin nông trại"
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item label="Ảnh" span={1}>
                        <Image height={80} width={80} className="rounded overflow-hidden" alt={farmHub?.name} src={farmHub?.image} />
                    </Descriptions.Item>
                    <Descriptions.Item label="Tên" span={2}>
                        {farmHub?.name}
                    </Descriptions.Item>
                    <Descriptions.Item label="Mã" span={1}>
                        {farmHub?.code}
                    </Descriptions.Item>
                    <Descriptions.Item label="Mô tả" span={2}>
                        {farmHub?.description}
                    </Descriptions.Item>
                </Descriptions>
            </Modal>
            <Modal
                className="w-full"
                onCancel={() => {
                    setDetailModalState(false);
                }}
                open={detailModalState}
                footer={null}
                width={1000}
            >
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={`Danh sách đơn hàng`}
                    className="p-4 bg-white rounded-lg"
                >
                    <div className="flex flex-col w-full gap-2">
                        <TableBuilder<OrderDetail>
                            rowKey="id"
                            isLoading={isLoading}
                            data={orderDetail}
                            columns={[
                                {
                                    title: () => <TableHeaderCell key="title" label="Tên sản phẩm" />,
                                    width: 400,
                                    key: 'title',
                                    render: ({ ...props }: OrderDetail) => <p className="m-0">{props.title}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="unit" label="Đơn vị" />,
                                    width: 400,
                                    key: 'unit',
                                    render: ({ ...props }: OrderDetail) => <p className="m-0">{props.unit}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="unitPrice" label="Đơn Giá(VNĐ)" />,
                                    width: 400,
                                    key: 'unitPrice',
                                    render: ({ ...props }: OrderDetail) => <p className="m-0">{props.unitPrice}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="quantity" label="Số lượng" />,
                                    width: 400,
                                    key: 'quantity',
                                    render: ({ ...props }: OrderDetail) => <p className="m-0">{props.quantity}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="totalPrice" label="Tổng giá trị(VNĐ)" />,
                                    width: 400,
                                    key: 'totalPrice',
                                    render: ({ ...props }: OrderDetail) => <p className="m-0">{props.totalPrice}</p>,
                                },
                            ]}
                        />
                    </div>
                </Descriptions>
            </Modal>
        </div>
    );
};

export default OrderList;
