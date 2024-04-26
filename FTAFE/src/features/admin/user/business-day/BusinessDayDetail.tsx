import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import { BusinessDay } from '@models/business-day';
import { FarmHubMenu } from '@models/farmhub-menu';
import { Badge, Descriptions, Spin, Tabs, Tag } from 'antd';
import clsx from 'clsx';
import moment from 'moment';
import Link from 'next/link';
import { useEffect } from 'react';

import BusinessDayStatChart from './components/BusinessDayStatChart';
interface BusinessDayDetailProps {
    value: BusinessDay;
}

const BusinessDayDetail: React.FC<BusinessDayDetailProps> = ({ value }) => {
    const menu: FarmHubMenu[] = value?.menus || [];
    const { setTotalItem } = useTableUtil();
    useEffect(() => {
        setTotalItem(menu.length);
    }, [menu]);

    return (
        <div className="flex flex-col w-full gap-4">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={'Thông tin liên quan đến ngày đăng bán'}
                className="p-4 bg-white rounded-lg"
            >
                <Descriptions.Item label="Tên sự kiện" span={2}>
                    {value?.name}
                </Descriptions.Item>
                <Descriptions.Item label="Trạng thái" span={1}>
                    <Badge
                        status={value?.status === 'Active' ? 'success' : 'error'}
                        color={
                            value?.status === 'Active'
                                ? 'blue'
                                : value?.status === 'Completed'
                                ? 'green'
                                : value?.status === 'PaymentConfirm'
                                ? 'magenta'
                                : 'red' // Mặc định cho các trạng thái khác
                        }
                        text={
                            value?.status === 'Active'
                                ? 'Hoạt động'
                                : value?.status === 'Completed'
                                ? 'Hoàn thành'
                                : value?.status === 'PaymentConfirm'
                                ? 'Chờ thanh toán'
                                : 'Không hoạt động'
                        }
                    />
                </Descriptions.Item>
                <Descriptions.Item label="Ngày đăng ký" span={1}>
                    {value?.regiterDay && moment(value?.regiterDay).format('DD/MM/YYYY')}
                </Descriptions.Item>
                <Descriptions.Item label="Ngày kết thúc đăng ký" span={1}>
                    {value?.endOfRegister && moment(value?.endOfRegister).format('DD/MM/YYYY')}
                </Descriptions.Item>

                <Descriptions.Item label="Ngày tạo" span={1}>
                    {moment(value?.createdAt).format('DD/MM/YYYY')}
                </Descriptions.Item>
            </Descriptions>
            <Tabs
                defaultActiveKey="1"
                centered
                className="bg-white rounded-lg"
                items={[
                    {
                        label: <p className="p-0 m-0 text-black ">Thống kê doanh thu</p>,
                        key: '1',
                        children: (
                            <div className="flex items-center justify-center">
                                {value?.id ? <BusinessDayStatChart id={value?.id} /> : <Spin size="large" />}
                            </div>
                        ),
                    },
                    {
                        label: <p className="p-0 m-0 text-black ">Các danh sách bán</p>,
                        key: '2',
                        children: (
                            <>
                                <Descriptions
                                    labelStyle={{
                                        fontWeight: 'bold',
                                    }}
                                    bordered
                                    className="p-4 bg-white rounded-lg"
                                    title={
                                        <div className="flex items-center justify-between">
                                            <p className="m-0">Menu</p>
                                        </div>
                                    }
                                >
                                    <div className="flex flex-col w-full gap-2">
                                        <TableBuilder<FarmHubMenu>
                                            rowKey="id"
                                            isLoading={false}
                                            data={menu}
                                            columns={[
                                                {
                                                    title: () => <TableHeaderCell key="name" sortKey="name" label="Menu" />,
                                                    width: 400,
                                                    key: 'name',
                                                    render: ({ ...props }: FarmHubMenu) => <p className="m-0">{props.name}</p>,
                                                },

                                                {
                                                    title: () => <TableHeaderCell key="tag" sortKey="tag" label="Mã tag" />,
                                                    width: 400,
                                                    key: 'tag',
                                                    render: ({ ...props }: FarmHubMenu) => <p className="m-0">{props.tag}</p>,
                                                },
                                                {
                                                    title: () => <TableHeaderCell key="createdAt" sortKey="createdAt" label="Ngày tạo" />,
                                                    width: 400,
                                                    key: 'createdAt',
                                                    render: ({ ...props }: FarmHubMenu) => (
                                                        <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY')}</p>
                                                    ),
                                                },

                                                {
                                                    title: () => <TableHeaderCell key="status" sortKey="status" label="Status" />,
                                                    width: 400,
                                                    key: 'status',
                                                    render: ({ ...props }: FarmHubMenu) => {
                                                        return (
                                                            <Tag
                                                                className={clsx(`text-sm whitespace-normal`)}
                                                                color={
                                                                    typeof props.status === 'string' && props.status === 'Active'
                                                                        ? 'geekblue'
                                                                        : 'volcano'
                                                                }
                                                            >
                                                                {props.status === 'Active'
                                                                    ? 'Hoạt động'
                                                                    : props.status === 'PENDING'
                                                                    ? 'Đang chờ'
                                                                    : 'Không hoạt động'}
                                                            </Tag>
                                                        );
                                                    },
                                                },
                                                {
                                                    title: () => <TableHeaderCell key="" sortKey="" label="Nhà cung cấp" />,
                                                    width: 200,
                                                    key: 'detail',
                                                    render: ({ ...props }: FarmHubMenu) => {
                                                        return (
                                                            <div>
                                                                <Link
                                                                    href={routes.admin.user.farm_hub.detail(props.farmHubId)}
                                                                    className="py-2 text-center text-white cursor-pointer bg-primary"
                                                                >
                                                                    Chi tiết
                                                                </Link>
                                                            </div>
                                                        );
                                                    },
                                                },
                                            ]}
                                        />
                                    </div>
                                </Descriptions>
                            </>
                        ),
                    },
                ]}
            />
        </div>
    );
};

export default BusinessDayDetail;
