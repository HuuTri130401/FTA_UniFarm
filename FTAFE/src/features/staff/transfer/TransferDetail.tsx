import { TableActionCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { OrderAPI } from '@core/api/order.api';
import { OrderDetail } from '@models/batch';
import { OrderTien } from '@models/order';
import { useQuery } from '@tanstack/react-query';
import { Button, Descriptions, Image, Modal } from 'antd';
import moment from 'moment';
import * as React from 'react';
import { CustomerStatus, DeliveryStatus } from 'src/constant/enum';
import { Tag } from 'antd';

interface TransferDetailProps {
    id: string;
}

const TransferDetail: React.FunctionComponent<TransferDetailProps> = ({ id }) => {
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['transfer', id, 'orders'],
        queryFn: async () => {
            const res = await OrderAPI.getByTransferId(id);
            setTotalItem(res.payload.length);
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
    return (
        <div className="flex flex-col w-full gap-10">
            <TableBuilder<OrderTien>
                rowKey="id"
                isLoading={isLoading}
                data={data?.payload}
                columns={[
                    {
                        title: () => <TableHeaderCell key="code" sortKey="code" label="Mã" />,
                        width: 400,
                        key: 'code',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{props.code}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="createdAt" sortKey="createdAt" label="Ngày tạo" />,
                        width: 400,
                        key: 'createdAt',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="expectedReceiveDate" sortKey="expectedReceiveDate" label="Dự kiến ngày giao" />,
                        width: 400,
                        key: 'expectedReceiveDate',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{moment(props.expectedReceiveDate).format('DD/MM/YYYY HH:mm:ss')}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="customerId" sortKey="customerId" label="Mã khách hàng" />,
                        width: 400,
                        key: 'customerId',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{props.customerId}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="shipAddress" sortKey="shipAddress" label="Địa chỉ giao hàng" />,
                        width: 400,
                        key: 'shipAddress',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{props.shipAddress}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="totalAmount" sortKey="totalAmount" label="Tổng số lượng" />,
                        width: 400,
                        key: 'totalAmount',
                        render: ({ ...props }: OrderTien) => <p className="m-0">{props.totalAmount}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="customerStatus" sortKey="customerStatus" label="Trạng thái khách hàng" />,
                        width: 400,
                        key: 'customerStatus',
                        render: ({ ...props }: OrderTien) => (
                            <p className="m-0">
                                {props.customerStatus in CustomerStatus
                                    ? CustomerStatus[props.customerStatus as keyof typeof CustomerStatus]
                                    : props.customerStatus}
                            </p>
                        ),
                    },
                    {
                        title: () => <TableHeaderCell key="deliveryStatus" sortKey="deliveryStatus" label="Trạng thái giao hàng1" />,
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

export default TransferDetail;
