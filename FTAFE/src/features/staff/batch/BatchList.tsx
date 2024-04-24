import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { BatchAPI } from '@core/api/batch.api';
import { routes } from '@core/routes';
import { Batch } from '@models/batch';
import { useStoreUser } from '@store/index';
import { useQuery } from '@tanstack/react-query';
import { Button, Tag } from 'antd';
import clsx from 'clsx';
import moment from 'moment';
import { useRouter } from 'next/router';
import * as React from 'react';
import { BatchStatus } from 'src/constant/enum';

import UpdateBatchModal from './component/UpdateBatchModal';

interface BatchListProps {
    businessDayId: string;
}

const BatchList: React.FunctionComponent<BatchListProps> = ({ businessDayId }) => {
    const { setTotalItem } = useTableUtil();
    const user = useStoreUser();

    const { data, isLoading } = useQuery({
        queryKey: ['batch', `dayId-${businessDayId}`],
        queryFn: async () => {
            const res = await BatchAPI.getListInBusinessDay(user.collectedHub ? user.collectedHub?.id : user.id, businessDayId);
            setTotalItem(res.length);
            return res;
        },
    });
    const [updateState, setUpdateState] = React.useState<boolean>(false);
    const [batchId, setBatchId] = React.useState<string>('');

    const router = useRouter();

    return (
        <div className="flex flex-col w-full gap-10">
            <TableBuilder<Batch>
                rowKey="id"
                isLoading={isLoading}
                data={data?.payload}
                columns={[
                    {
                        title: () => <TableHeaderCell key="businessDayName" label="Tên sự kiện" />,
                        width: 400,
                        key: 'businessDayName',
                        render: ({ ...props }: Batch) => <p className="m-0">{props.businessDayName}</p>,
                        sorter: (a, b) => a.businessDayName.localeCompare(b.businessDayName),
                    },
                    {
                        title: () => <TableHeaderCell key="businessDayOpen" label="Ngày mở bán" />,
                        width: 400,
                        key: 'businessDayOpen',
                        render: ({ ...props }: Batch) => <p className="m-0">{props.businessDayOpen}</p>,
                        sorter: (a, b) => a.businessDayOpen.localeCompare(b.businessDayOpen),
                    },
                    {
                        title: () => <TableHeaderCell key="farmShipDate" label="Ngày giao hàng" />,
                        width: 400,
                        key: 'farmShipDate',
                        render: ({ ...props }: Batch) => <p className="m-0">{moment(props.farmShipDate).format('DD/MM/YYYY HH:mm:ss')}</p>,
                        sorter: (a, b) => moment(a.farmShipDate).valueOf() - moment(b.farmShipDate).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="collectedHubReceiveDate" label="Ngày nhận hàng" />,
                        width: 400,
                        key: 'collectedHubReceiveDate',
                        render: ({ ...props }: Batch) => <p className="m-0">{moment(props.collectedHubReceiveDate).format('DD/MM/YYYY HH:mm:ss')}</p>,
                        sorter: (a, b) => moment(a.collectedHubReceiveDate).valueOf() - moment(b.collectedHubReceiveDate).valueOf(),
                    },
                    {
                        title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
                        width: 400,
                        key: 'status',
                        render: ({ ...props }: Batch) => {
                            return (
                                <Tag
                                    className={clsx(`text-sm whitespace-normal`)}
                                    color={
                                        typeof props.status === 'string' && props.status === 'Received'
                                            ? '#13c2c2'
                                            : props.status === 'Pending'
                                            ? '#1677ff'
                                            : props.status === 'Processed'
                                            ? '#531dab'
                                            : 'volcano'
                                    }
                                >
                                    {props.status in BatchStatus ? BatchStatus[props.status as keyof typeof BatchStatus] : props.status}
                                </Tag>
                            );
                        },
                    },
                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'action',
                        render: ({ ...props }: Batch) => {
                            return (
                                <Button
                                    type="primary"
                                    onClick={() => {
                                        setUpdateState(!updateState);
                                        setBatchId(props.id);
                                    }}
                                >
                                    Xác nhận
                                </Button>
                            );
                        },
                    },
                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'detail',
                        render: ({ ...props }: Batch) => {
                            return (
                                <Button
                                    disabled={props.status === 'NotReceived'}
                                    type="primary"
                                    onClick={() => {
                                        router.push({
                                            pathname: routes.staff.order.inBatch(props.id),
                                            query: {
                                                businessDayId: props.businessDayId,
                                            },
                                        });
                                    }}
                                >
                                    Xem đơn hàng
                                </Button>
                            );
                        },
                    },
                ]}
            />
            <UpdateBatchModal
                bDayId={businessDayId}
                id={batchId}
                open={updateState}
                afterClose={() => setUpdateState(false)}
                onCancel={() => setUpdateState(false)}
            />
        </div>
    );
};

export default BatchList;
