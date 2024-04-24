import { TableBuilder, TableHeaderCell } from '@components/tables';
import { BatchDetail, Order } from '@models/batch';
import { useStoreUser } from '@store/index';
import { Badge, Descriptions, Tag } from 'antd';
import moment from 'moment';
import React from 'react';
interface BatchDetailProps {
    value?: BatchDetail;
}

const BatchDetails: React.FC<BatchDetailProps> = ({ value }) => {
    const user = useStoreUser();
    // const { data } = useQueryGetListOrderInBusinessDayForFarmHub(user.farmHub?.id as string, value?.id as string);

    return (
        <>
            <div className="flex flex-col w-full gap-4">
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={'Thông tin liên quan đến lô hàng'}
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item label="Tên kho hàng" span={2}>
                        {value?.collectedHubName}
                    </Descriptions.Item>
                    <Descriptions.Item label="Trạng thái" span={1}>
                        <Badge
                            status={value?.status === 'Received' ? 'success' : 'error'}
                            text={value?.status === 'Received' ? 'Đã nhận' : 'Đang xử lý'}
                        />
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày đăng bán" span={1}>
                        {moment(value?.businessDayOpen).format('DD/MM/YYYY')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày giao hàng" span={1}>
                        {value?.farmShipDate && moment(value?.farmShipDate).format('DD/MM/YYYY')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày nhận hàng" span={1}>
                        {value?.collectedHubReceiveDate ? moment(value?.collectedHubReceiveDate).format('DD/MM/YYYY') : 'Chưa nhận'}
                    </Descriptions.Item>
                </Descriptions>
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={'Danh sách đơn hàng'}
                >
                    <div className="flex flex-col w-full gap-2">
                        <TableBuilder<Order>
                            rowKey="id"
                            isLoading={false}
                            data={value?.orders || []}
                            columns={[
                                {
                                    title: () => <TableHeaderCell key="customername" sortKey="customername" label="Tên khách hàng" />,
                                    width: 300,
                                    sorter: (a, b) => a.customerName.localeCompare(b.customerName),
                                    key: 'customername',
                                    render: ({ ...props }: Order) => <p className="m-0">{props.customerName ?? 'Khách Hàng'}</p>,
                                },

                                {
                                    title: () => <TableHeaderCell key="address" sortKey="address" label="Địa chỉ" />,
                                    width: 350,
                                    key: 'address',
                                    sorter: (a, b) => a.shipAddress.localeCompare(b.shipAddress),
                                    render: ({ ...props }: Order) => <p className="m-0">{props.shipAddress}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="code" sortKey="code" label="Mã" />,
                                    width: 150,
                                    key: 'createdAt',
                                    sorter: (a, b) => a.code.localeCompare(b.code),
                                    render: ({ ...props }: Order) => <p className="m-0">{props.code}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="totalAmount" sortKey="totalAmount" label="Tổng tiền / VND" />,
                                    width: 200,
                                    key: 'totalAmount',
                                    sorter: (a, b) => a.totalAmount - b.totalAmount,
                                    render: ({ ...props }: Order) => <p className="m-0">{props.totalAmount}</p>,
                                },

                                {
                                    title: () => <TableHeaderCell key="customerStatus" sortKey="customerStatus" label="Trạng thái khách hàng" />,
                                    width: 200,
                                    key: 'customerStatus',
                                    render: ({ ...props }: Order) => (
                                        <p className="m-0">
                                            {props.customerStatus == 'Confirmed' ? (
                                                <Tag color="success">Chấp nhận</Tag>
                                            ) : props.customerStatus == 'CanceledByFarmHub' ? (
                                                <Tag color="red">Đã huỷ</Tag>
                                            ) : (
                                                <Tag color="blue">Đang chờ</Tag>
                                            )}
                                        </p>
                                    ),
                                },
                                {
                                    title: () => <TableHeaderCell key="deliveryStatus" sortKey="deliveryStatus" label="Trạng thái giao hàng" />,
                                    width: 200,
                                    key: 'deliveryStatus',
                                    render: ({ ...props }: Order) => (
                                        <p className="m-0">
                                            {props.deliveryStatus == 'Pending' ? (
                                                <Tag color="blue">Chưa giao</Tag>
                                            ) : props.deliveryStatus == 'CollectedHubNotReceived' ? (
                                                <Tag color="error">Không nhận hàng</Tag>
                                            ) : (
                                                <Tag color="success">Đã giao</Tag>
                                            )}
                                        </p>
                                    ),
                                },
                                {
                                    title: () => <TableHeaderCell key="isPaid" sortKey="isPaid" label="Thanh toán" />,
                                    width: 200,
                                    key: 'isPaid',
                                    render: ({ ...props }: Order) => (
                                        <p className="m-0">
                                            {props.isPaid ? <Tag color="success">Đã thanh toán</Tag> : <Tag color="blue">Chưa thanh toán</Tag>}
                                        </p>
                                    ),
                                },
                                // {
                                //     title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                //     width: 400,
                                //     key: 'detail',
                                //     render: ({ ...props }: Order) => {
                                //         return (
                                //             <div>
                                //                 <Link
                                //                     href={routes.farmhub.menu.detail(props.id)}
                                //                     className="py-2 text-center text-white cursor-pointer bg-primary"
                                //                 >
                                //                     Xem sản phẩm của menu
                                //                 </Link>
                                //             </div>
                                //         );
                                //     },
                                // },
                            ]}
                        />
                    </div>
                </Descriptions>
            </div>
        </>
    );
};

export default BatchDetails;
