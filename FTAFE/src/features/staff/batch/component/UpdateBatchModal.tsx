import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import CustomImageUpload from '@components/forms/CustomImageInput';
import { BatchAPI, BatchConfirmFormData, BatchConfirmStatusEnum } from '@core/api/batch.api';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

//Update Category Modal
interface UpdateBatchModalProps extends ModalProps {
    id: string;
    bDayId: string;
}
const defaultValues = {
    Status: BatchConfirmStatusEnum.RECEIVED,
    ReceivedDescription: '',
    FeedBackImage: null,
};
const UpdateBatchModal: React.FC<UpdateBatchModalProps> = ({ id, bDayId, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });

    const updateMutation = useMutation(async (data: BatchConfirmFormData) => await BatchAPI.confirm(id, data));

    const queryClient = useQueryClient();

    const onSubmit = (data: any) => {
        methods.reset();
        updateMutation.mutate(data, {
            onSuccess: () => {
                rest.afterClose && rest.afterClose();
                toast.success('Update batch successfully');
                queryClient.invalidateQueries(['batch', `dayId-${bDayId}`]);
            },
        });
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Cập nhật trạng thái lô hàng"
                width={600}
                footer={[
                    <Button key="close" type="default" loading={updateMutation.isLoading} onClick={rest.onCancel}>
                        Huỷ
                    </Button>,
                    <Button key="edit" type="primary" loading={updateMutation.isLoading} onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <TextInput name="ReceivedDescription" label="Ghi chú lô hàng" />
                    <CustomImageUpload name="FeedBackImage" label="Ảnh nhận hàng" />
                    <SelectInput
                        required
                        label="Trạng thái"
                        name="Status"
                        options={
                            //     Object.keys(BatchConfirmStatusEnum).map((key) => {
                            //     return {
                            //         label: BatchConfirmStatusEnum[key as keyof typeof BatchConfirmStatusEnum],
                            //         value: BatchConfirmStatusEnum[key as keyof typeof BatchConfirmStatusEnum],
                            //         origin: BatchConfirmStatusEnum[key as keyof typeof BatchConfirmStatusEnum],
                            //     };
                            // })
                            [
                                {
                                    label: BatchConfirmStatusEnum.RECEIVED,
                                    value: BatchConfirmStatusEnum.RECEIVED,
                                    origin: BatchConfirmStatusEnum.RECEIVED,
                                },
                                {
                                    label: BatchConfirmStatusEnum.NOT_RECEIVED,
                                    value: BatchConfirmStatusEnum.NOT_RECEIVED,
                                    origin: BatchConfirmStatusEnum.NOT_RECEIVED,
                                },
                            ]
                        }
                    />
                </form>
            </Modal>
        </FormWrapper>
    );
};
export default UpdateBatchModal;
