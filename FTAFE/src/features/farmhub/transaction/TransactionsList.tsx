import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useQueryGetTransactionsAll } from '@hooks/api/payment.hook';
import { useDebounce } from '@hooks/useDebounce';
import { Transaction } from '@models/payment';
import { Descriptions, Input, Tag } from 'antd';
import clsx from 'clsx';
import moment from 'moment';
import React from 'react';

interface TransactionsListProps {}
const { Search } = Input;

const TransactionsList: React.FunctionComponent<TransactionsListProps> = () => {
    const { data, isLoading } = useQueryGetTransactionsAll();
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const listTransaction: Transaction[] = data || [];
    const filterData = listTransaction.filter(
        (i) => i.payeeName.toLowerCase().includes(debouncedValue.toLowerCase()) || i.payerName.toLowerCase().includes(debouncedValue.toLowerCase())
    );

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
                    <div className="flex items-center w-full gap-5">
                        <Search
                            placeholder="Tìm kiếm người gửi/người nhận"
                            allowClear
                            enterButton="Tìm kiếm"
                            size="middle"
                            onChange={(e) => setSearchText(e.target.value)} // Update search text
                            style={{ marginBottom: '1rem', marginTop: '1rem', width: '500px' }}
                        />
                    </div>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Transaction>
                        rowKey="id"
                        isLoading={isLoading}
                        data={filterData}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="userName" label="Tên người nhận" />,
                                width: 400,
                                key: 'userName',
                                render: ({ ...props }: Transaction) => <p className="m-0">{props.payeeName}</p>,
                                sorter: (a, b) => a.payeeName.localeCompare(b.payeeName),
                            },
                            {
                                title: () => <TableHeaderCell key="payer" label="Người gửi" />,
                                width: 400,
                                key: 'payer',
                                render: ({ ...props }: Transaction) => <p className="m-0">{props.payerName}</p>,
                                sorter: (a, b) => a.payerName.localeCompare(b.payerName),
                            },
                            {
                                title: () => <TableHeaderCell key="amount" label="Số tiền nhận(VNĐ)" />,
                                width: 400,
                                key: 'amount',
                                render: ({ ...props }: Transaction) => <p className="m-0">{props.amount.toLocaleString('en')}</p>,
                                sorter: (a, b) => a.amount - b.amount,
                            },
                            {
                                title: () => <TableHeaderCell key="transactionType" label="Phương thức giao dịch" />,
                                width: 400,
                                key: 'paymentMethod',
                                render: ({ ...props }: Transaction) => <p className="m-0">Nhận tiền</p>,
                            },
                            {
                                title: () => <TableHeaderCell key="paymentDate" label="Thời gian giao dịch" />,
                                width: 400,
                                key: 'paymentDate',
                                render: ({ ...props }: Transaction) => (
                                    <p className="m-0">{moment(props.paymentDate).format('DD/MM/YYYY HH:mm:ss')}</p>
                                ),
                                sorter: (a, b) => new Date(a.paymentDate).getTime() - new Date(b.paymentDate).getTime(),
                            },
                            {
                                title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                                width: 400,
                                key: 'status',
                                render: ({ ...props }: Transaction) => {
                                    return (
                                        <Tag
                                            className={clsx(`text-sm whitespace-normal`)}
                                            color={props.status === 'Success' ? 'green' : props.status === 'Pending' ? 'blue' : 'red'}
                                        >
                                            {props.status === 'Success' ? 'Thành công' : props.status === 'Pending' ? 'Đang chờ' : 'Từ chối'}
                                        </Tag>
                                    );
                                },
                                // filters: [
                                //     { text: 'Thành công', value: 'Success' },
                                //     { text: 'Chờ duyệt', value: 'Pending' },
                                //     { text: 'Từ chối', value: 'Denied' },
                                // ],
                                // onFilter: (value, record) => record.status === value,
                            },
                        ]}
                    />
                </div>
            </Descriptions>
        </div>
    );
};

export default TransactionsList;
