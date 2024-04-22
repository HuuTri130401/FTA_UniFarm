import { EyeOutlined } from '@ant-design/icons';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { PaymentAPI } from '@core/api/payment.api';
import { FilterPayment, Payment } from '@models/payment';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { DatePicker, Descriptions, Tag } from 'antd';
import clsx from 'clsx';
import { PlusIcon } from 'lucide-react';
import moment, { Moment } from 'moment';
import React, { useEffect, useState } from 'react';

import BankModal, { BankInfo } from './BankModal';
import WithDrawModal from './WithDrawModal';

interface PaymentListProps {
    filter: Partial<FilterPayment>;
}

const PaymentList: React.FC<PaymentListProps> = ({ filter }) => {
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['payments', filter],
        queryFn: async () => await PaymentAPI.getAllUser(filter),
    });
    const paymentList = data?.payload || [];
    useEffect(() => {
        setTotalItem(paymentList.length);
    }, [paymentList]);

    const [openDetailBankModal, setOpenDetailBankModal] = useState(false);
    const [openCreateModalState, setOpenCreateModalState] = useState(false);
    const [selectedDate, setSelectedDate] = useState<Moment | null>(null);
    const [bankInfo, setBankInfo] = useState<BankInfo>({} as BankInfo);
    const queryClient = useQueryClient();

    return (
        <div className="flex flex-col w-full gap-2">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Rút tiền`}
                className="p-4 bg-white rounded-lg"
                extra={
                    <button
                        type="button"
                        onClick={() => {
                            setOpenCreateModalState(!openCreateModalState);
                        }}
                        className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                    >
                        <PlusIcon className="w-5 h-5 text-white" />
                        <span>
                            <strong>Tạo yêu cầu rút tiền</strong>
                        </span>
                    </button>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Payment>
                        rowKey="id"
                        isLoading={isLoading}
                        data={paymentList}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="balance" label="Số dư(VNĐ)" />,
                                width: 400,
                                key: 'balance',
                                render: ({ ...props }: Payment) => <p className="m-0">{props.balance}</p>,
                                sorter: (a, b) => a.balance - b.balance,
                            },
                            {
                                title: () => <TableHeaderCell key="transferAmount" label="Tổng giao dịch(VNĐ)" />,
                                width: 400,
                                key: 'transferAmount',
                                render: ({ ...props }: Payment) => <p className="m-0">{props.transferAmount}</p>,
                                sorter: (a, b) => a.transferAmount - b.transferAmount,
                            },
                            {
                                title: () => <TableHeaderCell key="type" label="Loại giao dịch" />,
                                width: 400,
                                key: 'type',
                                render: ({ ...props }: Payment) => {
                                    return <p className="m-0">{props.type === 'WITHDRAW' ? 'Rút tiền' : 'Nạp tiền'}</p>;
                                },
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
                                            {props.paymentDay ? (
                                                moment(props.paymentDay).format('DD/MM/YYYY HH:mm:ss')
                                            ) : (
                                                <Tag color="blue">Đang chờ</Tag>
                                            )}
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
                                title: () => <TableHeaderCell key="action" label="" />,
                                width: 400,
                                key: 'action',
                                render: ({ ...props }: Payment) => {
                                    return (
                                        <button className="flex items-center font-semibold text-primary">
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
                                        </button>
                                    );
                                },
                            },
                        ]}
                    />
                </div>
            </Descriptions>
            <BankModal open={openDetailBankModal} onCancel={() => setOpenDetailBankModal(false)} bankInfo={bankInfo} />
            <WithDrawModal open={openCreateModalState} closeModal={() => setOpenCreateModalState(false)} />
        </div>
    );
};

export default PaymentList;
