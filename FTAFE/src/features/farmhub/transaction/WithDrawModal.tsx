import { FormWrapper, TextInput, TextareaField } from '@components/forms';
import { PaymentAPI } from '@core/api/payment.api';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { CreatePayment } from '@models/payment';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface WidthDrawModalProps extends ModalProps {
    closeModal: () => void;
}

const defaultValues = {
    bankName: '',
    bankOwnerName: '',
    bankAccountNumber: '',
    amount: 1000,
    note: '',
};

const WithDrawModal: React.FunctionComponent<WidthDrawModalProps> = ({ closeModal, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();

    const createPaymentMutation = useMutation(async (data: CreatePayment) => await PaymentAPI.createPayment(data), {
        onSuccess: (res) => {
            methods.reset();
            toast.success('Tạo thành công.');
            queryClient.invalidateQueries();
            closeModal();
        },
        onError: (error) => {
            console.log('error', error);
        },
    });

    const onSubmit = (data: CreatePayment) => {
        if (!data.bankName) {
            methods.setError('bankName', { type: 'manual', message: 'Tên ngân hàng không được để trống' });
            return;
        }
        if (!data.bankOwnerName) {
            methods.setError('bankOwnerName', { type: 'manual', message: 'Tên chủ tài khoản không được để trống' });
            return;
        }
        if (!data.bankAccountNumber) {
            methods.setError('bankAccountNumber', { type: 'manual', message: 'Số tài khoản không được để trống' });
            return;
        }
        if (!data.amount) {
            methods.setError('amount', { type: 'manual', message: 'Số tiền không được để trống' });
            return;
        }
        createPaymentMutation.mutate(data);
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title={
                    <h3>
                        <span className="text-xl font-semibold">Rút tiền</span>
                    </h3>
                }
                width={600}
                cancelButtonProps={{ onClick: closeModal }}
                closeIcon={
                    <XMarkIcon
                        className="absolute w-6 h-6 cursor-pointer top-4"
                        onClick={() => {
                            methods.clearErrors();
                            closeModal();
                        }}
                    />
                }
                footer={[
                    <Button
                        key="close"
                        type="default"
                        className="my-2"
                        onClick={
                            //clear error before close modal
                            () => {
                                methods.clearErrors();
                                closeModal();
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
                    <TextInput label="Tên ngân hàng" name="bankName" placeholder="VD: Vietcombank, Techcombank, Agribank, ..." />
                    <TextInput label="Tên chủ tài khoản" name="bankOwnerName" placeholder="VD: Nguyễn Văn A, Trần Thị B, ..." />
                    <TextInput label="Số tài khoản" name="bankAccountNumber" placeholder="VD: 1234567890" />
                    <TextInput label="Số tiền" name="amount" placeholder="VD: Số tiền" type="number" min="1000" step="1000" />
                    <TextareaField label="Ghi chú" name="note" placeholder="VD: Rút tiền từ ví Farmhub" required={false} rows={3} />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default WithDrawModal;
