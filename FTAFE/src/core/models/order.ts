import { BusinessDay } from './business-day';
import { Station } from './staff';
import { FarmHub } from './user';

export type OrderTien = {
    id: string;
    farmHubId: string;
    customerId: string;
    stationId: string;
    businessDayId: string;
    createdAt: string;
    expectedReceiveDate: string;
    code: string;
    shipAddress: string;
    totalAmount: number;
    deliveryStatus: string;
    customerStatus: string;
    farmHubResponse: FarmHub;
    businessDayResponse?: BusinessDay;
    stationResponse?: Station;
    orderDetailResponse?: any;
    batchResponse?: any;
    transferResponse?: any;
    customerResponse?: any;
};
// export interface OrderFilter extends PagingProps, PicK<Order, ""> {}
export type OrderTri = {
    id: string;
    customerId: string;
    customerName: string | null;
    createdAt: string;
    code: string;
    shipAddress: string;
    totalAmount: number;
    customerStatus: string;
    deliveryStatus: string;
    isPaid: boolean;
};
// export type OrderTienTransfer = {
//     id: 'e177202c-5a27-4f1f-abfd-7f656bd6fb2c';
//     farmHubId: 'fffd91bc-a792-4687-a4ab-498be1789490';
//     customerId: '60e1e7b7-73e8-4a37-8c51-58401f09f7e9';
//     stationId: '72a47835-6bef-4287-8408-00659bac2e3f';
//     businessDayId: '24c71120-6a21-4380-83ad-61c431a4d1f2';
//     createdAt: '2024-03-23T23:57:08.0166667';
//     expectedReceiveDate: '2024-03-23T23:57:08.0166667';
//     code: 'STU901';
//     shipAddress: '901 Road, City, Country';
//     totalAmount: 700;
//     deliveryStatus: 'OnTheWayToStation';
//     customerStatus: 'AtCollectedHub';
//     farmHubResponse: {
//         id: 'fffd91bc-a792-4687-a4ab-498be1789490';
//         name: 'Hữu Trí Farm';
//         code: 'HTF001';
//         description: 'Chuyên thu gom điều và cà phê';
//         image: 'https://res.cloudinary.com/dweagskzl/image/upload/v1711103947/irj1wvfivdqde1peq0fk.jpg';
//         address: 'Bình Phước';
//         createdAt: '2024-03-22T17:39:08.75';
//         updatedAt: '0001-01-01T00:00:00';
//         status: 'Active';
//     };
// };
