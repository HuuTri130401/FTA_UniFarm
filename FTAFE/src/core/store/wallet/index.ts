import { UserRole } from '@models/user';
import { Wallet, walletDefaultValues } from '@models/wallet';
import { createSelector, createSlice } from '@reduxjs/toolkit';
import { userThunk } from '@store/user/thunks';

import { RootState } from '..';

export interface WalletState extends Wallet {}

const initialState: WalletState = {
    ...walletDefaultValues,
};

const reducer = createSlice({
    name: 'wallet',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(userThunk.getCurrentUser.fulfilled, (state, { payload }) => {
            const { wallet } = payload;

            return { ...state, ...wallet };
        });
    },
});
export const walletActions = {
    ...reducer.actions,
};
export const walletReducer = reducer.reducer;

export const useSelectWallet = () =>
    createSelector(
        (state: RootState) => state.wallet,
        (wallet) => wallet
    );
