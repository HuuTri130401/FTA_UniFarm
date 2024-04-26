import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { FarmHubAPI } from '@core/api/farmhub.api';
import { routes } from '@core/routes';
import { useQueryFarmHub } from '@hooks/api/farmhub.hook';
import { useDebounce } from '@hooks/useDebounce';
import { FarmHub, UserRole } from '@models/user';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { stringHelper } from '@utils/index';
import { Button, DatePicker, Descriptions, Dropdown, Image, Input, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import { PlusIcon } from 'lucide-react';
import moment from 'moment';
import Link from 'next/link';
import * as React from 'react';
import { toast } from 'react-toastify';

import CreateFarmHubModal from './component/CreateFarmHubModal';
import UpdateFarmHubModal from './component/UpdateFarmHubModal';
const { RangePicker } = DatePicker;
const { Search } = Input;
interface FarmHubListProps {}

const FarmHubList: React.FunctionComponent<FarmHubListProps> = () => {
    const { data, isLoading } = useQueryFarmHub();

    const farmHub: FarmHub[] = data?.payload || [];

    const deleteFarmHubMutation = useMutation({
        mutationKey: ['farm-hub'],
        mutationFn: async (id: string) => await FarmHubAPI.deleteFarmHub(id),
    });
    const queryClient = useQueryClient();

    const handleDelete = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc muốn ngưng hoạt đồng nông trại này',
            content: 'Bạn sẽ không thể khôi phục lại',
            okText: 'Tiếp tục',
            okType: 'danger',
            cancelText: 'Hủy',
            onOk: async () => {
                try {
                    await deleteFarmHubMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['farm-hub']);
                            toast.success('FarmHub deleted successfully!');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting FarmHub:', error);
                }
            },
        });
    };

    const [createModalState, setCreateModalState] = React.useState<boolean>(false);
    const [updateModalState, setUpdateModalState] = React.useState<boolean>(false);
    const [farmHubValue, setFarmHubValue] = React.useState<FarmHub>({
        name: '',
        description: '',
        image: '',
        code: '',
        status: '',
        address: '',
        updatedAt: '',
        roleName: UserRole.FARM_HUB,
        createdAt: '',
        id: '',
    });

    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const sortedData = farmHub.sort((a, b) => moment(b.createdAt).valueOf() - moment(a.createdAt).valueOf());
    const filterData = sortedData.filter((f) => f.name.toLowerCase().includes(debouncedValue.toLowerCase()));

    return (
        <div className="flex flex-col w-full gap-2">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Danh sách nông trại`}
                className="p-4 bg-white rounded-lg"
                extra={
                    <div className="flex items-center w-full gap-5">
                        <Search
                            placeholder="Tìm kiếm"
                            allowClear
                            enterButton="Tìm kiếm"
                            size="middle"
                            onChange={(e) => setSearchText(e.target.value)} // Update search text
                            style={{ marginBottom: '1rem', marginTop: '1rem', width: '300px' }}
                        />
                        <button
                            onClick={() => {
                                setCreateModalState(!createModalState);
                            }}
                            className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                        >
                            <PlusIcon className="w-5 h-5 text-white" />
                            <span>Tạo mới nông trại</span>
                        </button>
                    </div>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<FarmHub>
                        rowKey="id"
                        isLoading={isLoading}
                        data={filterData}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="image" label="Hình ảnh" />,
                                width: 400,
                                key: 'image',
                                render: ({ ...props }: FarmHub) => (
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
                                render: ({ ...props }: FarmHub) => (
                                    <TableBodyCell label={<Link href={routes.admin.user.farm_hub.detail(props.id)}>{props.name}</Link>} />
                                ),
                                sorter: (a, b) => a.name.localeCompare(b.name),
                            },
                            {
                                title: () => <TableHeaderCell key="createdAt" label="Ngày tạo" />,
                                width: 400,
                                key: 'createdAt',
                                render: ({ ...props }: FarmHub) => (
                                    <TableBodyCell
                                        label={
                                            <Link href={routes.admin.user.farm_hub.detail(props.id)}>
                                                {moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}
                                            </Link>
                                        }
                                    />
                                ),
                                sorter: (a, b) => moment(a.createdAt).valueOf() - moment(b.createdAt).valueOf(),
                            },
                            {
                                title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                                width: 400,
                                key: 'status',
                                render: ({ ...props }: FarmHub) => {
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
                                title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                width: 400,
                                key: 'action',
                                render: ({ ...props }) => {
                                    return (
                                        <Dropdown
                                            overlay={
                                                <Menu>
                                                    <Menu.Item key="1">
                                                        <Button
                                                            style={{
                                                                width: '100px',
                                                            }}
                                                            type="primary"
                                                            onClick={() => {
                                                                setUpdateModalState(!updateModalState);
                                                                setFarmHubValue(props);
                                                            }}
                                                        >
                                                            Điều chỉnh
                                                        </Button>
                                                    </Menu.Item>

                                                    <Menu.Item key="2">
                                                        <Button
                                                            style={{
                                                                width: '100px',
                                                            }}
                                                            danger
                                                            type="primary"
                                                            onClick={() => handleDelete(props?.id)}
                                                        >
                                                            Xoá
                                                        </Button>
                                                    </Menu.Item>
                                                </Menu>
                                            }
                                            trigger={['click']}
                                        >
                                            <span className="cursor-pointer">Chỉnh sửa</span>
                                        </Dropdown>
                                    );
                                },
                            },
                        ]}
                    />
                </div>
            </Descriptions>
            <CreateFarmHubModal open={createModalState} afterClose={() => setCreateModalState(false)} onCancel={() => setCreateModalState(false)} />
            <UpdateFarmHubModal
                currentValue={farmHubValue}
                open={updateModalState}
                afterClose={() => setUpdateModalState(false)}
                onCancel={() => setUpdateModalState(false)}
            />
        </div>
    );
};

export default FarmHubList;
