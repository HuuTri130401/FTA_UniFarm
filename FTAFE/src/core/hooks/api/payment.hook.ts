import { PaymentAPI } from '@core/api/payment.api';
import { Transaction } from '@models/payment';
import { useQuery } from '@tanstack/react-query';

export const useQueryGetTransactionsAll = () => {
    return useQuery({
        queryKey: ['transactions'],
        queryFn: async () => {
            const res = await PaymentAPI.getTransactionsAll();
            return res.payload as Transaction[];
        },
    });
};
