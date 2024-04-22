import { ADMIN_API } from '@core/api/admin.api';
import { Dashboard, ProductStatistic } from '@models/admin';
import { useQuery } from '@tanstack/react-query';

export const useQueryGetDashboard = () => {
    return useQuery({
        queryKey: ['dashboard'],
        queryFn: async () => {
            const res = await ADMIN_API.getDashboard();
            return res.payload as Dashboard[];
        },
    });
};

export const useQueryGetProductStatistic = () => {
    return useQuery({
        queryKey: ['product-statistic'],
        queryFn: async () => {
            const res = await ADMIN_API.getProductStatistic();
            return res.payload as ProductStatistic[];
        },
    });
};

export const useQueryGetTopFarmHub = () => {
    return useQuery({
        queryKey: ['farm-hub-ranking'],
        queryFn: async () => {
            const res = await ADMIN_API.getTopFarmHub();
            return res.payload;
        },
    });
};
