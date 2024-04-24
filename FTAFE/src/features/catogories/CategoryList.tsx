import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { CategoryAPI } from '@core/api/category.api';
import { useDebounce } from '@hooks/useDebounce';
import { Category } from '@models/category';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { stringHelper } from '@utils/index';
import { Button, Descriptions, Dropdown, Image, Input, Menu, Modal, Tag } from 'antd';
import clsx from 'clsx';
import { PlusIcon } from 'lucide-react';
import * as React from 'react';
import { toast } from 'react-toastify';

import CategoryDetailModal from './components/CategoryDetailModal';
import CreateCategoryModal from './components/CreateCategoryModal';
import UpdateCategoryModal from './components/UpdateCategoryModal';

interface CategoryListProps {}
const { Search } = Input;

const CategoryList: React.FunctionComponent<CategoryListProps> = () => {
    const [openDetailModal, setOpenDetailModalState] = React.useState<boolean>(false);
    const [categoryDetailId, setCategoryDetailId] = React.useState<string>('');
    const queryClient = useQueryClient();
    const setOpenDetailModal = (isOpen: boolean, categoryId: string) => {
        setCategoryDetailId(categoryId);
        setOpenDetailModalState(isOpen);
    };

    const [openUpdateModal, setOpenUpdateModalState] = React.useState<boolean>(false);
    const [defaultCategory, setDefaultCategoryState] = React.useState<Category>({
        id: '',
        name: '',
        description: '',
        image: '',
        code: '',
        status: '',
        displayIndex: 0,
        systemPrice: 0,
        minSystemPrice: 0,
        maxSystemPrice: 0,
        margin: 0,
        createdAt: '',
        updatedAt: '',
    });
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['categories'],
        queryFn: async () => {
            const res = await CategoryAPI.getAllCategories();
            return res;
        },
    });

    const deleteCategoryMutation = useMutation({
        mutationFn: async (categoryId: string) => {
            const res = await CategoryAPI.deleteCategory(categoryId);
            return res;
        },
    });

    const [openCreateModalState, setOpenCreateModalState] = React.useState<boolean>(false);

    const handleDeleteCategory = (id: string) => {
        Modal.confirm({
            title: 'Bạn có chắc chắn không?',
            content: 'Bạn không thể phục hồi dữ liệu này!',
            okText: 'Xoá!',
            okType: 'danger',
            cancelText: 'Huỷ',
            onOk: async () => {
                try {
                    await deleteCategoryMutation.mutateAsync(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries();
                            toast.success('Category deleted successfully!');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting FarmHub:', error);
                }
            },
        });
    };

    const categories: Category[] = data?.payload || [];

    React.useEffect(() => {
        setTotalItem(categories?.length);
    }, [categories]);
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const filterData = categories.filter(
        (i) => i.name.toLowerCase().includes(debouncedValue.toLowerCase()) || i.code.toLowerCase().includes(debouncedValue.toLowerCase())
    );

    return (
        <div className="flex flex-col w-full gap-2">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Danh sách phân loại sản phẩm`}
                className="p-4 bg-white rounded-lg"
                extra={
                    <div className="flex items-center w-full gap-5 ">
                        <Search
                            placeholder="Tìm kiếm"
                            allowClear
                            enterButton="Tìm kiếm"
                            size="middle"
                            onChange={(e) => setSearchText(e.target.value)} // Update search text
                            style={{ marginBottom: '1rem', marginTop: '1rem', width: '300px' }}
                        />
                        <button
                            onClick={() => setOpenCreateModalState(!openCreateModalState)}
                            className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                        >
                            <PlusIcon className="w-5 h-5 text-white" />
                            <span>Tạo thêm loại sản phẩm</span>
                        </button>
                    </div>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Category>
                        rowKey="id"
                        isLoading={isLoading}
                        data={filterData}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="image" label="Ảnh" />,
                                width: 200,
                                key: 'image',
                                render: ({ ...props }: Category) => (
                                    <TableBodyCell
                                        label={
                                            <Image
                                                alt={props.name}
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
                                render: ({ ...props }: Category) => (
                                    <Button type="text" onClick={() => setOpenDetailModal(true, props.id)}>
                                        {props.name}
                                    </Button>
                                ),
                                sorter: (a, b) => a.name.localeCompare(b.name),
                            },
                            {
                                title: () => <TableHeaderCell key="description" label="Mô tả" />,
                                width: 400,
                                key: 'description',
                                render: ({ ...props }: Category) => (
                                    // <TableBodyCell label={<Link href={routes.admin.user.farm_hub.detail(props.id)}></Link>} />
                                    <span>{props.description}</span>
                                ),
                            },
                            {
                                title: () => <TableHeaderCell key="status" sortKey="status" label="Trạng thái" />,
                                width: 400,
                                key: 'status',
                                render: ({ ...props }: Category) => {
                                    return (
                                        <Tag
                                            className={clsx(`text-sm whitespace-normal`)}
                                            color={typeof props.status === 'string' && props.status === 'Active' ? 'geekblue' : 'volcano'}
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
                                width: 400,
                                key: 'action',
                                render: ({ ...props }) => {
                                    return (
                                        <Dropdown
                                            overlay={
                                                <Menu>
                                                    <Menu.Item key="1">
                                                        <Button
                                                            type="primary"
                                                            style={{
                                                                width: '100px',
                                                            }}
                                                            onClick={() => {
                                                                setOpenUpdateModalState(true);
                                                                setDefaultCategoryState(props);
                                                            }}
                                                        >
                                                            Điều chỉnh
                                                        </Button>
                                                    </Menu.Item>
                                                    <Menu.Item key="2">
                                                        <Button
                                                            type="primary"
                                                            style={{
                                                                width: '100px',
                                                            }}
                                                            danger
                                                            onClick={() => handleDeleteCategory(props.id)}
                                                        >
                                                            Xóa
                                                        </Button>
                                                    </Menu.Item>
                                                </Menu>
                                            }
                                            trigger={['click']}
                                        >
                                            <span className="cursor-pointer">Chỉnh Sửa</span>
                                        </Dropdown>
                                    );
                                },
                            },
                        ]}
                    />
                </div>
            </Descriptions>
            <CategoryDetailModal
                categoryId={categoryDetailId}
                footer={null}
                width={1000}
                open={openDetailModal}
                onCancel={() => setOpenDetailModalState(false)}
            />

            <UpdateCategoryModal
                category={defaultCategory}
                width={1000}
                open={openUpdateModal}
                afterClose={() => setOpenUpdateModalState(false)}
                onCancel={() => setOpenUpdateModalState(false)}
            />

            <CreateCategoryModal
                open={openCreateModalState}
                afterClose={() => setOpenCreateModalState(false)}
                onCancel={() => setOpenCreateModalState(false)}
            />
        </div>
    );
};

export default CategoryList;
