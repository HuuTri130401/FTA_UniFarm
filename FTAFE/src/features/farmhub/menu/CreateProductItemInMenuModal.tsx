import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { MenuAPI } from '@core/api/menu.api';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { useQueryGetAllProductItems } from '@hooks/api/product.hook';
import { CreateProductItemInMenu, ProductItem } from '@models/product-item';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateProductItemInMenuModal extends ModalProps {
    menuId: string;
    closeProductItemInMenuModal: () => void;
}

const defaultValues = {
    productItemId: '',
    salePrice: 1,
    quantity: 1,
};

const CreateProductItemInMenuModal: React.FC<CreateProductItemInMenuModal> = ({ menuId, closeProductItemInMenuModal, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();
    const { data, isSuccess } = useQueryGetAllProductItems();

    const [productItems, setProductItems] = React.useState<ProductItem[]>([]);

    React.useEffect(() => {
        if (isSuccess) {
            setProductItems(data);
        }
    }, [isSuccess]);

    const CreateProductItemInMenuModal = useMutation(async (data: CreateProductItemInMenu) => await MenuAPI.createProductItemInMenu(data, menuId), {
        onSuccess: (res) => {
            methods.reset();
            toast.success('Thêm thành công');
            queryClient.invalidateQueries();
            rest.afterClose && rest.afterClose();
            closeProductItemInMenuModal();
        },
        onError: (error) => {},
    });

    const onSubmit = async (data: CreateProductItemInMenu) => {
        if (data.productItemId === '') {
            methods.setError('productItemId', { type: 'manual', message: 'Vui lòng chọn sản phẩm' });
            return;
        }
        if (data.salePrice <= 0) {
            methods.setError('salePrice', { type: 'manual', message: 'Giá bán phải lớn hơn 0' });
            return;
        }
        if (data.quantity <= 0) {
            methods.setError('quantity', { type: 'manual', message: 'Số lượng phải lớn hơn 0' });
            return;
        }

        CreateProductItemInMenuModal.mutate(data);
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Thêm sản phẩm vào menu"
                width={600}
                cancelButtonProps={{ onClick: closeProductItemInMenuModal }}
                closeIcon={
                    <XMarkIcon
                        className="absolute w-6 h-6 cursor-pointer top-4"
                        onClick={() => {
                            methods.clearErrors();
                            closeProductItemInMenuModal();
                        }}
                    />
                }
                footer={[
                    <Button
                        key="close"
                        type="default"
                        onClick={
                            //clear error before close modal
                            () => {
                                methods.clearErrors();
                                closeProductItemInMenuModal();
                            }
                        }
                    >
                        Trở lại
                    </Button>,
                    <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                        Tạo
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <SelectInput
                        label="Sản phẩm"
                        name="productItemId"
                        options={productItems.map((item) => ({
                            label: item.title,
                            value: item.id,
                            origin: item,
                        }))}
                        defaultValue={productItems[0]?.id}
                        placeholder="Chọn sản phẩm ..."
                        required
                    />
                    <TextInput name="quantity" label="Số lượng" placeholder="Số lượng ..." required type="number" />
                    <TextInput name="salePrice" label="Giá bán" placeholder="Giá bán ..." required type="number" />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateProductItemInMenuModal;
