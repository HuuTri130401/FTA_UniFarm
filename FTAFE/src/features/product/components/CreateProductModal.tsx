import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { CategoryAPI } from '@core/api/category.api';
import { productAPI } from '@core/api/product.api';
import { Category } from '@models/category';
import { CreateProduct } from '@models/product';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateProductModalProps extends ModalProps {}
const defaultValues = {
    code: '',
    name: '',
    description: '',
    label: '',
    categoryId: '',
};

const CreateProductModal: React.FC<CreateProductModalProps> = ({ ...rest }) => {
    const methods = useForm({
        defaultValues,
    });

    const queryClient = useQueryClient();

    const createProductMutation = useMutation(async (data: CreateProduct) => await productAPI.createProduct(data), {
        onSuccess: (res) => {
            methods.reset();
            toast.success('Tạo thành công.');
            queryClient.invalidateQueries();
            rest.afterClose && rest.afterClose();
        },
        onError: (error) => {
            toast.error('created fail');
        },
    });

    const getAllCategoriesQuery = useQuery({
        queryFn: async () => await CategoryAPI.getAllCategories(),
        queryKey: ['categories'],
    });
    const categoryList = getAllCategoriesQuery.data?.payload || [];

    const onSubmit = async (data: CreateProduct) => createProductMutation.mutate(data);

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo sản phẩm"
                width={600}
                footer={[
                    <Button key="close" type="default" onClick={rest.onCancel}>
                        Trở lại
                    </Button>,
                    <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                        Tạo
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="name" label=" Tên sản phẩm" placeholder="Tên sản phẩm ..." required />
                    <TextInput name="description" label="Mô tả sản phẩm" placeholder="Mô tả ..." required />
                    <TextInput name="code" label="Mã" required />
                    <TextInput name="label" label="Nhãn" required />
                    <SelectInput
                        label="Loại sản phẩm"
                        name="categoryId"
                        options={categoryList.map((c: Category) => {
                            return {
                                value: c.id,
                                label: c.name,
                                origin: c.id,
                            };
                        })}
                    />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateProductModal;
