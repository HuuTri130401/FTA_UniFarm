import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { MultiImageUploadInput } from '@components/images/MultiImageUploadInput';
import { ProductItemAPI } from '@core/api/product-item.api';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { useQueryProducts } from '@hooks/api/product.hook';
import { Product } from '@models/product';
import { CreateProductItem } from '@models/product-item';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useRouter } from 'next/router';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateProductItemModalProps extends ModalProps {
    onClose: () => void;
}
const defaultValues = {
    Title: '',
    Description: '',
    ProductOrigin: '',
    SpecialTag: '',
    StorageType: '',
    Price: 1,
    Quantity: 1,
    MinOrder: 1,
    Unit: '',
    productId: '',
    Images: [],
};

const CreateProductItemModal: React.FC<CreateProductItemModalProps> = ({ onClose, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });

    const queryClient = useQueryClient();
    const router = useRouter();
    const id = methods.watch('productId');
    const { data, isSuccess } = useQueryProducts();

    const [product, setProduct] = React.useState<Product[]>([]);

    React.useEffect(() => {
        if (isSuccess) {
            setProduct(data);
        }
    }, [isSuccess]);

    const createProductItemMutation = useMutation(
        async (data: CreateProductItem) => {
            const formData = new FormData();
            Object.entries(data).forEach(([key, value]) => {
                if (key === 'Images' && value instanceof Array) {
                    value.forEach((file: File) => formData.append('Images', file));
                } else {
                    formData.append(key, value.toString());
                }
            });
            return await ProductItemAPI.createProductItem(formData, id as string);
        },
        {
            onSuccess: (res) => {
                methods.reset();
                methods.setValue('productId', product[0]?.id);
                methods.setValue('Unit', 'kg');
                toast.success('Thêm thành công');
                queryClient.invalidateQueries();
                onClose();
            },
            onError: (error) => {
                toast.error('Thêm thất bại');
            },
        }
    );

    const onSubmit = async (data: CreateProductItem) => {
        createProductItemMutation.mutate(data);
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo sản phẩm mới"
                width={600}
                closeIcon={
                    <XMarkIcon
                        className="absolute w-6 h-6 cursor-pointer top-4"
                        onClick={() => {
                            onClose();
                        }}
                    />
                }
                footer={[
                    <Button
                        key="reset"
                        type="default"
                        onClick={() => {
                            methods.reset;
                        }}
                    >
                        Làm mới
                    </Button>,
                    <Button key="close" type="default" onClick={onClose}>
                        Trở lại
                    </Button>,
                    <Button key="create" type="primary" onClick={methods.handleSubmit(onSubmit)}>
                        Tạo
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <SelectInput
                        label="Sản phẩm"
                        name="productId"
                        defaultValue={product[0]?.id}
                        options={product.map((item) => ({
                            label: item.name,
                            value: item.id,
                            origin: item,
                        }))}
                        required
                    />
                    <TextInput name="Title" label="Tên sản phẩm chi tiết" placeholder="Tên sản phẩm ..." required />
                    <TextInput name="Description" label="Mô tả sản phẩm" placeholder="Mô tả ..." required />
                    <TextInput name="ProductOrigin" label="Nơi sản xuất" placeholder="Nơi sản xuất ..." required />
                    <TextInput name="SpecialTag" label="Tag đặc biệt" required />
                    <TextInput name="StorageType" label="Lưu trữ" required />
                    <TextInput name="Price" label="Giá nhập hàng" required type="number" min={1} step={0.1} />
                    <TextInput name="Quantity" label="Số lượng nhập" required type="number" min={1} />
                    <TextInput name="MinOrder" label="Đơn hàng tối thiểu" required type="number" min={0} />
                    <SelectInput
                        label="Đơn vị"
                        name="Unit"
                        options={[
                            { value: 'kg', label: 'Kg', origin: 'kg' },
                            { value: 'g', label: 'G', origin: 'G' },
                            { value: 'l', label: 'L', origin: 'L' },
                            { value: 'ml', label: 'Ml', origin: 'Ml' },
                            { value: 'cái', label: 'Cái', origin: 'Cái' },
                            { value: 'hộp', label: 'Hộp', origin: 'Hộp' },
                            { value: 'chai', label: 'Chai', origin: 'Chai' },
                            { value: 'Vỉ', label: 'Vỉ', origin: 'Vỉ' },
                        ]}
                    />
                    {/* <CustomImageUpload name="Images" label="Ảnh" /> */}
                    <MultiImageUploadInput name="Images" label="Ảnh" />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateProductItemModal;
