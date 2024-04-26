import { PaymentAPI, PaymentRequestStatus, UpdatePaymentRequestForm } from '@core/api/payment.api';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Divider, Modal, ModalProps } from 'antd';
import { toast } from 'react-toastify';

export interface BankInfo {
    bankName: string;
    bankOwnerName: string;
    bankAccountNumber: string;
    transferAmount: number;
    code: string;
    note: string;
    paymentId: string;
    paymentStatus: string;
}

interface BankModalProps extends ModalProps {
    bankInfo: BankInfo;
}

const BankModal: React.FunctionComponent<BankModalProps> = ({ bankInfo, ...props }) => {
    const user = useStoreUser();
    const queryClient = useQueryClient();
    const updateStatusMutation = useMutation(async (data: UpdatePaymentRequestForm) => await PaymentAPI.updateStatusPaymentRequest(data), {
        onSuccess: () => {
            queryClient.invalidateQueries(['payments']);
            toast.success('Update thành công');
            props.afterClose && props.afterClose();
        },
        onError: (error) => {
            console.log('error', error);
        },
    });
    const handleUpdate = (data: UpdatePaymentRequestForm) => {
        Modal.confirm({
            title: 'Bạn có muốn chỉnh sửa status',
            content: 'bạn sẽ không thể khôi phục dữ liệu sau chỉnh sửa',
            okText: 'Tiếp tục',
            okType: 'danger',
            cancelText: 'Từ chối',
            onOk: async () => {
                try {
                    await updateStatusMutation.mutateAsync(data);
                } catch (error) {
                    console.error('Error update :', error);
                }
            },
        });
    };
    return (
        <Modal
            {...props}
            title={<div className="text-2xl font-semibold">Thông tin tài khoản ngân hàng</div>}
            width={600}
            cancelButtonProps={{ onClick: props.onCancel }}
            footer={[
                user.roleName === UserRole.ADMIN && bankInfo.paymentStatus === 'PENDING' && (
                    <Button
                        key="approve"
                        type="primary"
                        onClick={() =>
                            handleUpdate({
                                id: bankInfo.paymentId,
                                status: PaymentRequestStatus.SUCCESS,
                            })
                        }
                        className="my-2"
                    >
                        Duyệt
                    </Button>
                ),
                user.roleName === UserRole.ADMIN && bankInfo.paymentStatus === 'PENDING' && (
                    <Button
                        key="reject"
                        onClick={() =>
                            handleUpdate({
                                id: bankInfo.paymentId,
                                status: PaymentRequestStatus.DENIED,
                            })
                        }
                        type="primary"
                        danger
                        className="my-2"
                    >
                        Từ chối
                    </Button>
                ),
                <Button key="close" type="default" onClick={props.onCancel} className="my-2">
                    Trở lại
                </Button>,
            ]}
        >
            <div className="flex flex-col gap-4">
                <div className="flex justify-between gap-2">
                    <span className="text-xl font-semibold">Ngân hàng:</span>
                    <span className="text-lg font-medium">{bankInfo.bankName}</span>
                </div>
                <Divider className="!my-1" dashed />
                <div className="flex justify-between gap-2">
                    <span className="text-xl font-semibold">Chủ tài khoản:</span>
                    <span className="text-lg font-medium">{bankInfo.bankOwnerName}</span>
                </div>
                <Divider className="!my-1" dashed />
                <div className="flex justify-between gap-2">
                    <span className="text-xl font-semibold">Số tài khoản:</span>
                    <span className="text-lg font-medium">{bankInfo.bankAccountNumber}</span>
                </div>
                <Divider className="!my-1" dashed />
                <div className="flex justify-between gap-2">
                    <span className="text-xl font-semibold">Số tiền:</span>
                    <span className="text-lg font-medium">{bankInfo.transferAmount ? bankInfo.transferAmount.toLocaleString('en') : 0} VNĐ</span>
                </div>
                <Divider className="!my-1" dashed />
                <div className="flex justify-between gap-2">
                    <span className="text-xl font-semibold">Mã giao dịch:</span>
                    <span className="text-lg font-medium">{bankInfo.code ? bankInfo.code : 'Chưa có mã giao dịch'}</span>
                </div>
                <Divider className="!my-1" dashed />
                <div className="flex justify-between gap-2">
                    <span className="text-xl font-semibold">Ghi chú:</span>
                    <span className="text-lg font-medium">{bankInfo.note}</span>
                </div>
            </div>
        </Modal>
    );
};

export default BankModal;
