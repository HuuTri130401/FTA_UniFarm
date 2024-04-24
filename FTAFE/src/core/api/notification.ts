import { PagingProps, ResponseList } from '@models/interface';
import { Notifications } from '@models/notification';
import _get from 'lodash.get';

import { http } from './http';

export interface IV1GetFilterNotification extends PagingProps {}
export const notificationApi = {
    v1Get: async (filter: Partial<IV1GetFilterNotification>) => {
        const res = await http.get('/notification', { params: { ...filter } });
        return _get(res, 'data') as ResponseList<Notifications>;
    },
};
