import { TableActionCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { OrderAPI } from '@core/api/order.api';
import { useDebounce } from '@hooks/useDebounce';
import { OrderTien } from '@models/order';
import { useQuery } from '@tanstack/react-query';
import { Button, Descriptions, Image, Input, Modal, Tag } from 'antd';
import moment from 'moment';
import * as React from 'react';
import { CustomerStatus, DeliveryStatus } from 'src/constant/enum';

interface OrderListProps {}
const { Search } = Input;
interface OrderDetail {
    productItemId: string;
    quantity: number;
    unitPrice: number;
    originUnitPrice: number;
    unit: string;
    totalPrice: number;
    title: string;
    productImage: string | null;
}

const OrderList: React.FunctionComponent<OrderListProps> = () => {
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['orders', 'collectedHub'],
        queryFn: async () => {
            const res = await OrderAPI.getAll();
            setTotalItem(res.payload.length);
            return res;
        },
    });
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
    const statusColorMap = {
        AtCollectedHub: 'blue',
        CanceledByCollectedHub: 'red',
        CollectedHubNotReceived: 'orange',
        Pending: 'gold',
        CanceledByFarmHub: 'red',
        OnTheWayToCollectedHub: 'purple',
        OnTheWayToStation: 'cyan',
        PickedUp: 'green',
    };
    const [openModalState, setOpenModalState] = React.useState(false);
    // detail modal
    const [detailModalState, setDetailModalState] = React.useState(false);
    const [orderDetail, setOrderDetail] = React.useState<OrderDetail[]>([]);
    //
    const listOrder: OrderTien[] = data?.payload || [];
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const filterData = listOrder.filter(
        (i) =>
            i.customerResponse.userName.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.shipAddress.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.code.toLowerCase().includes(debouncedValue.toLowerCase())
    );

    return (
        <div className="flex flex-col w-full gap-10">
            <Search
                placeholder="Tìm kiếm khách hàng, địa chỉ, code"
                allowClear
                enterButton="Tìm kiếm tên khách hàng/địa chỉ"
                size="middle"
                onChange={(e) => setSearchText(e.target.value)} // Update search text
                style={{ marginBottom: '1rem', marginTop: '1rem' }}
            />
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
                        sorter: (a, b) => a.code.localeCompare(b.code),
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
                        render: ({ ...props }: OrderTien) => <p className="m-0">{moment(props.expectedReceiveDate).format('DD/MM/YYYY HH:mm:ss')}</p>,
                        sorter: (a, b) => moment(a.expectedReceiveDate).valueOf() - moment(b.expectedReceiveDate).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="customerResponse.userName" label="Tên khách hàng" />,
                        width: 400,
                        key: 'customerResponse.userName',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{props.customerResponse.userName}</p>,
                        sorter: (a, b) => a.customerResponse.userName.localeCompare(b.customerResponse.userName),
                    },
                    {
                        title: () => <TableHeaderCell key="shipAddress" label="Địa chỉ giao hàng" />,
                        width: 400,
                        key: 'shipAddress',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{props.shipAddress}</p>,
                        sorter: (a, b) => a.shipAddress.localeCompare(b.shipAddress),
                    },
                    {
                        title: () => <TableHeaderCell key="totalAmount" label="Giá" />,
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
                        filters: [
                            { text: 'Đã tới kho phân loại', value: 'AtCollectedHub' },
                            { text: 'Đang vận chuyển', value: 'OnDelivery' },
                            { text: 'Đã bị hủy bởi hệ thống', value: 'CanceledByCollectedHub' },
                            { text: 'Đã xác nhận', value: 'Confirmed' },
                            { text: 'Đã bị hủy bởi cửa hàng', value: 'CanceledByFarmHub' },
                            { text: 'Chờ xử lý', value: 'Pending' },
                            { text: 'Đã nhận', value: 'PickedUp' },
                            // Add more filters if needed
                        ],
                        onFilter: (value, record) => record.customerStatus === value,
                    },
                    {
                        title: () => <TableHeaderCell key="deliveryStatus" sortKey="deliveryStatus" label="Trạng thái giao hàng" />,
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
                        filters: [
                            { text: 'Đã tới kho phân loại', value: 'AtCollectedHub' },
                            { text: 'Đã bị hủy bởi kho phân loại', value: 'CanceledByCollectedHub' },
                            { text: 'Kho phân loại không nhận được hàng', value: 'CollectedHubNotReceived' },
                            { text: 'Chờ xử lý', value: 'Pending' },
                            { text: 'Đã bị hủy bởi cửa hàng', value: 'CanceledByFarmHub' },
                            { text: 'Đang trên đường tới kho', value: 'OnTheWayToCollectedHub' },
                            { text: 'Đang vận chuyển đến trạm', value: 'OnTheWayToStation' },
                            { text: 'Đã nhận', value: 'PickedUp' },
                        ],
                        onFilter: (value, record) => record.deliveryStatus === value,
                    },
                    {
                        title: () => <TableHeaderCell key="farmhub-name" sortKey="farmhub-name" label="Nhà cung cấp" />,
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
                                <Button
                                    type="primary"
                                    onClick={() => {
                                        setOrderDetail(props.orderDetailResponse);
                                        setDetailModalState(!detailModalState);
                                    }}
                                >
                                    Xem chi tiết
                                </Button>
                            );
                        },
                    },
                ]}
            />
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
                            title: () => <TableHeaderCell key="unitPrice" label="Đơn Giá" />,
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
                            title: () => <TableHeaderCell key="totalPrice" label="Tổng giá trị" />,
                            width: 400,
                            key: 'totalPrice',
                            render: ({ ...props }: OrderDetail) => <p className="m-0">{props.totalPrice}</p>,
                        },
                    ]}
                />
            </Modal>
        </div>
    );
};

export default OrderList;
