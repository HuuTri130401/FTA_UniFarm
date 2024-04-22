import { TableActionCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { OrderAPI } from '@core/api/order.api';
import { OrderTri } from '@models/order';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, message } from 'antd';
import moment from 'moment';
import * as React from 'react';
import { CustomerStatus, DeliveryStatus } from 'src/constant/enum';

interface OrderListProps {
    batchId: string;
}
enum Status {
    RECEIVED = 'Received',
    NOT_RECEIVED = 'NotReceived',
    CANCEL = 'Canceled',
}
interface UpdateOrder {
    orderId: string;
    status: Status;
}

const OrderList: React.FunctionComponent<OrderListProps> = ({ batchId }) => {
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['orders', `batch-${batchId}`],
        queryFn: async () => {
            const res = await OrderAPI.getByBatchId(batchId);
            setTotalItem(res.length);
            return res;
        },
    });
    const updateOrderMutation = useMutation(async (st: UpdateOrder) => await OrderAPI.updateProcessOrder(st.orderId, st.status));

    const orderList = data && data.orders;

    const query = useQueryClient();
    const handleUpdate = (data: UpdateOrder) => {
        updateOrderMutation.mutateAsync(data, {
            onSuccess: () => {
                message.success('Cập nhật trạng thái thành công');
                query.invalidateQueries(['orders', `batch-${batchId}`]);
            },
            onError: () => {
                message.error('Cập nhật trạng thái thất bại');
            },
        });
    };

    return (
        <div className="flex flex-col w-full gap-10">
            <TableBuilder<OrderTri>
                rowKey="id"
                isLoading={isLoading}
                data={orderList}
                columns={[
                    {
                        title: () => <TableHeaderCell key="code" sortKey="code" label="Mã" />,
                        width: 400,
                        key: 'code',
                        render: ({ ...props }: OrderTri) => <p className="m-0">{props.code}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="customerName" sortKey="customerName" label="Tên Khách hàng" />,
                        width: 400,
                        key: 'customerName',
                        render: ({ ...props }: OrderTri) => <p className="m-0">{props.customerName}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="shipAddress" sortKey="shipAddress" label="Địa chỉ giao hàng" />,
                        width: 400,
                        key: 'shipAddress',
                        render: ({ ...props }: OrderTri) => <p className="m-0">{props.shipAddress}</p>,
                    },

                    {
                        title: () => <TableHeaderCell key="totalAmount" sortKey="totalAmount" label="Số lượng" />,
                        width: 400,
                        key: 'totalAmount',
                        render: ({ ...props }: OrderTri) => <p className="m-0">{props.totalAmount}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="isPaid" sortKey="isPaid" label="Trạng thái" />,
                        width: 400,
                        key: 'isPaid',
                        render: ({ ...props }: OrderTri) => <p className="m-0">{props.isPaid ? 'đã thanh toán' : 'chưa thanh toán'}</p>,
                    },
                    {
                        title: () => <TableHeaderCell key="customerStatus" sortKey="customerStatus" label="Trạng thái khách hàng" />,
                        width: 400,
                        key: 'customerStatus',
                        render: ({ ...props }: OrderTri) => (
                            <p className="m-0">
                                {props.customerStatus in CustomerStatus
                                    ? CustomerStatus[props.customerStatus as keyof typeof CustomerStatus]
                                    : props.customerStatus}
                            </p>
                        ),
                    },
                    {
                        title: () => <TableHeaderCell key="deliveryStatus" sortKey="deliveryStatus" label="Trạng thái giao hàng" />,
                        width: 400,
                        key: 'deliveryStatus',
                        render: ({ ...props }: OrderTri) => (
                            <p className="m-0">
                                {props.deliveryStatus in DeliveryStatus
                                    ? DeliveryStatus[props.deliveryStatus as keyof typeof DeliveryStatus]
                                    : props.deliveryStatus}
                            </p>
                        ),
                    },

                    {
                        title: () => <TableHeaderCell key="createdAt" sortKey="createdAt" label="Ngày tạo" />,
                        width: 400,
                        key: 'createdAt',
                        render: ({ ...props }: OrderTri) => <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}</p>,
                    },

                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'action',
                        render: ({ ...props }: OrderTri) => {
                            return (
                                <TableActionCell
                                    label="Cập nhật trạng thái"
                                    actions={[
                                        {
                                            label: (
                                                <Button type="primary" color="#4096ff" className="w-full">
                                                    Đã nhận hàng
                                                </Button>
                                            ),

                                            onClick: () =>
                                                handleUpdate({
                                                    status: Status.RECEIVED,
                                                    orderId: props.id,
                                                }),
                                        },
                                        {
                                            label: (
                                                <Button
                                                    type="primary"
                                                    style={{
                                                        background: 'blue',
                                                    }}
                                                    className="w-full"
                                                >
                                                    Chưa nhận hàng
                                                </Button>
                                            ),
                                            onClick: () =>
                                                handleUpdate({
                                                    status: Status.NOT_RECEIVED,
                                                    orderId: props.id,
                                                }),
                                        },
                                        {
                                            label: (
                                                <Button type="primary" danger className="w-full">
                                                    Hủy đơn
                                                </Button>
                                            ),
                                            onClick: () =>
                                                handleUpdate({
                                                    status: Status.CANCEL,
                                                    orderId: props.id,
                                                }),
                                        },
                                    ]}
                                />
                            );
                        },
                    },
                ]}
            />
        </div>
    );
};

export default OrderList;
