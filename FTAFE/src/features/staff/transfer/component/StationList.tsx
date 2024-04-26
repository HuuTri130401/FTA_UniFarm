import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { StationAPI } from '@core/api/station.api';
import { routes } from '@core/routes';
import { useDebounce } from '@hooks/useDebounce';
import { Station } from '@models/staff';
import { useQuery } from '@tanstack/react-query';
import { stringHelper } from '@utils/index';
import { Button, Image, Input, Tag } from 'antd';
import clsx from 'clsx';
import { useRouter } from 'next/router';
import * as React from 'react';

interface StationListProps {}
const { Search } = Input;

const StationList: React.FunctionComponent<StationListProps> = () => {
    const router = useRouter();
    const { setTotalItem, setPageSize } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['stations'],
        queryFn: async () => {
            const res = await StationAPI.getAll();
            setTotalItem(res.length);
            return res;
        },
    });
    const hubs: Station[] = data || [];
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const filterData = hubs.filter((i) => i.name.toLowerCase().includes(debouncedValue.toLowerCase()));
    return (
        <div className="flex flex-col w-full gap-2">
            <Search
                placeholder="Tìm kiếm theo tên"
                allowClear
                enterButton="Tìm kiếm"
                size="middle"
                onChange={(e) => setSearchText(e.target.value)} // Update search text
            />

            <TableBuilder<Station>
                rowKey="id"
                isLoading={isLoading}
                data={filterData}
                columns={[
                    {
                        title: () => <TableHeaderCell key="image" label="Hình ảnh" />,
                        width: 100,
                        key: 'image',
                        render: ({ ...props }: Station) => (
                            <TableBodyCell
                                label={
                                    <Image
                                        alt=""
                                        width={64}
                                        height={64}
                                        className="rounded overflow-hidden"
                                        src={props.image ? props.image : stringHelper.convertTextToAvatar(props.name)}
                                    />
                                }
                            />
                        ),
                    },
                    {
                        title: () => <TableHeaderCell key="name" label="Tên" />,
                        width: 400,
                        key: 'name',
                        render: ({ ...props }: Station) => {
                            return <TableBodyCell label={<p className="m-0">{props.name}</p>} />;
                        },
                    },
                    {
                        title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                        width: 100,
                        key: 'status',
                        render: ({ ...props }: Station) => {
                            return (
                                <Tag
                                    className={clsx(`text-sm whitespace-normal`)}
                                    color={
                                        typeof props.status === 'string' && props.status === 'Active'
                                            ? 'green'
                                            : props.status === 'Active'
                                            ? 'geekblue'
                                            : 'volcano'
                                    }
                                >
                                    {props.status === 'Active' ? 'Hoạt động' : props.status === 'PENDING' ? 'Đang chờ' : 'Không hoạt động'}
                                </Tag>
                            );
                        },
                        filters: [
                            { text: 'Active', value: 'Active' },
                            { text: 'Inactive', value: 'Inactive' },
                            // Add more filters if needed
                        ],
                        onFilter: (value, record) => record.status === value,
                    },
                    {
                        title: () => <TableHeaderCell key="action" label="" />,
                        width: 100,
                        key: 'action',
                        render: ({ ...props }: Station) => (
                            <Button
                                onClick={() => {
                                    router.push({
                                        pathname: routes.staff.transfer.create(),
                                        query: {
                                            stationId: props.id,
                                        },
                                    });
                                }}
                            >
                                Tạo đơn chuyển hàng
                            </Button>
                        ),
                    },
                ]}
            />
        </div>
    );
};

export default StationList;
