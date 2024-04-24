export interface Product {
    id: string;
    categoryId: string;
    code: string;
    name: string;
    description: string;
    label: string;
    createdAt: string;
    updatedAt: string;
    status: string;
}
export type CreateProduct = Pick<Product, 'categoryId' | 'name' | 'code' | 'description' | 'label'>;

export type UpdateProduct = Pick<Product, 'categoryId' | 'name' | 'code' | 'description' | 'label' | 'status' | 'id'>;

export type ProductItemSelling = {
    productItemId: string;
    title: string;
    salePrice: number;
    quantity: number;
    sold: number;
    status: string;
    soldPercent: number;
};
