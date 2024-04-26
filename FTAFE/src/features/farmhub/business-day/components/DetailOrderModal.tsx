import { XMarkIcon } from '@heroicons/react/24/outline';
import { OrderDetail } from '@models/batch';
import { Button, Descriptions, Modal, ModalProps } from 'antd';
import React from 'react';

interface DetailOrderModalProps extends ModalProps {
    orderDetail: OrderDetail[];
    closeModal: () => void;
    customerName: string;
    status: string;
    confirmButton: () => void;
    cancelButton: () => void;
}

const DetailOrderModal: React.FC<DetailOrderModalProps> = ({
    orderDetail,
    closeModal,
    customerName,
    status,
    confirmButton,
    cancelButton,
    ...rest
}) => {
    return (
        <Modal
            {...rest}
            title={<h3 className="text-xl font-semibold">Chi tiết đơn hàng của {customerName}</h3>}
            width={800}
            cancelButtonProps={{ onClick: closeModal }}
            centered
            closeIcon={
                <XMarkIcon
                    className="absolute w-6 h-6 cursor-pointer top-4"
                    onClick={() => {
                        closeModal();
                    }}
                />
            }
            footer={
                status === 'Pending'
                    ? [
                          <Button
                              key="close"
                              type="default"
                              onClick={() => {
                                  cancelButton();
                                  closeModal();
                              }}
                              className="my-5"
                          >
                              Hủy bỏ đơn hàng
                          </Button>,
                          <Button
                              key="create"
                              type="primary"
                              onClick={() => {
                                  confirmButton();
                                  closeModal();
                              }}
                              className="my-5"
                          >
                              Xác nhận đơn hàng
                          </Button>,
                      ]
                    : null
            }
        >
            <div>
                <div className="flex flex-col gap-10">
                    {orderDetail.map((order, index) => (
                        <div key={index}>
                            <Descriptions
                                title={<h4 className="text-lg font-semibold">Chi tiết đơn hàng {index + 1}</h4>}
                                layout="horizontal"
                                bordered
                            >
                                <Descriptions.Item label={<h5 className="text-base font-semibold">Mã đơn hàng</h5>} span={3}>
                                    {order.orderId}
                                </Descriptions.Item>
                                <Descriptions.Item label={<h5 className="text-base font-semibold">Tên sản phẩm</h5>} span={3}>
                                    {order.productItemTitle}
                                </Descriptions.Item>
                                <Descriptions.Item label={<h5 className="text-base font-semibold">Giá 1 đơn vị</h5>}>
                                    {order.unitPrice}
                                </Descriptions.Item>
                                <Descriptions.Item label={<h5 className="text-base font-semibold">Số lượng</h5>}>{order.quantity}</Descriptions.Item>
                                <Descriptions.Item label={<h5 className="text-base font-semibold">Đơn vị</h5>}>
                                    {order.unit ?? 'KG'}
                                </Descriptions.Item>
                                <Descriptions.Item label={<h5 className="text-base font-semibold">Tổng tiền</h5>} span={3}>
                                    <span className="text-lg font-bold text-gray-700">{order.totalPrice.toLocaleString('en-US')} VNĐ</span>
                                </Descriptions.Item>

                                {/* <Descriptions.Item label="Tên sản phẩm">{order.}</Descriptions.Item>
                                <Descriptions.Item label="Số lượng">{order.quantity}</Descriptions.Item>
                                <Descriptions.Item label="Giá tiền">{order.price}</Descriptions.Item>
                                <Descriptions.Item label="Tổng tiền">{order.totalPrice}</Descriptions.Item> */}
                            </Descriptions>
                        </div>
                    ))}
                </div>
            </div>
        </Modal>
    );
};

export default DetailOrderModal;
