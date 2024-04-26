import { Menu } from './menu';

export interface ProductItem {
    id: string;
    productId: string;
    farmHubId: string;
    title: string;
    description: string;
    productOrigin: string;
    specialTag: string;
    storageType: string;
    outOfStock: boolean;
    createdAt: string;
    updatedAt: string;
    price: number;
    quantity: number;
    minOrder: number;
    status: string;
    unit: string;
    productImages: productImage[];
    sold: number;
    productName: string;
}

export interface productImage {
    id: string;
    productItemId: string;
    caption: string;
    imageUrl: string;
    displayIndex: number;
    status: string;
}

export interface CreateProductItem {
    SpecialTag: string;
    Unit: string;
    Price: number;
    Quantity: number;
    StorageType: string;
    ProductOrigin: string;
    Title: string;
    MinOrder: number;
    Description: string;
    Images: any[];
}
export interface CreateProductItemInMenu {
    productItemId: string;
    salePrice: number;
    quantity: number;
}

export interface ProductItemInMenu {
    id: string;
    menuId: string;
    menu: Menu;
    price: number;
    productItem: ProductItem;
    productItemId: string;
    quantity: number;
    salePrice: number;
    sold: number;
    status: string;
}
