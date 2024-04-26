import { useTableUtil } from '@context/tableUtilContext';
import { IV1GetFilterNotification, notificationApi } from '@core/api/notification';
import { useQuery } from '@tanstack/react-query';

const useQueryNotificationFilter = (filter: Partial<IV1GetFilterNotification>) => {
    const { setTotalItem } = useTableUtil();
    return useQuery(
        ['notifications', filter],
        async () => {
            const res = await notificationApi.v1Get(filter);
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
            // refetchInterval: 5000,
            // refetchIntervalInBackground: true,
        }
    );
};

export { useQueryNotificationFilter };
