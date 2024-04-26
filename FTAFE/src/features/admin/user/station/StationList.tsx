import { DashOutlined } from '@ant-design/icons';
import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { StationAPI, StationFilter } from '@core/api/station.api';
import CreateStaffModal from '@features/staff/components/CreateStaffModal';
import { PlusIcon } from '@heroicons/react/24/outline';
import { useDebounce } from '@hooks/useDebounce';
import { Station } from '@models/staff';
import { UserRole } from '@models/user';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { stringHelper } from '@utils/index';
import { Button, Dropdown, Image, Input, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import Link from 'next/link';
import { useRouter } from 'next/router';
import * as React from 'react';
import { toast } from 'react-toastify';

import CreateStationModal from './components/CreateStationModal';
import UpdateStationModal from './components/UpdateStationModal';

interface StationListProps {
    filter: Partial<StationFilter>;
}
const { Search } = Input;

const StationList: React.FunctionComponent<StationListProps> = ({ filter }) => {
    const router = useRouter();
    const { setTotalItem, setPageSize } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['stations', filter],
        queryFn: async () => {
            const res = await StationAPI.getAll();
            setTotalItem(res.length);
            return res;
        },
    });
    const hubs: Station[] = data || [];

    const deleteCollectedHubMutation = useMutation(async (id: string) => await StationAPI.deleteOne(id));

    const queryClient = useQueryClient();
    const [createStaffModal, setCreateStaffModalState] = React.useState(false);
    const handleDelete = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc muốn ngưng hoạt đồng trạm vận chuyển này',
            content: 'Bạn sẽ không thể khôi phục lại',
            okText: 'Tiếp tục',
            okType: 'danger',
            cancelText: 'Hủy',
            onOk: async () => {
                try {
                    await deleteCollectedHubMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['stations', filter]);
                            toast.success('FarmHub deleted successfully!');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting FarmHub:', error);
                }
            },
        });
    };
    //Open modal
    const [openCreateModalState, setOpenCreateModalState] = React.useState<boolean>(false);
    //Update modal
    const [updateModalState, setUpdateModalState] = React.useState<boolean>(false);
    const [currentValue, setCurrentValue] = React.useState<Station>({
        id: '',
        name: '',
        description: '',
        image: '',
        code: '',
        status: '',
        address: '',
        createdAt: '',
        updatedAt: '',
        areaId: '',
        area: null,
    });

    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const filterData = hubs.filter(
        (i) =>
            i.name.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.code.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.address.toLowerCase().includes(debouncedValue.toLowerCase())
    );

    return (
        <div className="flex flex-col w-full gap-2">
            <div className="flex flex-row-reverse items-end w-full gap-2 ">
                <button
                    onClick={() => {
                        setCreateStaffModalState(!createStaffModal);
                    }}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Tạo nhân viên Trạm</span>
                </button>
                <button
                    onClick={() => setOpenCreateModalState(!openCreateModalState)}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Tạo Trạm mới</span>
                </button>
            </div>

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
                        width: 300,
                        key: 'name',
                        render: ({ ...props }: Station) => {
                            return <TableBodyCell label={<Link href={`station/${props.id}`}>{props.name}</Link>} />;
                        },
                        sorter: (a, b) => a.name.localeCompare(b.name),
                    },
                    {
                        title: () => <TableHeaderCell key="description" label="Mô tả" />,
                        width: 400,
                        key: 'description',
                        render: ({ ...props }: Station) => <TableBodyCell label={<span>{props.description}</span>} />,
                    },

                    {
                        title: () => <TableHeaderCell key="address" sortKey="address" label="Địa chỉ" />,
                        width: 400,
                        key: 'address',
                        render: ({ ...props }: Station) => <TableBodyCell label={<span>{props.address}</span>} />,
                        sorter: (a, b) => a.address.localeCompare(b.address),
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
                        filters: [
                            { text: 'Hoạt động', value: 'Active' },
                            { text: 'Không hoạt động', value: 'Inactive' },
                            // Add more filters if needed
                        ],
                        onFilter: (value, record) => record.status === value,
                    },
                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 50,
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
                                                        setCurrentValue(props);
                                                        setUpdateModalState(!updateModalState);
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
                                                    type="primary"
                                                    danger
                                                    onClick={() => handleDelete(props.id)}
                                                >
                                                    Xoá
                                                </Button>
                                            </Menu.Item>
                                        </Menu>
                                    }
                                    trigger={['click']}
                                >
                                    <DashOutlined />
                                </Dropdown>
                            );
                        },
                    },
                ]}
            />
            <CreateStationModal
                open={openCreateModalState}
                afterClose={() => setOpenCreateModalState(false)}
                onCancel={() => setOpenCreateModalState(false)}
            />
            <UpdateStationModal
                open={updateModalState}
                currentValue={currentValue}
                onCancel={() => setUpdateModalState(false)}
                afterClose={() => setUpdateModalState(false)}
            />
            <CreateStaffModal
                open={createStaffModal}
                afterClose={() => setCreateStaffModalState(false)}
                onCancel={() => setCreateStaffModalState(false)}
                role={UserRole.STATION_STAFF}
            />
        </div>
    );
};

export default StationList;
