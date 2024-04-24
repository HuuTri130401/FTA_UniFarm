import { EyeOutlined } from '@ant-design/icons';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { PaymentAPI } from '@core/api/payment.api';
import BankModal, { BankInfo } from '@features/farmhub/transaction/BankModal';
import { FilterPayment, Payment } from '@models/payment';
import { useQuery } from '@tanstack/react-query';
import { DatePicker, Descriptions, Tag } from 'antd';
import clsx from 'clsx';
import moment, { Moment } from 'moment';
import React, { useEffect, useState } from 'react';

interface PaymentListProps {
    filter: Partial<FilterPayment>;
}

const PaymentList: React.FC<PaymentListProps> = ({ filter }) => {
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['payments'],
        queryFn: async () => await PaymentAPI.getAll(filter),
    });
    const paymentList = data?.payload || [];
    useEffect(() => {
        setTotalItem(paymentList.length);
    }, [paymentList]);
    const [openDetailBankModal, setOpenDetailBankModal] = useState(false);
    const [bankInfo, setBankInfo] = useState<BankInfo>({} as BankInfo);
    const [selectedDate, setSelectedDate] = useState<Moment | null>(null);

    return (
        <div className="flex flex-col w-full gap-2">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Danh sách rút tiền`}
                className="p-4 bg-white rounded-lg"
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Payment>
                        rowKey="id"
                        isLoading={isLoading}
                        data={paymentList}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="userName" label="Tên" />,
                                width: 400,
                                key: 'userName',
                                render: ({ ...props }: Payment) => <p className="m-0">{props.userName}</p>,
                                sorter: (a, b) => a.userName.localeCompare(b.userName),
                            },
                            {
                                title: () => <TableHeaderCell key="balance" label="Số dư(VNĐ)" />,
                                width: 400,
                                key: 'balance',
                                render: ({ ...props }: Payment) => <p className="m-0">{props.balance.toLocaleString('en')}</p>,
                                sorter: (a, b) => a.balance - b.balance,
                            },
                            {
                                title: () => <TableHeaderCell key="transferAmount" label="Tổng giao dịch(VNĐ)" />,
                                width: 400,
                                key: 'transferAmount',
                                render: ({ ...props }: Payment) => <p className="m-0">{props.transferAmount.toLocaleString('en')}</p>,
                                sorter: (a, b) => a.transferAmount - b.transferAmount,
                            },
                            {
                                title: () => <TableHeaderCell key="type" label="Loại giao dịch" />,
                                width: 400,
                                key: 'type',
                                render: ({ ...props }: Payment) => {
                                    return <p className="m-0">{props.type === 'WITHDRAW' ? 'Rút tiền' : 'Nạp tiền'}</p>;
                                },
                                filters: [
                                    { text: 'WITHDRAW', value: 'WITHDRAW' },
                                    { text: 'DEPOSIT', value: 'DEPOSIT' },
                                ],
                                onFilter: (value, record) => record.type === value,
                            },
                            {
                                title: () => <TableHeaderCell key="from" label="Giao dịch từ" />,
                                width: 400,
                                key: 'from',
                                render: ({ ...props }: Payment) => {
                                    return <p className="m-0">{props.from === 'WALLET' ? 'Ví' : 'Ngân hàng'}</p>;
                                },
                            },
                            {
                                title: () => <TableHeaderCell key="to" label="Giao dịch đến" />,
                                width: 400,
                                key: 'to',
                                render: ({ ...props }: Payment) => {
                                    return <p className="m-0">{props.to === 'WALLET' ? 'Ví' : 'Ngân hàng'}</p>;
                                },
                            },
                            {
                                title: () => (
                                    <div>
                                        <TableHeaderCell key="paymentDay" label="Ngày giao dịch" />
                                        <DatePicker
                                            style={{
                                                display: 'none',
                                            }}
                                            value={selectedDate}
                                            onChange={(date) => setSelectedDate(date)}
                                        />
                                    </div>
                                ),
                                width: 400,
                                key: 'paymentDay',
                                render: ({ ...props }: Payment) => {
                                    return (
                                        <p className="m-0">
                                            {moment(props.paymentDay).isValid() ? moment(props.paymentDay).format('DD/MM/YYYY HH:mm:ss') : 'Đang chờ'}
                                        </p>
                                    );
                                },
                                sorter: (a, b) => moment(a.paymentDay).valueOf() - moment(b.paymentDay).valueOf(),
                                filters: [
                                    { text: 'Hôm nay', value: 'today' },
                                    { text: 'Hôm qua', value: 'yesterday' },
                                    { text: 'Tuần này', value: 'thisWeek' },
                                    { text: 'Tuần trước', value: 'lastWeek' },
                                    { text: 'Tháng này', value: 'thisMonth' },
                                    { text: 'Tháng trước', value: 'lastMonth' },
                                ],
                                onFilter: (value, record) => {
                                    if (value === 'today') {
                                        return moment(record.paymentDay).isSame(moment(), 'day');
                                    } else if (value === 'yesterday') {
                                        return moment(record.paymentDay).isSame(moment().subtract(1, 'day'), 'day');
                                    } else if (value === 'thisWeek') {
                                        return moment(record.paymentDay).isSame(moment(), 'week');
                                    } else if (value === 'lastWeek') {
                                        return moment(record.paymentDay).isSame(moment().subtract(1, 'week'), 'week');
                                    } else if (value === 'thisMonth') {
                                        return moment(record.paymentDay).isSame(moment(), 'month');
                                    } else if (value === 'lastMonth') {
                                        return moment(record.paymentDay).isSame(moment().subtract(1, 'month'), 'month');
                                    }

                                    return true;
                                },
                            },
                            {
                                title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                                width: 400,
                                key: 'status',
                                render: ({ ...props }: Payment) => {
                                    return (
                                        <Tag
                                            className={clsx(`text-sm whitespace-normal`)}
                                            color={
                                                typeof props.status === 'string' && props.status === 'SUCCESS'
                                                    ? 'green'
                                                    : props.status === 'PENDING'
                                                    ? 'geekblue'
                                                    : 'volcano'
                                            }
                                        >
                                            {props.status === 'SUCCESS' ? 'Thành công' : props.status === 'PENDING' ? 'Đang chờ' : 'Từ chối'}
                                        </Tag>
                                    );
                                },
                                filters: [
                                    { text: 'Thành công', value: 'SUCCESS' },
                                    { text: 'Chờ duyệt', value: 'PENDING' },
                                    { text: 'Từ chối', value: 'DENIED' },
                                ],
                                onFilter: (value, record) => record.status === value,
                            },
                            {
                                title: () => <TableHeaderCell key="" label="" />,
                                width: 400,
                                key: 'action',
                                render: ({ ...props }: Payment) => {
                                    return (
                                        <EyeOutlined
                                            className="w-10 h-10 mt-4 text-blue-500 cursor-pointer"
                                            style={{ fontSize: '1.5rem' }}
                                            onClick={() => {
                                                setOpenDetailBankModal(true);
                                                setBankInfo({
                                                    bankName: props.bankName,
                                                    bankOwnerName: props.bankOwnerName,
                                                    bankAccountNumber: props.bankAccountNumber,
                                                    transferAmount: props.transferAmount,
                                                    code: props.code,
                                                    note: props.note,
                                                    paymentId: props.id,
                                                    paymentStatus: props.status,
                                                });
                                            }}
                                        />
                                    );
                                },
                            },
                        ]}
                    />
                </div>
            </Descriptions>
            <BankModal
                open={openDetailBankModal}
                afterClose={() => setOpenDetailBankModal(false)}
                onCancel={() => setOpenDetailBankModal(false)}
                bankInfo={bankInfo}
            />
        </div>
    );
};

export default PaymentList;
