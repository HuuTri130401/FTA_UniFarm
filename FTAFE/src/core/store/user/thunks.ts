import { userApi } from '@core/api';
import { UserItem } from '@models/user';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const userThunk = {
    getCurrentUser: createAsyncThunk<UserItem, void>('getCurrentUser', async (_, { rejectWithValue }) => {
        try {
            const res = await userApi.v1GetMe();
            return res;
        } catch (error) {
            return rejectWithValue(null);
        }
    }),
};
