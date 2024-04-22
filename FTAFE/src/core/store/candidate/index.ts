import { Candidate, candidateDefaultValues } from '@models/candidate';
import { UserRole } from '@models/user';
import { createSelector, createSlice } from '@reduxjs/toolkit';
import { userThunk } from '@store/user/thunks';

import { RootState } from '..';

export interface CandidateState extends Candidate {}

const initialState: CandidateState = {
    ...candidateDefaultValues,
};

const reducer = createSlice({
    name: 'candidate',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(userThunk.getCurrentUser.fulfilled, (state, { payload }) => {
            //const expert = payload.candidate;

            if (payload.roleName !== UserRole.ADMIN) {
                return { ...state };
            }

            //return { ...state, ...expert };
            return { ...state };
        });
    },
});
export const candidateActions = {
    ...reducer.actions,
};
export const candidateReducer = reducer.reducer;

export const useSelectCandidate = () =>
    createSelector(
        (state: RootState) => state.candidate,
        (candidate) => candidate
    );
