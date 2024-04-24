import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { AreaAPI } from '@core/api/area.api';
import { routes } from '@core/routes';
import { Area } from '@models/area';
import { Station } from '@models/staff';
import { useQuery } from '@tanstack/react-query';
import { stringHelper } from '@utils/index';
import { Descriptions, Image, Tag } from 'antd';
import clsx from 'clsx';
import Link from 'next/link';
interface AreaDetailProps {
    value: Area;
}

const AreaDetail: React.FC<AreaDetailProps> = ({ value }) => {
    const { data, isLoading } = useQuery({
        queryKey: ['area', 'station'],
        queryFn: async () => await AreaAPI.getStationsInArea(value.id),
    });
    const stationList: Station[] = data?.payload || [];

    return (
        <div className="flex flex-col w-full gap-4">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={'Danh sách trạm hàng hóa trong khu vực'}
                className="p-4 bg-white rounded-lg"
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Station>
                        rowKey="id"
                        isLoading={isLoading}
                        data={stationList}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="image" sortKey="image" label="Ảnh" />,
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
                                title: () => <TableHeaderCell key="name" sortKey="name" label="Tên" />,
                                width: 300,
                                key: 'name',
                                render: ({ ...props }: Station) => {
                                    return <TableBodyCell label={<Link href={routes.admin.user.station.detail(props.id)}>{props.name}</Link>} />;
                                },
                            },
                            {
                                title: () => <TableHeaderCell key="description" sortKey="description" label="Mô tả" />,
                                width: 400,
                                key: 'description',
                                render: ({ ...props }: Station) => <TableBodyCell label={<span>{props.description}</span>} />,
                            },

                            {
                                title: () => <TableHeaderCell key="address" sortKey="address" label="Địa chỉ" />,
                                width: 400,
                                key: 'address',
                                render: ({ ...props }: Station) => <TableBodyCell label={<span>{props.address}</span>} />,
                            },
                            {
                                title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
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
                            },
                        ]}
                    />
                </div>
            </Descriptions>
        </div>
    );
};

export default AreaDetail;
