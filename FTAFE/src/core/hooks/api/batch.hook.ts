import { BatchAPI } from '@core/api/batch.api';
import { Batch, BatchDetail, CreateBatch, CreateBatchREQ } from '@models/batch';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-toastify';

const useQueryGetListOrderInBusinessDayForFarmHub = (farmhubId: string, businessDayId: string) => {
    return useQuery(
        ['order', 'farmhub', farmhubId, businessDayId],
        async () => {
            const res = await BatchAPI.getListOrderInBusinessDayFarmHub(farmhubId, businessDayId);
            return res.payload as Batch[];
        },
        {
            initialData: [],
        }
    );
};

const useQueryGetListOrderByFarmHub = (farmhubId: string) => {
    return useQuery(
        ['order', 'farmhub', farmhubId],
        async () => {
            const res = await BatchAPI.getListOrderByFarmHub(farmhubId);
            return res.payload as Batch[];
        },
        {
            initialData: [],
        }
    );
};

const useQueryGetListBatchByBusinessDayFarmHub = (farmhubId: string, businessDayId: string) => {
    return useQuery(
        ['batch', 'farmhub', farmhubId, businessDayId],
        async () => {
            const res = await BatchAPI.getListBatchByBusinessDayFarmHub(farmhubId, businessDayId);
            return res.payload as Batch[];
        },
        {
            initialData: [],
        }
    );
};

const useQueryGetDetailBatch = (batchId: string) => {
    return useQuery(
        ['order', 'detail', batchId],
        async () => {
            const res = await BatchAPI.getDetailBatch(batchId);
            return res.payload as BatchDetail;
        },
        {
            initialData: {} as BatchDetail,
        }
    );
};

const useMutationConfirmOrder = () => {
    const queryClient = useQueryClient();
    const { mutate, ...rest } = useMutation(
        (input: { orderId: string; confirmStatus: string }) => {
            return BatchAPI.confirmOrder(input.orderId, input.confirmStatus);
        },
        {
            onSuccess: () => {
                toast.success('Cập nhật thành công!');
                queryClient.invalidateQueries();
            },
        }
    );
    return { mutateConfirmOrder: mutate, ...rest };
};

const useMutationCreateBatch = () => {
    const queryClient = useQueryClient();

    const { mutate, ...rest } = useMutation(
        (input: { farmHubId: string; body: CreateBatch }) => {
            return BatchAPI.createBatch(input.farmHubId, input.body);
        },
        {
            onSuccess: () => {
                toast.success('Tạo batch thành công!');
                queryClient.invalidateQueries();
            },
        }
    );
    return { mutateCreateBatch: mutate, ...rest };
};

const useMutationCreateBatches = () => {
    const queryClient = useQueryClient();

    const { mutate, ...rest } = useMutation(
        (input: { farmHubId: string; body: CreateBatchREQ }) => {
            return BatchAPI.createBatches(input.farmHubId, input.body);
        },
        {
            onSuccess: () => {
                toast.success('Tạo batch thành công!');
                queryClient.invalidateQueries();
            },
        }
    );
    return { mutateCreateBatch: mutate, ...rest };
};

export {
    useMutationConfirmOrder,
    useMutationCreateBatch,
    useMutationCreateBatches,
    useQueryGetDetailBatch,
    useQueryGetListBatchByBusinessDayFarmHub,
    useQueryGetListOrderByFarmHub,
    useQueryGetListOrderInBusinessDayForFarmHub,
};
