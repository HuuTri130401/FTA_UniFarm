import { EyeOutlined } from '@ant-design/icons';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { CategoryAPI } from '@core/api/category.api';
import { productAPI } from '@core/api/product.api';
import { routes } from '@core/routes';
import { Category } from '@models/category';
import { Product } from '@models/product';
import { useQuery } from '@tanstack/react-query';
import { Descriptions, Image, Modal, ModalProps } from 'antd';
import { useRouter } from 'next/router';

//Category Detail Modal
interface CategoryDetail extends ModalProps {
    categoryId: string;
}
const CategoryDetailModal: React.FunctionComponent<CategoryDetail> = ({ categoryId, ...rest }) => {
    const { data } = useQuery<Category>({
        queryKey: ['category', categoryId],
        queryFn: async () => {
            const res = await CategoryAPI.getCategoryById(categoryId);

            return res?.payload;
        },
        enabled: rest.open,
    });

    const getProductByCategoryQuery = useQuery({
        queryKey: ['products', categoryId],
        queryFn: async () => {
            const res = await productAPI.getProductsByCategoryId(categoryId);
            return res;
        },
        enabled: rest.open,
    });

    const productList: Product[] = getProductByCategoryQuery.data?.payload;
    const router = useRouter();
    return (
        <Modal {...rest} width={1400}>
            <Descriptions
                title="Mô tả loại sản phẩm"
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                className="p-4 bg-white rounded-lg"
            >
                <Descriptions.Item label="ảnh" span={1}>
                    <Image height={80} width={80} className="rounded overflow-hidden" alt={data?.name} src={data?.image} />
                </Descriptions.Item>
                <Descriptions.Item label="Tên loại sản phẩm" span={2}>
                    {data?.name}
                </Descriptions.Item>
                <Descriptions.Item label="Mã" span={1}>
                    {data?.code}
                </Descriptions.Item>
                <Descriptions.Item label="Mô tả" span={2}>
                    {data?.description}
                </Descriptions.Item>
                <Descriptions.Item label="Trạng thái">{data?.status}</Descriptions.Item>
                <Descriptions.Item label="Ngày tạo">{data?.createdAt}</Descriptions.Item>
                <Descriptions.Item label="Cập nhật lần cuối">{data?.updatedAt}</Descriptions.Item>
                <Descriptions.Item label="Giá hệ thống">{data?.systemPrice}</Descriptions.Item>
                <Descriptions.Item label="Giá hệ thống cao nhất">{data?.maxSystemPrice}</Descriptions.Item>
                <Descriptions.Item label="Giá hệ thống thấp nhất">{data?.minSystemPrice}</Descriptions.Item>
            </Descriptions>
            <Descriptions
                title="List Product"
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                className="p-4 bg-white rounded-lg"
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Product>
                        rowKey="id"
                        isLoading={getProductByCategoryQuery.isLoading}
                        data={productList}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="name" label="Tên sản phẩm" />,
                                width: 400,
                                key: 'name',
                                render: ({ ...props }: Product) => <span>{props.name}</span>,
                            },
                            {
                                title: () => <TableHeaderCell key="description" label="Mô tả" />,
                                width: 400,
                                key: 'description',
                                render: ({ ...props }: Product) => <span>{props.description}</span>,
                            },
                            {
                                title: () => <TableHeaderCell key="label" label="Nhãn" />,
                                width: 400,
                                key: 'label',
                                render: ({ ...props }: Product) => <span>{props.label}</span>,
                            },
                            {
                                title: () => <TableHeaderCell key="" sortKey="" label="" />,
                                width: 400,
                                key: 'action',
                                render: ({ ...props }: Product) => (
                                    <EyeOutlined
                                        className="w-10 h-10 mt-4 text-blue-500 cursor-pointer"
                                        style={{ fontSize: '1.5rem' }}
                                        onClick={() => {
                                            router.push(routes.admin.product.detail(props.id));
                                        }}
                                    />
                                ),
                            },
                        ]}
                    />
                </div>
            </Descriptions>
        </Modal>
    );
};
export default CategoryDetailModal;
