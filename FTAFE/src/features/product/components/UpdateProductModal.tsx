import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { CategoryAPI } from '@core/api/category.api';
import { productAPI } from '@core/api/product.api';
import { Category } from '@models/category';
import { Product, UpdateProduct } from '@models/product';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateProductModalProps extends ModalProps {
    currentValue: Product;
}

const defaultValues = {
    id: '',
    categoryId: '',
    name: '',
    description: '',
    code: '',
    status: '',
    label: '',
    createdAt: '',
    updatedAt: '',
};

const UpdateProductModal: React.FC<UpdateProductModalProps> = ({ currentValue, ...rest }) => {
    const updateProductMutation = useMutation(async (data: UpdateProduct) => await productAPI.updateProduct(currentValue.id, data));

    const methods = useForm({
        defaultValues,
    });

    React.useEffect(() => {
        methods.setValue('id', currentValue.id);
        methods.setValue('code', currentValue.code);
        methods.setValue('name', currentValue.name);
        methods.setValue('description', currentValue.description);
        methods.setValue('status', currentValue.status);
        methods.setValue('label', currentValue.label);
        methods.setValue('categoryId', currentValue.categoryId);
    }, [currentValue]);

    const queryClient = useQueryClient();

    const onSubmit = (data: UpdateProduct) => {
        updateProductMutation.mutate(data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('Update product successfully');
                queryClient.invalidateQueries();
            },
        });
    };

    const getAllCategoriesQuery = useQuery({
        queryFn: async () => await CategoryAPI.getAllCategories(),
        queryKey: ['categories'],
    });
    const categoryList = getAllCategoriesQuery.data?.payload || [];

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Cập nhật sản phẩm"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={updateProductMutation.isLoading} onClick={rest.onCancel}>
                        Hủy
                    </Button>,
                    <Button key="edit" type="primary" loading={updateProductMutation.isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="name" label="Tên sản phẩm" required />
                    <TextInput name="description" label="Mô tả" required />
                    <TextInput name="code" label="Mã" required />
                    <TextInput name="label" label="Nhãn" required />
                    <SelectInput
                        label="Danh mục"
                        name="categoryId"
                        options={categoryList.map((c: Category) => {
                            return {
                                value: c.id,
                                label: c.name,
                                origin: c.id,
                            };
                        })}
                    />
                    <SelectInput
                        label="Trạng thái"
                        name="status"
                        options={[
                            {
                                label: 'Hoạt động',
                                value: 'Active',
                                origin: 'active',
                            },
                            {
                                label: 'Không hoạt động',
                                value: 'Inactive',
                                origin: 'inactive',
                            },
                        ]}
                    />{' '}
                </form>
            </Modal>
        </FormWrapper>
    );
};
export default UpdateProductModal;
