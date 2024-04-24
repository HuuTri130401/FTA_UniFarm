import { FormWrapper, TextInput } from '@components/forms';
import { MenuAPI } from '@core/api/menu.api';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { CreateMenu } from '@models/farmhub-menu';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateMenuModalProps extends ModalProps {
    closeMenuModal: () => void;
}

const defaultValues = {
    name: '',
    tag: '',
};

const CreateMenuModal: React.FunctionComponent<CreateMenuModalProps> = ({ closeMenuModal, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();

    const createMenuMutation = useMutation(async (data: CreateMenu) => await MenuAPI.createMenu(data), {
        onSuccess: (res) => {
            methods.reset();
            toast.success('Tạo thành công.');
            queryClient.invalidateQueries();
            rest.afterClose && rest.afterClose();
            closeMenuModal();
        },
        onError: (error) => {
            console.log('error', error);
        },
    });

    const onSubmit = (data: CreateMenu) => {
        if (!data.name) {
            methods.setError('name', { type: 'manual', message: 'Tên menu không được để trống' });
            return;
        }
        if (!data.tag) {
            methods.setError('tag', { type: 'manual', message: 'Tag không được để trống' });
            return;
        }
        createMenuMutation.mutate(data);
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo danh sách bán"
                width={600}
                cancelButtonProps={{ onClick: closeMenuModal }}
                closeIcon={
                    <XMarkIcon
                        className="absolute w-6 h-6 cursor-pointer top-4"
                        onClick={() => {
                            methods.clearErrors();
                            closeMenuModal();
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
                                closeMenuModal();
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
                    <TextInput name="name" label="Tên menu" placeholder="Tên menu ..." required />
                    <TextInput name="tag" label="Tag" placeholder="Tag ..." required />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateMenuModal;
