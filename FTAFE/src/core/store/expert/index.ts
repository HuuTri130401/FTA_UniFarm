import { Expert, expertDefaultValues } from '@models/expert';
import { UserRole } from '@models/user';
import { createSelector, createSlice } from '@reduxjs/toolkit';
import { userThunk } from '@store/user/thunks';

import { RootState } from '..';

export interface ExpertState extends Expert {}

const initialState: ExpertState = {
    ...expertDefaultValues,
};

const reducer = createSlice({
    name: 'expert',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(userThunk.getCurrentUser.fulfilled, (state, { payload }) => {
            /*  const expert = payload.expert;

            if (payload.type !== UserRole.EXPERT) {
                return { ...state };
            }

            return { ...state, ...expert }; */
            return { ...state };
        });
    },
});
export const expertActions = {
    ...reducer.actions,
};
export const expertReducer = reducer.reducer;

export const useSelectExpert = () =>
    createSelector(
        (state: RootState) => state.expert,
        (expert) => expert
    );
