import { BaseModel } from './interface';
import { User, userDefaultValues } from './user';

export interface Candidate extends BaseModel {
    id: string;
    // firebaseId: string;
    // deviceId: string;
}

export const candidateDefaultValues: Candidate = {
    id: '',
    // firebaseId: '',
    // deviceId: '',
    createdAt: '',
    isDeleted: false,
    updatedAt: '',
};

export interface CandidateItem extends Candidate {
    user: User;
}

export const candidateItemDefaultValue: CandidateItem = {
    ...candidateDefaultValues,
    user: userDefaultValues,
};
export interface Customer extends User {}
