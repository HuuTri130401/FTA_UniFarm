import { CollectedHub, Station } from './staff';

export type Transfer = {
    id: string;
    collectedId: string;
    stationId: string;
    createdAt: string;
    updatedAt: string;
    expectedReceiveDate: string | null;
    receivedDate: string;
    createdBy: string;
    updatedBy: string | null;
    noteSend: string;
    noteReceived: string;
    code: string;
    status: string;
    collected: CollectedHub;
    station: Station;
    orders: any | null;
};
export interface CreateTransferForm {
    collectedId: string;
    stationId: string;
    noteSend: string;
    expectedReceiveDate: Date | string;
    orderIds: string[];
}

export interface UpdateTransferForm {
    id: string;
    status: string;
    noteReceived: string;
}
