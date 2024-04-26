import { DashOutlined } from '@ant-design/icons';
import { TextInput } from '@components/forms';
import FormFilterWrapper from '@components/forms/FormFilterWrapper';
import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { ApartmentAPI } from '@core/api/apartment.api';
import { IV1GetFilterExpert } from '@core/api/expert.api';
import { PlusIcon } from '@heroicons/react/24/outline';
import { Apartment, ApartmentFilter } from '@models/apartment';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Dropdown, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import { useRouter } from 'next/router';
import * as React from 'react';
import { toast } from 'react-toastify';

import CreateApartmentModal from './component/CreateApartmentComponent';
import UpdateApartmentModal from './component/UpdateApartmentComponent';

interface ApartmentListProps {
    filter: Partial<ApartmentFilter>;
}

const ApartmentList: React.FunctionComponent<ApartmentListProps> = ({ filter }) => {
    const router = useRouter();
    const { setTotalItem } = useTableUtil();

    const { data, isLoading } = useQuery({
        queryKey: ['apartments'],
        queryFn: async () => {
            const res = await ApartmentAPI.getAll(filter);
            setTotalItem(res.length);
            return res;
        },
    });

    const list: Apartment[] = data || [];

    const deleteApartmentMutation = useMutation(async (id: string) => await ApartmentAPI.deleteOne(id));

    const queryClient = useQueryClient();

    const handleDelete = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc muốn ngưng hoạt đồng căn hộ này',
            content: 'Bạn sẽ không thể khôi phục lại',
            okText: 'Tiếp tục',
            okType: 'danger',
            cancelText: 'Hủy',
            onOk: async () => {
                try {
                    await deleteApartmentMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries(['apartments']);
                            toast.success('Update căn hộ thành công');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting apartment:', error);
                }
            },
        });
    };
    //Open modal
    const [openCreateModalState, setOpenCreateModalState] = React.useState<boolean>(false);
    //Update modal
    const [updateModalState, setUpdateModalState] = React.useState<boolean>(false);
    const [currentValue, setCurrentValue] = React.useState<Apartment>({
        id: '',
        address: '',
        status: '',
        code: '',
        areaId: '',
        createdAt: '',
        updatedAt: '',
        name: '',
    });

    return (
        <div className="flex flex-col w-full gap-2">
            <div className="flex flex-col items-end w-full gap-2 ">
                <button
                    onClick={() => setOpenCreateModalState(!openCreateModalState)}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Tạo mới căn hộ</span>
                </button>
            </div>

            <FormFilterWrapper<IV1GetFilterExpert> defaultValues={{ ...filter }}>
                <div className="w-56">
                    <TextInput name="name" label="Tên" />
                </div>
                <div className="w-56">
                    <TextInput name="address" label="Địa chỉ" />
                </div>
            </FormFilterWrapper>

            <TableBuilder<Apartment>
                rowKey="id"
                isLoading={isLoading}
                data={list}
                columns={[
                    {
                        title: () => <TableHeaderCell key="code" label="Mã" />,
                        width: 400,
                        key: 'code',
                        render: ({ ...props }: Apartment) => <TableBodyCell label={<span>{props.code}</span>} />,
                    },
                    {
                        title: () => <TableHeaderCell key="name" sortKey="name" label="Tên" />,
                        width: 400,
                        key: 'name',
                        render: ({ ...props }: Apartment) => <TableBodyCell label={<span>{props.name}</span>} />,
                    },
                    {
                        title: () => <TableHeaderCell key="address" sortKey="address" label="Địa chỉ" />,
                        width: 400,
                        key: 'address',
                        render: ({ ...props }: Apartment) => <TableBodyCell label={<span>{props.address}</span>} />,
                    },
                    {
                        title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng Thái" />,
                        width: 100,
                        key: 'status',
                        render: ({ ...props }: Apartment) => {
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
                        render: ({ ...props }: Apartment) => {
                            return (
                                <Dropdown
                                    overlay={
                                        <Menu>
                                            {/* <Menu.Item key="1">
                                                <Button
                                                    style={{
                                                        background: '#bae0ff',
                                                    }}
                                                    onClick={() => {}}
                                                >
                                                    Chi tiết
                                                </Button>
                                            </Menu.Item> */}
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
                                                    Điều chỉnh
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
            <CreateApartmentModal
                onCancel={() => setOpenCreateModalState(false)}
                open={openCreateModalState}
                afterClose={() => setOpenCreateModalState(false)}
            />
            <UpdateApartmentModal
                open={updateModalState}
                currentValue={currentValue}
                onCancel={() => setUpdateModalState(false)}
                afterClose={() => setUpdateModalState(false)}
            />
        </div>
    );
};

export default ApartmentList;
