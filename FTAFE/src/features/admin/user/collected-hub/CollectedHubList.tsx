import { DashOutlined } from '@ant-design/icons';
import { TextInput } from '@components/forms';
import FormFilterWrapper from '@components/forms/FormFilterWrapper';
import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { CollectedHubAPI, CollectedHubFilter } from '@core/api/collected-hub.api';
import { IV1GetFilterExpert } from '@core/api/expert.api';
import CreateStaffModal from '@features/staff/components/CreateStaffModal';
import { PlusIcon } from '@heroicons/react/24/outline';
import { CollectedHub } from '@models/staff';
import { UserRole } from '@models/user';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { stringHelper } from '@utils/index';
import { Button, Dropdown, Image, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import Link from 'next/link';
import * as React from 'react';
import { toast } from 'react-toastify';

import CreateCollectedHubModal from './components/CreateCollectedHubModal';
import UpdateCollectedHubModal from './components/UpdateCollectedHubModal';

interface CollectedHubListProps {
    filter: Partial<CollectedHubFilter>;
}

const CollectedHubList: React.FunctionComponent<CollectedHubListProps> = ({ filter }) => {
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['collected-hub-list'],
        queryFn: async () => {
            const res = await CollectedHubAPI.getAll({
                ...filter,
                pageSize: 999,
            });
            setTotalItem(res.length);
            return res;
        },
    });
    const hubs: CollectedHub[] = data;

    const deleteCollectedHubMutation = useMutation(async (id: string) => await CollectedHubAPI.deleteOne(id));

    const queryClient = useQueryClient();

    const handleDelete = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc muốn ngưng hoạt đồng kho hàng này',
            content: 'Bạn sẽ không thể khôi phục lại',
            okText: 'Tiếp tục',
            okType: 'danger',
            cancelText: 'Hủy',
            onOk: async () => {
                try {
                    await deleteCollectedHubMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['collected-hub-list']);
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
    const [currentValue, setCurrentValue] = React.useState<CollectedHub>({
        id: '',
        name: '',
        description: '',
        image: '',
        code: '',
        status: '',
        address: '',
        createdAt: '',
        updatedAt: '',
    });
    //Create Staff modal
    const [createStaffModal, setCreateStaffModalState] = React.useState(false);

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
                    <span>Tạo nhân viên kho</span>
                </button>
                <button
                    onClick={() => setOpenCreateModalState(!openCreateModalState)}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Tạo thêm kho</span>
                </button>
            </div>

            <FormFilterWrapper<IV1GetFilterExpert> defaultValues={{ ...filter }}>
                <div className="w-56">
                    <TextInput name="name" label="Tên" />
                </div>
                <div className="w-56">
                    <TextInput name="description" label="Mô tả" />
                </div>
                <div className="w-56">
                    <TextInput name="address" label="Địa chỉ" />
                </div>
            </FormFilterWrapper>

            <TableBuilder<CollectedHub>
                rowKey="id"
                isLoading={isLoading}
                data={hubs}
                columns={[
                    {
                        title: () => <TableHeaderCell key="image" label="Hình ảnh" />,
                        width: 100,
                        key: 'image',
                        render: ({ ...props }: CollectedHub) => (
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
                        render: ({ ...props }: CollectedHub) => {
                            return <TableBodyCell label={<Link href={`collected-hub-staff/${props.id}`}>{props.name}</Link>} />;
                        },
                        sorter: (a, b) => a.name.localeCompare(b.name),
                    },
                    {
                        title: () => <TableHeaderCell key="description" label="Mô tả" />,
                        width: 400,
                        key: 'description',
                        render: ({ ...props }: CollectedHub) => <TableBodyCell label={<span>{props.description}</span>} />,
                    },

                    {
                        title: () => <TableHeaderCell key="address" label="Địa chỉ" />,
                        width: 400,
                        key: 'address',
                        render: ({ ...props }: CollectedHub) => <TableBodyCell label={<span>{props.address}</span>} />,
                        sorter: (a, b) => a.address.localeCompare(b.address),
                    },
                    {
                        title: () => <TableHeaderCell key="status" label="Trạng thái" />,
                        width: 100,
                        key: 'status',
                        render: ({ ...props }: CollectedHub) => {
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
            <CreateCollectedHubModal
                open={openCreateModalState}
                afterClose={() => setOpenCreateModalState(false)}
                onCancel={() => setOpenCreateModalState(false)}
            />
            <UpdateCollectedHubModal
                open={updateModalState}
                currentValue={currentValue}
                onCancel={() => setUpdateModalState(false)}
                afterClose={() => setUpdateModalState(false)}
            />
            <CreateStaffModal
                open={createStaffModal}
                afterClose={() => setCreateStaffModalState(false)}
                onCancel={() => setCreateStaffModalState(false)}
                role={UserRole.COLLECTED_STAFF}
            />
        </div>
    );
};

export default CollectedHubList;
