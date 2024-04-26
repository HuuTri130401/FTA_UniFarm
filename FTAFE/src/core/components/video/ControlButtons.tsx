import { MicrophoneIcon, RectangleStackIcon, VideoCameraIcon, VideoCameraSlashIcon } from '@heroicons/react/24/outline';
import clsx from 'clsx';
import * as React from 'react';

interface ControlButtonsProps {
    isJoined: boolean;
    isShareScreen: boolean;
    isMicMuted: boolean;
    isCameraMuted: boolean;
    // setIsMicMuted: React.Dispatch<React.SetStateAction<boolean>>;
    // setIsCameraMuted: React.Dispatch<React.SetStateAction<boolean>>;
    handleMuteCamera: () => void;
    handleMuteMic: () => void;
    setIsShareScreen: React.Dispatch<React.SetStateAction<boolean>>;
    cameraAvailable: boolean;
    handleEndCall: () => void;
}

const ControlButtons: React.FunctionComponent<ControlButtonsProps> = ({
    isJoined,
    isMicMuted,
    isCameraMuted,
    handleMuteCamera,
    handleMuteMic,
    cameraAvailable,
    isShareScreen,
    setIsShareScreen,
    handleEndCall,
}) => {
    return (
        <div className="flex items-center justify-center gap-6">
            <button
                className={clsx('flex flex-col items-center justify-center cursor-pointer text-white', {
                    // 'text-white': isMicMuted,
                    // 'text-white ': !isMicMuted,
                })}
                onClick={() => handleMuteMic()}
                disabled={!isJoined}
            >
                <div className="relative w-5 h-5">
                    <MicrophoneIcon />
                    {isMicMuted && <div className="w-full h-0.5 bg-white absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 rotate-45" />}
                </div>
                <p className="m-0 text-sm font-medium text-gray-100">Microphone</p>
            </button>
            <button
                className={clsx('flex flex-col items-center justify-center', {
                    'text-gray-600 cursor-not-allowed': !cameraAvailable,
                    'text-white cursor-pointer': cameraAvailable,
                })}
                onClick={() => cameraAvailable && handleMuteCamera()}
                disabled={!isJoined}
            >
                <div className="w-5 h-5 ">{!cameraAvailable || isCameraMuted ? <VideoCameraSlashIcon /> : <VideoCameraIcon />}</div>
                <p className="m-0 text-sm font-medium text-gray-100">Camera</p>
            </button>

            <button
                className="flex flex-col items-center justify-center text-white cursor-pointer"
                onClick={() => setIsShareScreen(!isShareScreen)}
                disabled={!isJoined}
            >
                <div className="w-5 h-5 ">
                    <RectangleStackIcon />
                </div>
                <p className="m-0 text-sm font-medium">Share</p>
            </button>
            <button className="flex items-center gap-2 px-4 py-1 text-white rounded-lg bg-red" onClick={() => handleEndCall()}>
                End call
            </button>
        </div>
    );
};

export default ControlButtons;
