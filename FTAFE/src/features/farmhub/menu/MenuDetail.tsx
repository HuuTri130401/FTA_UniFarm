import { WarningOutlined } from '@ant-design/icons';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useDeleteProductItemInMenuMutation, useQueryGetProductItemByMenuId } from '@hooks/api/farmhub.hook';
import { useDebounce } from '@hooks/useDebounce';
import { Menu } from '@models/menu';
import { ProductItemInMenu } from '@models/product-item';
import { useQueryClient } from '@tanstack/react-query';
import { Badge, Button, Descriptions, Image, Input, Modal } from 'antd';
import { PlusIcon } from 'lucide-react';
import moment from 'moment';
import React from 'react';
import { toast } from 'react-toastify';
import CreateProductItemModal from '../product/components/CreateProductItemModal';
import CreateProductItemInMenuModal from './CreateProductItemInMenuModal';

const { Search } = Input;

interface MenuDetailProps {
    menu?: Menu;
}

const MenuDetail: React.FunctionComponent<MenuDetailProps> = ({ menu }: MenuDetailProps) => {
    const [openCreateModalState, setOpenCreateModalState] = React.useState<boolean>(false);
    const [openCreateProductItemModal, setOpenCreateProductItemModal] = React.useState<boolean>(false);
    const { data: productItemByMenuId, isLoading } = useQueryGetProductItemByMenuId(menu?.id as string);
    const { mutationDeleteProductItemInMenu, isLoading: isDeleting } = useDeleteProductItemInMenuMutation();
    const queryClient = useQueryClient();

    //Search bar
    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });

    const handleDeleteProductItemInMenu = (id: string) => {
        Modal.confirm({
            title: 'Bạn có muốn xoá?',
            content: 'Hành động này sẽ không thể hoàn tác lại được!',
            okText: 'Xoá!',
            okType: 'danger',
            icon: <WarningOutlined style={{ color: 'red' }} />,
            cancelText: 'Trở lại',
            onOk: async () => {
                try {
                    mutationDeleteProductItemInMenu(id, {
                        onSuccess: () => {
                            queryClient.invalidateQueries();
                            toast.success('Xoá thành công!');
                        },
                    });
                } catch (error) {
                    console.error('Error deleting FarmHub:', error);
                }
            },
        });
    };

    return (
        <>
            <div className="flex flex-col w-full gap-4">
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={'Thông tin liên quan đến sản phẩm'}
                    className="p-4 bg-white rounded-lg"
                    extra={
                        <div className="flex gap-5 items-center">
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
                                    setOpenCreateProductItemModal(!openCreateProductItemModal);
                                }}
                                className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                            >
                                <PlusIcon className="w-5 h-5 text-white" />
                                <span>
                                    <strong>Thêm Sản Phẩm Chi Tiết</strong>
                                </span>
                            </button>
                            <button
                                onClick={() => {
                                    setOpenCreateModalState(!openCreateModalState);
                                }}
                                className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                            >
                                <PlusIcon className="w-5 h-5 text-white" />
                                <span>
                                    <strong>Thêm Sản Phẩm Vào Menu</strong>
                                </span>
                            </button>
                        </div>
                    }
                >
                    <Descriptions.Item label="Menu" span={3}>
                        {menu?.name}
                    </Descriptions.Item>
                    <Descriptions.Item label="Mã Tag" span={1}>
                        {menu?.tag}
                    </Descriptions.Item>

                    <Descriptions.Item label="Trạng thái" span={1}>
                        <Badge status={menu?.status === 'Active' ? 'success' : 'error'} text={menu?.status} />
                    </Descriptions.Item>
                    <Descriptions.Item label="Ngày tạo" span={1}>
                        {moment(menu?.createdAt).format('DD/MM/YYYY')}
                    </Descriptions.Item>
                </Descriptions>
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={`Các sản phẩm trong ${menu?.name}`}
                    className="p-4 bg-white rounded-lg"
                >
                    <div className="flex flex-col w-full gap-2">
                        <TableBuilder<ProductItemInMenu>
                            rowKey="id"
                            isLoading={isLoading}
                            data={productItemByMenuId as ProductItemInMenu[]}
                            columns={[
                                {
                                    title: () => <></>,
                                    width: 180,
                                    render: ({ ...props }: ProductItemInMenu) => (
                                        <Image
                                            src={props.productItem.productImages.length > 0 ? props.productItem.productImages[0].imageUrl : ''}
                                            width={150}
                                            alt={props.productItem.productImages.length > 0 ? props.productItem.productImages[0].caption : ''}
                                            fallback="https://www.eclosio.ong/wp-content/uploads/2018/08/default.png"
                                        />
                                    ),
                                    fixed: 'left',
                                },
                                {
                                    title: () => <TableHeaderCell key="title" sortKey="title" label="Tên" />,
                                    width: 200,
                                    key: 'title',
                                    render: ({ ...props }: ProductItemInMenu) => <p className="m-0">{props.productItem.title}</p>,
                                    fixed: 'left',
                                },
                                {
                                    title: () => <TableHeaderCell key="productOrigin" sortKey="productOrigin" label="Nơi sản xuất" />,
                                    width: 200,
                                    key: 'productOrigin',
                                    render: ({ ...props }: ProductItemInMenu) => <p className="m-0">{props.productItem.productOrigin}</p>,
                                },
                                {
                                    title: () => <TableHeaderCell key="description" sortKey="description" label="Mô tả sản phẩm" />,
                                    width: 500,
                                    key: 'description',
                                    render: ({ ...props }: ProductItemInMenu) => <p className="m-0">{props.productItem.description}</p>,
                                },

                                {
                                    title: () => <TableHeaderCell key="price" sortKey="price" label="Giá tiền /VND" />,
                                    width: 200,
                                    key: 'price',
                                    render: ({ ...props }: ProductItemInMenu) => <span>{props.salePrice}</span>,
                                },
                                {
                                    title: () => <TableHeaderCell key="quantity" sortKey="quantity" label="Tổng số lượng" />,
                                    width: 150,
                                    key: 'quantity',
                                    render: ({ ...props }: ProductItemInMenu) => <span>{props.quantity}</span>,
                                },
                                {
                                    title: () => <TableHeaderCell key="unit" sortKey="unit" label="Đơn vị" />,
                                    width: 150,
                                    key: 'unit',
                                    render: ({ ...props }: ProductItemInMenu) => <span>{props.productItem.unit}</span>,
                                },
                                {
                                    title: () => <TableHeaderCell key="sold" sortKey="sold" label="Đã bán" />,
                                    width: 150,
                                    key: 'sold',
                                    render: ({ ...props }: ProductItemInMenu) => <span>{props.sold}</span>,
                                },
                                {
                                    title: () => <TableHeaderCell key="outOfStock" sortKey="outOfStock" label="Tình trạng" />,
                                    width: 200,
                                    key: 'outOfStock',
                                    render: ({ ...props }: ProductItemInMenu) => (
                                        <span>{props.productItem.outOfStock === false ? 'Còn hàng' : 'Hết hàng'}</span>
                                    ),
                                },
                                {
                                    title: () => <TableHeaderCell key="minOrder" sortKey="minOrder" label="Số lượng mua thấp nhất" />,
                                    width: 300,
                                    key: 'minOrder',
                                    render: ({ ...props }: ProductItemInMenu) => <span>{props.productItem.minOrder}</span>,
                                },
                                {
                                    title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                    width: 200,
                                    key: 'action',
                                    render: ({ ...props }: ProductItemInMenu) => {
                                        return (
                                            <Button type="primary" danger onClick={() => handleDeleteProductItemInMenu(props.id)}>
                                                Xoá
                                            </Button>
                                        );
                                    },
                                    fixed: 'right',
                                },
                            ]}
                        />
                    </div>
                </Descriptions>
            </div>
            <CreateProductItemInMenuModal
                menuId={menu?.id as string}
                open={openCreateModalState}
                closeProductItemInMenuModal={() => setOpenCreateModalState(false)}
            />
            <CreateProductItemModal open={openCreateProductItemModal} onClose={() => setOpenCreateProductItemModal(false)} />
        </>
    );
};

export default MenuDetail;
