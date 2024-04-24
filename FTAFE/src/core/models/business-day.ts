import { FarmHubMenu } from './farmhub-menu';

export type BusinessDay = {
    id: string;
    name: string;
    regiterDay: Date | string;
    endOfRegister: Date | string;
    openDay: Date | string;
    endOfDay: Date | string;
    createdAt: string;
    updatedAt: string;
    status: string;
    menus: FarmHubMenu[];
};

export type Settlement = {
    id: string;
    farmHubId: string;
    businessDayId: string;
    businessDayName: string;
    businessOpenday: string;
    priceTableId: string;
    totalSales: number;
    numOfOrder: number;
    deliveryFeeOfOrder: number;
    commissionFee: number;
    dailyFee: number;
    amountToBePaid: number;
    profit: number;
    paymentStatus: string;
    paymentDate: string;
};

export interface CreateBusinessDay extends Pick<BusinessDay, 'name' | 'openDay'| 'endOfRegister'|'regiterDay'> {}
