import { FormWrapper, SelectInput } from '@components/forms';
import { BusinessDayAPI } from '@core/api/business-day.api';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { useQueryGetAllMenus } from '@hooks/api/farmhub.hook';
import { Menu } from '@models/menu';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateMenuInBusinessDayModalProps extends ModalProps {
    businessDayId: string;
    closeMenuModal: () => void;
}

const defaultValues = {
    menuId: '',
};

const CreateMenuInBusinessDayModal: React.FC<CreateMenuInBusinessDayModalProps> = ({ businessDayId, closeMenuModal, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const { data, isSuccess } = useQueryGetAllMenus();

    const [menus, setMenus] = React.useState<Menu[]>([]);
    const queryClient = useQueryClient();

    React.useEffect(() => {
        if (isSuccess) {
            setMenus(data.payload);
        }
    }, [isSuccess]);

    const CreateMenuInBusinessDay = useMutation(async (data: string) => await BusinessDayAPI.createMenuInBusinessDay(businessDayId, data), {
        onSuccess: () => {
            methods.reset();
            toast.success('Thêm thành công');
            queryClient.invalidateQueries();
            rest.afterClose && rest.afterClose();
            closeMenuModal();
        },
        onError: () => {},
    });

    const onSubmit = async (data: any) => {
        if (data.menuId === '') {
            methods.setError('menuId', { type: 'manual', message: 'Vui lòng chọn menu' });
            return;
        }
        CreateMenuInBusinessDay.mutate(data.menuId);
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo danh sách bán"
                width={600}
                cancelButtonProps={{ onClick: closeMenuModal }}
                closeIcon={<XMarkIcon className="absolute w-6 h-6 cursor-pointer top-4" onClick={closeMenuModal} />}
                footer={[
                    <Button key="back" onClick={closeMenuModal}>
                        Hủy
                    </Button>,
                    <Button key="submit" type="primary" loading={CreateMenuInBusinessDay.isLoading} onClick={methods.handleSubmit(onSubmit)}>
                        Tạo
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)}>
                    <SelectInput
                        label="Chọn danh sách bán cho ngày đăng bán"
                        name="menuId"
                        options={menus.map((menu) => ({
                            label: menu.name,
                            value: menu.id,
                            origin: menu,
                        }))}
                        placeholder="Chọn menu"
                        defaultValue={menus[0]?.id}
                        required
                    />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateMenuInBusinessDayModal;
