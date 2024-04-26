import { useTableUtil } from '@context/tableUtilContext';
import { IV1UserTransactionFilter, userApi } from '@core/api';
import { useQuery } from '@tanstack/react-query';

const useQueryUserTransactions = (filter: Partial<IV1UserTransactionFilter>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['transactions', filter],
        async () => {
            const res = await userApi.v1UserGetTransactions(filter);
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

export { useQueryUserTransactions };
