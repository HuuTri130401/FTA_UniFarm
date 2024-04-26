import { Candidate } from '@models/candidate';
import { Wallet } from '@models/wallet';
import { combineReducers, configureStore } from '@reduxjs/toolkit';
import { useSelector } from 'react-redux';

import { apiReducer, ApiState } from './api';
import { candidateReducer } from './candidate';
import { expertReducer, ExpertState } from './expert';
import { userReducer, UserState } from './user';
import { walletReducer } from './wallet';

export interface RootState {
    api: ApiState;
    user: UserState;
    expert: ExpertState;
    candidate: Candidate;
    wallet: Wallet;
}

const reducers = combineReducers<RootState>({
    api: apiReducer,
    user: userReducer,
    expert: expertReducer,
    candidate: candidateReducer,
    wallet: walletReducer,
});

export const store = configureStore({
    reducer: reducers,
    devTools: process.env.NODE_ENV !== 'production',
});

export const useStoreApi: () => ApiState = () => useSelector<RootState, ApiState>((state) => state.api);
export const useStoreUser: () => UserState = () => useSelector<RootState, UserState>((state) => state.user);
export const useStoreExpert: () => ExpertState = () => useSelector<RootState, ExpertState>((state) => state.expert);
export const useStoreCandidate: () => Candidate = () => useSelector<RootState, Candidate>((state) => state.candidate);
export const useStoreWallet: () => Wallet = () => useSelector<RootState, Wallet>((state) => state.wallet);
