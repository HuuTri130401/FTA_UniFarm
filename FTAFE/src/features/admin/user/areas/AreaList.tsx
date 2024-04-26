import { DashOutlined } from '@ant-design/icons';
import { TextInput } from '@components/forms';
import FormFilterWrapper from '@components/forms/FormFilterWrapper';
import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { AreaAPI } from '@core/api/area.api';
import { IV1GetFilterExpert } from '@core/api/expert.api';
import { PlusIcon } from '@heroicons/react/24/outline';
import { Area, AreaFilter } from '@models/area';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Descriptions, Dropdown, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import { useRouter } from 'next/router';
import * as React from 'react';
import { toast } from 'react-toastify';

import AreaCreateModal from './components/AreaCreateModal';
import UpdateAreaModal from './components/AreaUpdateModal';

interface AreaListProps {
    filter: Partial<AreaFilter>;
}

const AreaList: React.FunctionComponent<AreaListProps> = ({ filter }) => {
    const router = useRouter();
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['areas', filter],
        queryFn: async () => {
            const res = await AreaAPI.getAll(filter);
            setTotalItem(res.length);
            return res;
        },
    });

    const areas: Area[] = data || [];

    const deleteAreaMutation = useMutation(async (id: string) => await AreaAPI.deleteOne(id));

    const queryClient = useQueryClient();

    const handleDelete = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc chắn không?',
            content: 'Bạn không thể phục hồi dữ liệu này!',
            okText: 'Xoá!',
            okType: 'danger',
            cancelText: 'Huỷ',
            onOk: async () => {
                try {
                    await deleteAreaMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['areas', filter]);
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
    const [currentValue, setCurrentValue] = React.useState<Area>({
        id: '',
        province: '',
        district: '',
        commune: '',
        address: '',
        status: '',
        code: '',
    });

    return (
        <div className="flex flex-col w-full gap-2">
            <FormFilterWrapper<IV1GetFilterExpert> defaultValues={{ ...filter }}>
                <div className="w-56">
                    <TextInput name="address" label="Địa chỉ" />
                </div>
                <div className="w-56">
                    <TextInput name="commune" label="Xã" />
                </div>
                <div className="w-56">
                    <TextInput name="district" label="Quận" />
                </div>
                <div className="w-56">
                    <TextInput name="province" label="Tỉnh" />
                </div>
            </FormFilterWrapper>
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Danh sách Khu vực`}
                className="p-4 bg-white rounded-lg"
                extra={
                    <div className="flex flex-col items-end w-full gap-2 ">
                        <button
                            onClick={() => setOpenCreateModalState(!openCreateModalState)}
                            className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                        >
                            <PlusIcon className="w-5 h-5 text-white" />
                            <span>Tạo mới khu vực</span>
                        </button>
                    </div>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Area>
                        rowKey="id"
                        isLoading={isLoading}
                        data={areas}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="province" sortKey="province" label="Tỉnh" />,
                                width: 400,
                                key: 'province',
                                render: ({ ...props }: Area) => <TableBodyCell label={<span>{props.province}</span>} />,
                            },
                            {
                                title: () => <TableHeaderCell key="district" sortKey="district" label="Quận" />,
                                width: 400,
                                key: 'district',
                                render: ({ ...props }: Area) => <TableBodyCell label={<span>{props.district}</span>} />,
                            },
                            {
                                title: () => <TableHeaderCell key="commune" sortKey="commune" label="Xã" />,
                                width: 400,
                                key: 'commune',
                                render: ({ ...props }: Area) => <TableBodyCell label={<span>{props.commune}</span>} />,
                            },

                            {
                                title: () => <TableHeaderCell key="address" sortKey="address" label="Địa chỉ" />,
                                width: 400,
                                key: 'address',
                                render: ({ ...props }: Area) => <TableBodyCell label={<span>{props.address}</span>} />,
                            },
                            {
                                title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
                                width: 100,
                                key: 'status',
                                render: ({ ...props }: Area) => {
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
                            {
                                title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                width: 50,
                                key: 'action',
                                render: ({ ...props }: Area) => {
                                    return (
                                        <Dropdown
                                            overlay={
                                                <Menu>
                                                    <Menu.Item key="1">
                                                        <Button
                                                            style={{
                                                                background: '#bae0ff',
                                                                width: '100px',
                                                            }}
                                                            className="bg-slate-500"
                                                            onClick={() => router.push(`/admin/area/${props.id}`)}
                                                        >
                                                            Chi tiết
                                                        </Button>
                                                    </Menu.Item>
                                                    <Menu.Item key="2">
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
                                                            Điều Chỉnh
                                                        </Button>
                                                    </Menu.Item>

                                                    <Menu.Item key="3">
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
                </div>
            </Descriptions>
            <AreaCreateModal
                open={openCreateModalState}
                afterClose={() => setOpenCreateModalState(false)}
                onCancel={() => setOpenCreateModalState(false)}
            />
            <UpdateAreaModal
                open={updateModalState}
                currentValue={currentValue}
                onCancel={() => setUpdateModalState(false)}
                afterClose={() => setUpdateModalState(false)}
            />
        </div>
    );
};

export default AreaList;
