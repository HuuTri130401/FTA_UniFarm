import { IV1TransactionGetRevenue, IV1TransactionMomoDeposit, IV1TransactionPaypalDeposit, transactionApi } from '@core/api/transaction.api';
import { useMutation, useQuery } from '@tanstack/react-query';

const useTransactionDepositMomoMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (dto: IV1TransactionMomoDeposit) => {
        const res = await transactionApi.v1DepositMomo(dto);
        return res;
    });

    return { transactionDepositMomoMutation: mutate, transactionDepositMomoMutationAsync: mutateAsync, ...rest };
};

const useTransactionDepositPaypalMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (dto: IV1TransactionPaypalDeposit) => {
        const res = await transactionApi.v1DepositPaypal(dto);
        return res;
    });

    return { transactionDepositPaypalMutation: mutate, transactionDepositPaypalMutationAsync: mutateAsync, ...rest };
};

const useTransactionSuccessDepositMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (token: string) => {
        const res = await transactionApi.v1SuccessDeposit(token);
        return res;
    });

    return { transactionSuccessDepositMutation: mutate, transactionSuccessDepositMutationAsync: mutateAsync, ...rest };
};

const useQueryRevenue = (dto: IV1TransactionGetRevenue) => {
    return useQuery(
        ['transaction', 'revenue', dto],
        async () => {
            const res = await transactionApi.v1Revenue(dto);
            return res;
        },
        {
            initialData: [],
        }
    );
};

export { useQueryRevenue, useTransactionDepositMomoMutation, useTransactionDepositPaypalMutation, useTransactionSuccessDepositMutation };
