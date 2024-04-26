import { TableActionCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { TransferAPI } from '@core/api/transfer.api';
import { routes } from '@core/routes';
import { useDebounce } from '@hooks/useDebounce';
import { Transfer } from '@models/transfer';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Input, Tag } from 'antd';
import { PlusIcon } from 'lucide-react';
import moment from 'moment';
import { useRouter } from 'next/router';
import * as React from 'react';
import { toast } from 'react-toastify';

interface TransferListProps {}
const { Search } = Input;

const TransferList: React.FunctionComponent<TransferListProps> = () => {
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['orders', 'collectedHub'],
        queryFn: async () => {
            const res = await TransferAPI.getAll();
            setTotalItem(res.data.length);
            return res;
        },
    });

    const router = useRouter();
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const transferList: Transfer[] = data?.data || [];
    const filterData = transferList.filter((i) => i.code.toLowerCase().includes(debouncedValue.toLowerCase()));
    const queryClient = useQueryClient();
    const resendTransferMutation = useMutation(async (id: string) => await TransferAPI.resend(id), {
        onSuccess: (data: any) => {
            queryClient.invalidateQueries(['orders', 'collectedHub']);
            toast.success('Gửi lại đơn hàng thành công');
        },
        onError: (error) => {
            console.log('error', error);
        },
    });

    return (
        <div className="flex flex-col w-full gap-10">
            <div className="flex flex-col items-end w-full gap-2 ">
                <button
                    onClick={() => router.push(routes.staff.transfer.station())}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Tạo đơn chuyển đến station</span>
                </button>
            </div>
            <Search
                placeholder="Tìm kiếm theo code"
                allowClear
                enterButton="Tìm kiếm"
                size="middle"
                onChange={(e) => setSearchText(e.target.value)} // Update search text
            />
            <TableBuilder<Transfer>
                rowKey="id"
                isLoading={isLoading}
                data={filterData}
                columns={[
                    {
                        title: () => <TableHeaderCell key="code" label="Mã" />,
                        width: 400,
                        key: 'code',
                        render: ({ ...props }: Transfer) => <p className="m-0">{props.code}</p>,
                        sorter: (a, b) => a.code.localeCompare(b.code),
                    },
                    {
                        title: () => <TableHeaderCell key="noteSend" label="Tin nhắn gửi đi" />,
                        width: 400,
                        key: 'noteSend',
                        render: ({ ...props }: Transfer) => <p className="m-0">{props.noteSend}</p>,
                        sorter: (a, b) => a.noteSend.localeCompare(b.noteSend),
                    },
                    {
                        title: () => <TableHeaderCell key="noteReceived" label="Tin nhắn nhận" />,
                        width: 400,
                        key: 'noteReceived',
                        render: ({ ...props }: Transfer) => <p className="m-0">{props.noteReceived}</p>,
                        sorter: (a, b) => a.noteReceived.localeCompare(b.noteReceived),
                    },
                    {
                        title: () => <TableHeaderCell key="createdAt" label="Ngày tạo" />,
                        width: 400,
                        key: 'createdAt',
                        render: ({ ...props }: Transfer) => <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}</p>,
                        sorter: (a, b) => moment(a.createdAt).valueOf() - moment(b.createdAt).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="expectedReceiveDate" label="Dự kiến ngày giao" />,
                        width: 400,
                        key: 'expectedReceiveDate',
                        render: ({ ...props }: Transfer) => <p className="m-0">{moment(props.expectedReceiveDate).format('DD/MM/YYYY HH:mm:ss')}</p>,
                        sorter: (a, b) => moment(a.expectedReceiveDate).valueOf() - moment(b.expectedReceiveDate).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                        width: 400,
                        key: 'status',
                        render: ({ ...props }: Transfer) => (
                            <p className="m-0">
                                {props.status === 'Received' ? (
                                    <Tag color="green">Đã nhận hàng</Tag>
                                ) : props.status === 'Pending' ? (
                                    <Tag color="gold">Đang chờ</Tag>
                                ) : props.status === 'Resend' ? (
                                    <Tag color="geekblue">Đang gửi lại</Tag>
                                ) : (
                                    <Tag color="red">Chưa nhận hàng</Tag>
                                )}
                            </p>
                        ),
                        filters: [
                            { text: 'Đang chờ', value: 'Pending' },
                            { text: 'Đã nhận hàng', value: 'Received' },
                            { text: 'Đang gửi lại', value: 'Resend' },
                            { text: 'Chưa nhận hàng', value: 'NotReceived' },
                            // Add more filters if needed
                        ],
                        onFilter: (value, record) => record.status === value,
                    },
                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'detail',
                        render: ({ ...props }: Transfer) => {
                            return (
                                <TableActionCell
                                    label={'Hành động'}
                                    actions={[
                                        {
                                            label: (
                                                <Button type="primary" color="#4096ff" className="w-full">
                                                    Xem chi tiết
                                                </Button>
                                            ),

                                            onClick: () => router.push(routes.staff.transfer.orderList(props.id)),
                                        },
                                        {
                                            label: (
                                                <Button
                                                    type="primary"
                                                    // style={{
                                                    //     background: '#391085',
                                                    // }}
                                                    disabled={props.status !== 'NotReceived'}
                                                    color="#391085"
                                                    className="w-full"
                                                >
                                                    Chuyển lại đơn hàng
                                                </Button>
                                            ),

                                            onClick: () => resendTransferMutation.mutate(props.id),
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

export default TransferList;
