import { PagingProps, ResponseList } from '@models/interface';
import { Transaction } from '@models/transaction';
import { User, UserItem } from '@models/user';
import _get from 'lodash.get';

import { http } from '.';

// export interface IV1UserUpdateMe extends Pick<User> {}
export interface IV1UserUpdateMePassword {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

export interface IV1UserUpdateMeAvatar extends Pick<User, 'avatar'> {}

export interface IV1UserTransactionFilter extends PagingProps {}

export const userApi = {
    v1GetMe: async () => {
        const res = await http.get<UserItem>('/aboutMe');
        return _get(res, 'data');
    },
    v1UserGetTransactions: async (filter: Partial<IV1UserTransactionFilter>) => {
        const res = await http.get<ResponseList<Transaction>>('/user/transactions', { params: { ...filter } });
        return _get(res, 'data');
    },
};
