export type BookingTime =
    | '08:00'
    | '08:30'
    | '09:00'
    | '09:30'
    | '10:00'
    | '10:30'
    | '11:00'
    | '11:30'
    | '12:00'
    | '12:30'
    | '13:00'
    | '13:30'
    | '14:00'
    | '14:30'
    | '15:00'
    | '15:30'
    | '16:00'
    | '16:30'
    | '17:00'
    | '17:30';

export const bookingTimeOptions: Array<{
    label: BookingTime;
    value: BookingTime;
}> = [
    {
        label: '08:00',
        value: '08:00',
    },
    {
        label: '08:30',
        value: '08:30',
    },
    {
        label: '09:00',
        value: '09:00',
    },
    {
        label: '09:30',
        value: '09:30',
    },
    {
        label: '10:00',
        value: '10:00',
    },
    {
        label: '10:30',
        value: '10:30',
    },
    {
        label: '11:00',
        value: '11:00',
    },
    {
        label: '11:30',
        value: '11:30',
    },
    {
        label: '12:00',
        value: '12:00',
    },
    {
        label: '12:30',
        value: '12:30',
    },
    {
        label: '13:00',
        value: '13:00',
    },
    {
        label: '13:30',
        value: '13:30',
    },
    {
        label: '14:00',
        value: '14:00',
    },
    {
        label: '14:30',
        value: '14:30',
    },
    {
        label: '15:00',
        value: '15:00',
    },
    {
        label: '15:30',
        value: '15:30',
    },
    {
        label: '16:00',
        value: '16:00',
    },
    {
        label: '16:30',
        value: '16:30',
    },
    {
        label: '17:00',
        value: '17:00',
    },
    {
        label: '17:30',
        value: '17:30',
    },
];
