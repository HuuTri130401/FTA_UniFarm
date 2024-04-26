import { useTableUtil } from '@context/tableUtilContext';
import { IV1GetFilterStaff, staffApi } from '@core/api/staff.api';
import { userItemDefaultValues } from '@models/user';
import { useQuery } from '@tanstack/react-query';

const useQueryStaffById = (id: string) => {
    return useQuery(
        ['staff', id],
        async () => {
            //const res = await staffApi.v1Get(id);
            const res = {};
            return res;
        },
        {
            initialData: userItemDefaultValues,
        }
    );
};

const useQueryStaffFilter = (filter: Partial<IV1GetFilterStaff>) => {
    const { setTotalItem } = useTableUtil();

    return useQuery(
        ['staff', filter],
        async () => {
            //const res = await staffApi.v1GetFilter(filter);
            const res = { total: 0, data: [] };
            setTotalItem(res.total);
            return res.data;
        },
        {
            initialData: [],
        }
    );
};

export { useQueryStaffById, useQueryStaffFilter };
