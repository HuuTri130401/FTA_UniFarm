import { BusinessDayAPI } from '@core/api/business-day.api';
import { BusinessDay, Settlement } from '@models/business-day';
import { useQuery } from '@tanstack/react-query';

export const useQueryGetDetailBusinessDay = (id: string) => {
    return useQuery({
        queryKey: ['business-day', id],
        queryFn: async () => {
            const res = await BusinessDayAPI.getDetailByFarmHub(id);
            return res.payload as BusinessDay;
        },
    });
};

export const useQueryGetSettlement = (businessDayId: string, farmHubId: string) => {
    return useQuery({
        queryKey: ['settlement', businessDayId, farmHubId],
        queryFn: async () => {
            const res = await BusinessDayAPI.getSettlement(businessDayId, farmHubId);
            return res.payload as Settlement;
        },
    });
};
