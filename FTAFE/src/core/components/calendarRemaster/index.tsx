import { Button, DatePicker, DatePickerProps, Table } from 'antd';
import { ColumnType } from 'antd/lib/table';
import clsx from 'clsx';
import moment from 'moment';
import * as React from 'react';

interface CalenderRemasterProps {
    week?: number;
    data: EventData[];
    onWeekChange?: (week: number) => void;
}

export interface EventData {
    startTimestamp: number;
    endTimestamp?: number;
    element: React.ReactNode;
}

const CalenderRemaster: React.FunctionComponent<CalenderRemasterProps> = ({ week = moment().week(), data = [], onWeekChange }) => {
    const [currentWeek, setCurrentWeek] = React.useState(week);

    const columns = React.useMemo(() => {
        const columns: ColumnType<any>[] = [
            {
                title: 'Hours',
                dataIndex: 'hours',
                key: 'hours',
                width: 72,
                className: 'bg-gray-50',
                render: (_, record) => <div>{record.hours}</div>,
            },
        ];

        for (let day = 0; day < 7; day++) {
            // find event data by timestamp
            const eventData = data.filter(
                (event) => moment(event.startTimestamp - 25200000).day() === day && moment(event.startTimestamp - 25200000).week() === currentWeek
            );

            columns.push({
                title: (
                    <div
                        className={clsx('capitalize z-[999]', {
                            'bg-blue/40 absolute h-full  w-full max-w-[218px] flex items-center p-4 top-0 left-0':
                                moment().day() === day && moment().week() === currentWeek,
                            relative: !Boolean(moment().day() === day && moment().week() === currentWeek),
                        })}
                    >
                        {moment().week(currentWeek).day(day).format('ddd DD/MM')}
                    </div>
                ),
                onCell: (record) => ({
                    className: clsx('w-full h-full !p-0 !h-[60px] border-0 relative', {}),
                }),
                render: (_, record) => {
                    const isCurrentTime = moment().hours() === record.key && moment().day() === day && moment().week() === currentWeek;
                    const events = eventData.filter(
                        (event) =>
                            moment(event.startTimestamp - 25200000).hours() === record.key &&
                            moment(event.startTimestamp - 25200000).week() === currentWeek
                    );

                    if (!Boolean(events.length))
                        return (
                            <div className="relative z-10 w-full h-full">
                                {/* current real time */}
                                {isCurrentTime && (
                                    <div className="absolute left-0 w-full" id="current-time" style={{ top: `${moment().minutes()}px` }}>
                                        <div className="relative w-full h-0.5 bg-red">
                                            <div className="absolute w-3 h-3 rounded-full -top-1 -left-2 bg-red"></div>
                                        </div>
                                    </div>
                                )}
                            </div>
                        );

                    return (
                        <div className="relative z-10 w-full h-[60px]">
                            {events.map((event) => {
                                const topMinutes = moment(event.startTimestamp - 25200000).minutes();
                                const heightMinutes = event.endTimestamp && (event.endTimestamp - event.startTimestamp - 25200000) / 60000;

                                return (
                                    <div
                                        key={event.startTimestamp - 25200000}
                                        className="absolute w-full rounded-md text-white min-h-[24px]"
                                        style={{
                                            top: `${topMinutes}px`,
                                            height: `${heightMinutes}px`,
                                        }}
                                    >
                                        {event.element}
                                    </div>
                                );
                            })}

                            {/* current real time */}
                            {isCurrentTime && (
                                <div className="absolute w-full" id="current-time" style={{ top: `${moment().minutes()}px` }}>
                                    <div className="relative w-full h-0.5 bg-red">
                                        <div className="absolute w-3 h-3 rounded-full -top-1 -left-2 bg-red"></div>
                                    </div>
                                </div>
                            )}
                        </div>
                    );
                },
            });
        }

        return columns;
    }, [currentWeek, data]);

    const dataSource = React.useMemo(() => {
        const data = [];

        for (let hour = 0; hour < 24; hour++) {
            data.push({
                key: hour,
                hours: <p className="mb-0 font-semibold">{hour}:00</p>,
            });
        }

        return data;
    }, []);

    const onChange: DatePickerProps['onChange'] = (date) => {
        if (date === null) {
            setCurrentWeek(moment().week());
            return;
        }
        setCurrentWeek(moment(date).week());
    };

    React.useEffect(() => {
        if (onWeekChange) {
            onWeekChange(currentWeek);
        }
    }, [currentWeek]);

    React.useEffect(() => {
        // scroll current time to view
        const currentTime = document.getElementById('current-time');
        if (currentTime) {
            currentTime.scrollIntoView({
                behavior: 'smooth',
                block: 'center',
                inline: 'center',
            });
        }
    }, [currentWeek]);

    return (
        <div className="flex flex-col gap-4 max-h-[calc(100vh-200px)] w-full">
            <div className="flex gap-3">
                <div>
                    <DatePicker onChange={onChange} picker="week" value={moment().week(currentWeek)} />
                </div>
                <div className="flex gap-2">
                    <Button type="primary" onClick={() => setCurrentWeek(currentWeek - 1)}>
                        Previous
                    </Button>
                    <Button type="primary" onClick={() => setCurrentWeek(moment().week())}>
                        Today
                    </Button>
                    <Button type="primary" onClick={() => setCurrentWeek(currentWeek + 1)}>
                        Next
                    </Button>
                </div>
            </div>
            <Table
                sticky={true}
                className="relative overflow-x-scroll overflow-y-auto "
                onRow={(data) => ({
                    className: clsx('hover:bg-blue-50', {}),
                    title: data.title,
                })}
                bordered={true}
                pagination={false}
                columns={columns}
                dataSource={dataSource}
            />
        </div>
    );
};

export default CalenderRemaster;
