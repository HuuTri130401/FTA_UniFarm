import { Space, Table } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import clsx from 'clsx';
import moment, { Moment } from 'moment';
import * as React from 'react';

interface WeekDays {
    Sun: React.ReactNode;
    Mon: React.ReactNode;
    Tue: React.ReactNode;
    Wed: React.ReactNode;
    Thu: React.ReactNode;
    Fri: React.ReactNode;
    Sat: React.ReactNode;
}

interface TableType {
    key: React.Key;
    name: React.ReactNode;
    hour: string;
    date: string;
    weekdays: WeekDays;
}

const columns: ColumnsType<TableType> = [
    {
        title: 'Hours',
        dataIndex: 'hours',
        key: 'hours',
        width: 120,
        className: 'bg-gray-50',
        render: (_, record) => (
            <Space size="middle" className="font-medium">
                {record.hour.toUpperCase()}
            </Space>
        ),
    },
    {
        title: 'Sun',
        dataIndex: 'Sun',
        key: 'Sun',
        render: (_, record) => {
            return record.weekdays.Sun;
        },
    },
    {
        title: 'Mon',
        dataIndex: 'Mon',
        key: 'Mon',
        render: (_, record) => {
            return record.weekdays.Mon;
        },
    },
    {
        title: 'Tue',
        dataIndex: 'Tue',
        key: 'Tue',
        render: (_, record) => {
            return record.weekdays.Tue;
        },
    },
    {
        title: 'Wed',
        dataIndex: 'Wed',
        key: 'Wed',
        render: (_, record) => {
            return record.weekdays.Wed;
        },
    },
    {
        title: 'Thu',
        dataIndex: 'Thu',
        key: 'Thu',
        render: (_, record) => {
            return record.weekdays.Thu;
        },
    },
    {
        title: 'Fri',
        dataIndex: 'Fri',
        key: 'Fri',
        render: (_, record) => {
            return record.weekdays.Fri;
        },
    },
    {
        title: 'Sat',
        dataIndex: 'Sat',
        key: 'Sat',
        render: (_, record) => {
            return record.weekdays.Sat;
        },
    },
];

export interface DataType<T> {
    id: string;
    event: T;
    startTime: number;
    endTime: number;
    date: Moment;
    slotId: string;
}

export interface SlotType {
    id: unknown;
    name: string;
    hour: string;
}

interface CalendarProps<T> {
    events: DataType<T>[];
    slots: SlotType[];
    onDisplayEvent: (event: T) => React.ReactNode;
    onCompare: (event: DataType<T>, Slot: SlotType) => boolean;
    currentWeek?: number;
}

/**
 * This is a component that displays a weekly calendar
 * @param {T} T - the type of the event
 * @param {DateType<T>} events - event only display if date of event in range the current week and correct slotId
 * @param {SlotType} slots - the slots of the calendar and make sure the id is unique
 * @param {Function} onDisplayEvent - the function to display the event return a ReactNode
 * @param {Function} onCompare - the function to compare the event and the slot
 * @param {number} currentWeek - the current week of the calendar
 */

const WeeklyCalendar = <T,>({ events, slots, currentWeek, onDisplayEvent, onCompare }: CalendarProps<T>) => {
    const [week, setWeek] = React.useState(currentWeek || moment().weeks());

    React.useEffect(() => {
        if (currentWeek) {
            setWeek(currentWeek);
        }
    }, [currentWeek]);

    const getEventByWeekDayAndSlot = React.useCallback(
        (weekday: number, slot: SlotType) => {
            return events.find((event) => {
                const { date: eventDate } = event;
                const eventWeekday = moment(eventDate).weekday();
                return onCompare(event, slot) && eventWeekday === weekday && event.date.week() === week;
            });
        },
        [events, onCompare, week]
    );

    const tableData = React.useMemo<TableType[]>(() => {
        const tableData = slots.map((slot: SlotType) => {
            const { id: slotId, name: slotName, hour } = slot;
            const weekdays: WeekDays = {
                Sun: null,
                Mon: null,
                Tue: null,
                Wed: null,
                Thu: null,
                Fri: null,
                Sat: null,
            };

            for (let i = 0; i < 7; i++) {
                const event = getEventByWeekDayAndSlot(i, slot);
                if (event) {
                    const { event: eventData } = event;
                    const weekday = moment().weekday(i).format('ddd') as keyof WeekDays;
                    weekdays[weekday] = onDisplayEvent(eventData);
                }
            }

            return {
                key: `${slotId}`,
                name: slotName,
                hour,
                date: moment().format('YYYY-MM-DD'),
                weekdays,
            };
        });

        return tableData;
    }, [slots, getEventByWeekDayAndSlot, onDisplayEvent]);

    const renderHeadRow = React.useCallback(
        (columns: ColumnsType<TableType>) => {
            const currentWeekday = moment().format('ddd: DD/MM');

            const columnsFormatWeekday = columns.map((column) => {
                switch (column.title) {
                    case 'Sun':
                        return {
                            ...column,
                            title: moment().week(week).weekday(0).format('ddd: DD/MM'),
                        };
                    case 'Mon':
                        return {
                            ...column,
                            title: moment().week(week).weekday(1).format('ddd: DD/MM'),
                        };
                    case 'Tue':
                        return {
                            ...column,
                            title: moment().week(week).weekday(2).format('ddd: DD/MM'),
                        };
                    case 'Wed':
                        return {
                            ...column,
                            title: moment().week(week).weekday(3).format('ddd: DD/MM'),
                        };
                    case 'Thu':
                        return {
                            ...column,
                            title: moment().week(week).weekday(4).format('ddd: DD/MM'),
                        };
                    case 'Fri':
                        return {
                            ...column,
                            title: moment().week(week).weekday(5).format('ddd: DD/MM'),
                        };
                    case 'Sat':
                        return {
                            ...column,
                            title: moment().week(week).weekday(6).format('ddd: DD/MM'),
                        };

                    default:
                        return column;
                }
            });

            const finaleColumns = columnsFormatWeekday.map((column) => {
                if (column.title === currentWeekday) {
                    return {
                        ...column,
                        className: 'bg-blue-50',
                    };
                }
                return column;
            });

            return finaleColumns;
        },
        [week]
    );

    // format of 05:32:34 AM to only get 05:00:00 AM

    const formatHour = (input: string, hour: string) => {
        const hourFormat = hour.split(':');
        const hourFormatOnlyHour = hourFormat[0];
        const hourFormatOnlyMinute = hourFormat[1];
        const hourFormatOnlySecond = hourFormat[2].split(' ')[0];
        const hourFormatOnlyAMPM = hourFormat[2].split(' ')[1];
        return `${hourFormatOnlyHour}:00:00 ${hourFormatOnlyAMPM}`;
    };

    return (
        <Table
            sticky={true}
            className="relative overflow-y-auto h-[702px]"
            bordered={true}
            onHeaderRow={() => ({ className: 'bg-gray-100' })}
            onRow={(data) => ({
                className: clsx('hover:bg-blue-50', {
                    'bg-blue-50': data.hour.toUpperCase() === formatHour(data.hour, moment().format('hh:mm:ss A')),
                }),
            })}
            pagination={false}
            columns={renderHeadRow(columns)}
            dataSource={tableData}
        />
    );
};

export default WeeklyCalendar;
