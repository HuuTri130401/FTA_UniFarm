import { FormWrapper, SelectInput, TextInput } from '@components/forms';
import { CollectedHubAPI } from '@core/api/collected-hub.api';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { useMutationCreateBatches, useQueryGetListOrderInBusinessDayForFarmHub } from '@hooks/api/batch.hook';
import { Batch, CreateBatch } from '@models/batch';
import { CollectedHub } from '@models/staff';
import { useStoreUser } from '@store/index';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';

interface CreateBatchModalProps extends ModalProps {
    closeBatchModal: () => void;
    businessDayId: string;
    busisnessDayName: string;
}

const defaultValues: CreateBatch = {
    businessDayName: '',
    businessDayId: '',
    collectedId: '',
    orderIds: [],
};

const CreateBatchModal: React.FC<CreateBatchModalProps> = ({ closeBatchModal, businessDayId, busisnessDayName, ...rest }) => {
    const { mutateCreateBatch } = useMutationCreateBatches();
    const methods = useForm({
        defaultValues,
    });
    const user = useStoreUser();
    const {
        data: dataListOrder,
        isLoading: isLoadingListOrder,
        isSuccess,
    } = useQueryGetListOrderInBusinessDayForFarmHub(user.farmHub?.id as string, businessDayId);
    const queryClient = useQueryClient();

    const [orderList, setOrderList] = React.useState<Batch[]>([]);
    React.useEffect(() => {
        if (isSuccess) {
            setOrderList(dataListOrder);
        }
    }, [dataListOrder, isSuccess]);

    const { data: dataCollectedHub, isLoading: isLoadingCollectedHub } = useQuery({
        queryKey: ['collected-hub-list'],
        queryFn: async () => {
            const res = await CollectedHubAPI.getAll({
                pageSize: 999,
            });
            return res;
        },
    });
    useEffect(() => {
        if (busisnessDayName) {
            methods.setValue('businessDayName', busisnessDayName);
        }
        if (businessDayId) {
            methods.setValue('businessDayId', businessDayId);
        }
    }, [businessDayId, busisnessDayName, methods]);

    const [collectedHub, setCollectedHub] = React.useState<CollectedHub[]>([]);
    React.useEffect(() => {
        if (!isLoadingCollectedHub) {
            setCollectedHub(dataCollectedHub);
        }
    }, [dataCollectedHub, isLoadingCollectedHub]);

    const onSubmit = async (data: CreateBatch) => {
        if (data.businessDayId === '') {
            methods.setError('businessDayId', { type: 'required', message: 'Ngày bán hàng không được để trống' });
            return;
        }
        if (data.collectedId === '') {
            methods.setError('collectedId', { type: 'required', message: 'Kho nhập hàng không được để trống' });
            return;
        }
        if (data.orderIds.length === 0) {
            methods.setError('orderIds', { type: 'required', message: 'Danh sách đơn hàng không được để trống' });
            return;
        }

        mutateCreateBatch(
            {
                farmHubId: user.farmHub?.id as string,
                body: {
                    businessDayId: data.businessDayId,
                    collectedId: data.collectedId,
                    orderIds: data.orderIds,
                },
            },
            {
                onSuccess: () => {
                    queryClient.invalidateQueries();
                    closeBatchModal();
                    methods.reset({
                        businessDayName: busisnessDayName,
                        businessDayId: businessDayId,
                        collectedId: collectedHub[0]?.id,
                        orderIds: [],
                    });
                },
            }
        );
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Tạo lô hàng"
                cancelButtonProps={{ onClick: closeBatchModal }}
                closeIcon={
                    <XMarkIcon
                        className="absolute w-6 h-6 cursor-pointer top-4"
                        onClick={() => {
                            methods.clearErrors();
                            closeBatchModal();
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
                                closeBatchModal();
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
                <div className="flex flex-col gap-5">
                    <TextInput label="Ngày bán hàng" name="businessDayName" placeholder="Chọn ngày bán hàng" readOnly />
                    <SelectInput
                        label="Kho nhập hàng"
                        name="collectedId"
                        options={collectedHub?.map((item) => ({
                            label: item.name,
                            value: item.id,
                            origin: item,
                        }))}
                        defaultValue={collectedHub[0]?.id}
                        placeholder="Chọn nơi nhập hàng"
                    />
                    <SelectInput
                        label="Danh sách đơn hàng"
                        name="orderIds"
                        mode="multiple"
                        options={orderList
                            ?.filter((item) => item.customerStatus === 'Confirmed')
                            .map((item) => ({
                                label: (
                                    <>
                                        {item.code} - {item.customerName} - {item.shipAddress}
                                    </>
                                ),
                                value: item.id.toUpperCase(),
                                origin: item,
                            }))}
                        placeholder="Chọn danh sách đơn hàng"
                    />
                </div>
            </Modal>
        </FormWrapper>
    );
};

export default CreateBatchModal;
