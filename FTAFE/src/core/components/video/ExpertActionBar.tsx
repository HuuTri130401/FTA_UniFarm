import InterviewInfoDrawer from '@components/drawers/InterviewInforDrawer';
import InterviewResultDrawer from '@components/drawers/InterviewResultDrawer';
import { interviewApi } from '@core/api/interview.api';
import { PencilIcon } from '@heroicons/react/24/outline';
import { Interview, InterviewItem } from '@models/interview';
import { Job } from '@models/job';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Question } from 'akar-icons';
import { Popover } from 'antd';
import clsx from 'clsx';
import * as React from 'react';
import { useFieldArray } from 'react-hook-form';

interface ExpertActionBarProps {
    interview: InterviewItem;
}

enum RecordingStatus {
    NOT_STARTED = 'NOT_STARTED',
    RECORDING = 'RECORDING',
    STOPPED = 'STOPPED',
}

const ExpertActionBar: React.FunctionComponent<ExpertActionBarProps> = ({ interview }) => {
    // Drawer

    const [open, setOpen] = React.useState<boolean>(false);

    const showDrawer = () => {
        setOpen(true);
    };

    const onClose = () => {
        setOpen(false);
    };

    // const { data, refetch } = useQuery<{ status: RecordingStatus }>(
    //     ['recording-status', interview.id],
    //     async () => {
    //         const res = await interviewApi.v1GetInterviewRecordStatus(interview.id);

    //         return res;
    //     },
    //     {
    //         initialData: {
    //             status: RecordingStatus.STOPPED,
    //         },
    //         refetchInterval: 5000,
    //         onSuccess: (data) => {
    //             if (data.status === RecordingStatus.NOT_STARTED) {
    //                 handleStart.mutate(interview.id);
    //             }
    //         },
    //     }
    // );

    // const handleStart = useMutation({
    //     mutationFn: interviewApi.v1StartInterviewRecord,
    //     onSuccess: () => {
    //         refetch();
    //     },
    // });

    // const handleStop = useMutation({
    //     mutationFn: interviewApi.v1StopInterviewRecord,
    //     onSuccess: () => {
    //         refetch();
    //     },
    // });

    const [showInfo, setShowInfo] = React.useState<boolean>(false);

    return (
        <aside className="flex flex-col items-center justify-start flex-shrink-0 w-16 h-full gap-2 p-2 bg-gray-200">
            <Popover content={<p className="m-0 text-base font-medium">Information</p>} placement="right">
                <button
                    onClick={() => setShowInfo(!showInfo)}
                    className="flex items-center justify-center w-12 h-12 text-gray-800 duration-300 border-2 border-gray-900 border-solid rounded-lg hover:bg-gray-900 hover:text-white"
                >
                    <Question className="w-8 h-8" />
                </button>
            </Popover>
            <Popover content={<p className="m-0 text-base font-medium">Write interview result</p>} placement="right">
                <button
                    onClick={() => showDrawer()}
                    className="flex items-center justify-center w-12 h-12 text-gray-800 duration-300 border-2 border-gray-900 border-solid rounded-lg hover:bg-gray-900 hover:text-white"
                >
                    <PencilIcon className="w-8 h-8" />
                </button>
            </Popover>
            {/* <Popover
                content={
                    <div className="flex flex-col w-[300px] h-fit">
                        <h2 className="mb-5 text-center text-header-5">Record overall</h2>
                        <div className="grid grid-cols-3 text-base gap-y-4">
                            <>
                                <p className="col-span-1 m-0 font-medium">Status:</p>

                                <p
                                    className={clsx('col-span-2 m-0 font-medium capitalize', {
                                        'text-gray-500': RecordingStatus.NOT_STARTED === data.status,
                                        'text-red-500': RecordingStatus.RECORDING === data.status,
                                        'text-green-500': RecordingStatus.STOPPED === data.status,
                                    })}
                                >
                                    {data.status}
                                </p>
                            </>
                            {data.status === RecordingStatus.RECORDING ? (
                                <>
                                    <div className="flex items-center justify-start col-span-3">
                                        <button
                                            className="flex items-center gap-2 px-4 py-1 text-white rounded-lg disabled:cursor-not-allowed bg-red"
                                            disabled={handleStop.isLoading}
                                            onClick={() => handleStop.mutate(interview.id)}
                                        >
                                            Stop Record
                                        </button>
                                    </div>
                                </>
                            ) : (
                                <div className="flex items-center justify-start col-span-3">
                                    <button
                                        className="flex items-center gap-2 px-4 py-1 text-white rounded-lg disabled:cursor-not-allowed bg-green"
                                        disabled={handleStart.isLoading}
                                        onClick={() => handleStart.mutate(interview.id)}
                                    >
                                        Start Record
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>
                }
                trigger={'click'}
                placement="right"
            >
                <button className="flex items-center justify-center w-12 h-12 duration-300 border-2 border-gray-900 border-solid rounded-lg ">
                    <div
                        className={clsx('flex items-center justify-center w-6 h-6 bg-white border-2 border-current border-solid rounded-full', {
                            'text-red-900': data.status !== RecordingStatus.RECORDING,
                            'text-red rec': data.status === RecordingStatus.RECORDING,
                        })}
                    >
                        <div className="flex flex-col w-4 h-4 bg-current rounded-full"></div>
                    </div>
                </button>
            </Popover> */}
            <InterviewResultDrawer width={880} interviewResultId={interview.id} onClose={onClose} open={open} placement="right" />
            <InterviewInfoDrawer width={1020} interviewResultId={interview.id} onClose={() => setShowInfo(false)} open={showInfo} placement="right" />
        </aside>
    );
};

export default ExpertActionBar;
