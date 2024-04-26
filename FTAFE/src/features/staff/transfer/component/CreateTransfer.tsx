import { DateInput, FormWrapper, TextInput } from '@components/forms';
import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { OrderAPI } from '@core/api/order.api';
import { TransferAPI } from '@core/api/transfer.api';
import { OrderTien } from '@models/order';
import { CreateTransferForm } from '@models/transfer';
import { useStoreUser } from '@store/index';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps, message } from 'antd';
import { TableProps } from 'antd/es/table';
import moment from 'moment';
import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

type TableRowSelection<T> = TableProps<T>['rowSelection'];

interface CreateProductModalProps extends ModalProps {
    stationId: string;
}

const defaultValues = {
    collectedId: '',
    stationId: '',
    noteSend: '',
    expectedReceiveDate: new Date(),
    orderIds: [],
};

const CreateTransfer: React.FC<CreateProductModalProps> = ({ stationId, ...rest }) => {
    const [modalState, setModalState] = React.useState(false);
    const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
    const methods = useForm({
        defaultValues,
    });
    const { errors } = methods.formState;
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['orders', 'station', stationId],
        queryFn: async () => {
            const res = await OrderAPI.getForCreateTransfer(stationId);
            setTotalItem(res.payload.length);
            return res;
        },
    });

    const queryClient = useQueryClient();
    const createTransferMutation = useMutation(async (data: CreateTransferForm) => await TransferAPI.create(data));
    const user = useStoreUser();

    const onSubmit = async (data: CreateTransferForm) => {
        if (data.orderIds.length < 1) {
            methods.setError('orderIds', {
                message: 'Phải có ít nhất một đơn hàng',
                type: 'manual',
            });
            return;
        }
        const _data = { ...data, expectedReceiveDate: new Date(data.expectedReceiveDate).toISOString() };
        createTransferMutation.mutate(_data, {
            onSuccess: () => {
                queryClient.invalidateQueries(['orders', 'station', stationId]);
                rest.afterClose && rest.afterClose();
                toast.success('Tạo chuyến hàng thành công');
                setModalState(false);
                methods.reset();
            },
        });
    };

    const onSelectChange = (newSelectedRowKeys: React.Key[]) => {
        setSelectedRowKeys(newSelectedRowKeys);
        methods.clearErrors();
    };
    const rowSelection: TableRowSelection<OrderTien> = {
        selectedRowKeys,
        onChange: onSelectChange,
    };
    useEffect(() => {
        methods.setValue('stationId', stationId);
        if (user.collectedHub) methods.setValue('collectedId', user.collectedHub.id);
    }, [user, stationId]);
    useEffect(() => {
        errors.orderIds && message.error(errors.orderIds.message);
    }, [errors.orderIds]);
    return (
        <FormWrapper methods={methods}>
            <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                {' '}
                <TableBuilder<OrderTien>
                    rowKey="id"
                    isLoading={isLoading}
                    data={data?.payload}
                    rowSelection={rowSelection}
                    columns={[
                        {
                            title: () => <TableHeaderCell key="code" sortKey="code" label="Mã" />,
                            width: 400,
                            key: 'code',
                            render: ({ ...props }: OrderTien) => <p className="m-0">{props.code}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="createdAt" sortKey="createdAt" label="Ngày tạo" />,
                            width: 400,
                            key: 'createdAt',
                            render: ({ ...props }: OrderTien) => <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}</p>,
                        },

                        {
                            title: () => <TableHeaderCell key="customerId" sortKey="customerId" label="Mã khách hàng" />,
                            width: 400,
                            key: 'customerId',
                            render: ({ ...props }: OrderTien) => <p className="m-0">{props.customerId}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="shipAddress" sortKey="shipAddress" label="Địa chỉ giao hàng" />,
                            width: 400,
                            key: 'shipAddress',
                            render: ({ ...props }: OrderTien) => <p className="m-0">{props.shipAddress}</p>,
                        },
                        {
                            title: () => <TableHeaderCell key="totalAmount" sortKey="totalAmount" label="Tổng số lượng" />,
                            width: 400,
                            key: 'totalAmount',
                            render: ({ ...props }: OrderTien) => <p className="m-0">{props.totalAmount}</p>,
                        },
                    ]}
                />
                <Button
                    disabled={data?.payload.length === 0}
                    type="primary"
                    onClick={() => {
                        setModalState(!modalState);
                        methods.setValue('orderIds', selectedRowKeys as never[]);
                    }}
                >
                    Tiếp theo
                </Button>
                <Modal
                    open={modalState}
                    onCancel={() => setModalState(false)}
                    afterClose={() => setModalState(false)}
                    width={600}
                    footer={[
                        <Button key="close" type="default" onClick={rest.onCancel}>
                            Trở lại
                        </Button>,
                        <Button
                            key="create"
                            type="primary"
                            onClick={() => {
                                methods.handleSubmit(onSubmit)();
                            }}
                        >
                            Tạo
                        </Button>,
                    ]}
                >
                    <TextInput name="noteSend" label="Tin nhắn" />
                    <DateInput name="expectedReceiveDate" label="Ngày nhận dự kiến" />
                </Modal>
            </form>
        </FormWrapper>
    );
};

export default CreateTransfer;
