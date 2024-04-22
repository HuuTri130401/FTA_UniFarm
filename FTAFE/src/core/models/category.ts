export type CreateCategory = Omit<Category, 'id' | 'createdAt' | 'updatedAt'>;
export type UpdateCategory = Omit<Category, 'id'>;
export type Category = {
    id: string;
    name: string;
    description: string;
    image: string;
    code: string;
    status: string;
    displayIndex: number;
    systemPrice: number;
    minSystemPrice: number;
    maxSystemPrice: number;
    margin: number;
    createdAt: string;
    updatedAt: string;
};
