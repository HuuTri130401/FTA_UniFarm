import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { AvatarUploadInput } from '@components/forms/AvatarUploadInput';
import { NumberInput } from '@components/forms/NumberInput';
import { CategoryAPI } from '@core/api/category.api';
import { Category, UpdateCategory } from '@models/category';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateCategoryProps extends ModalProps {
    category: Category;
}

const defaultValues = {
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
};

const UpdateCategoryModal: React.FC<UpdateCategoryProps> = ({ category, ...rest }) => {
    const updateCategoryMutation = useMutation({
        mutationFn: async (data: UpdateCategory) => {
            const res = await CategoryAPI.updateCategory(category.id, data);
            return res;
        },
    });

    const methods = useForm({
        defaultValues,
    });

    React.useEffect(() => {
        methods.setValue('code', category.code);
        methods.setValue('id', category.id);
        methods.setValue('name', category.name);
        methods.setValue('description', category.description);
        methods.setValue('image', category.image);
        methods.setValue('status', category.status);
        methods.setValue('systemPrice', category.systemPrice);
        methods.setValue('minSystemPrice', category.minSystemPrice);
        methods.setValue('maxSystemPrice', category.maxSystemPrice);
        methods.setValue('margin', category.margin);
    }, [category]);

    const queryClient = useQueryClient();

    const onSubmit = (data: UpdateCategory) => {
        updateCategoryMutation.mutate(data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('Update category successfully');
                queryClient.invalidateQueries(['categories']);
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Chỉnh sửa loại sản phẩm"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={updateCategoryMutation.isLoading} onClick={rest.onCancel}>
                        Hủy
                    </Button>,
                    <Button key="edit" type="primary" loading={updateCategoryMutation.isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <AvatarUploadInput name="image" label="Ảnh loại sản phẩm" className="col-span-full" />
                    <TextInput name="name" label="Tên loại sản phẩm" required />
                    <TextInput name="description" label="Mô tả sản phẩm" placeholder="Mô tả ..." required />
                    <TextInput name="code" label="Mã" required />
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
                    />
                    <NumberInput name="displayIndex" label="Thứ tự hiển thị" required />
                    <NumberInput name="systemPrice" label="Giá hệ thống(VNĐ)" required />
                    <NumberInput name="minSystemPrice" label="Giá thấp nhất (VNĐ)" required />
                    <NumberInput name="maxSystemPrice" label="Giá cao nhất (VNĐ)" required />
                </form>
            </Modal>
        </FormWrapper>
    );
};
export default UpdateCategoryModal;
