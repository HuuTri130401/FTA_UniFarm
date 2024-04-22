import { ArrowsPointingInIcon, ArrowsPointingOutIcon } from '@heroicons/react/24/outline';
import { AgoraVideoPlayer, ICameraVideoTrack, ILocalVideoTrack, IRemoteVideoTrack, UID } from 'agora-rtc-react';
import { Tooltip } from 'antd';
import clsx from 'clsx';
import { Monitor, MonitorSmartphone, Smartphone } from 'lucide-react';
import * as React from 'react';

interface VideoPlayerProps {
    videoTrack: ILocalVideoTrack | IRemoteVideoTrack | ICameraVideoTrack | null | undefined;
    uid: UID;
    debug?: boolean;
    setPinnedTrack?: React.Dispatch<React.SetStateAction<ILocalVideoTrack | IRemoteVideoTrack | ICameraVideoTrack | null | undefined>>;
    isCurrentPin?: boolean;
    toggleScreen?: boolean;
}

type DisplayScreen = 'desktop' | 'phone';

const VideoPlayer: React.FunctionComponent<VideoPlayerProps> = ({
    videoTrack,
    uid,
    debug,
    setPinnedTrack = () => {},
    isCurrentPin = false,
    toggleScreen = false,
}) => {
    const handlePin = () => {
        if (!isCurrentPin) {
            setPinnedTrack(videoTrack as any);
        } else {
            setPinnedTrack(null);
        }
    };

    const [screen, setScreen] = React.useState<DisplayScreen>('desktop');

    return (
        <div
            className={clsx('relative  overflow-hidden bg-gray-400 rounded-lg ', {
                'w-full h-auto max-w-[1440px]': screen === 'desktop',
                'w-[400px] h-[720px]': screen === 'phone',
            })}
        >
            {videoTrack && (
                <div className="w-full group">
                    <AgoraVideoPlayer
                        videoTrack={videoTrack}
                        className={clsx('relative z-0 w-full ', {
                            'aspect-video': screen === 'desktop',
                            'h-[720px]': screen === 'phone',
                        })}
                    />
                    <div className="absolute z-10 flex gap-2 right-2 top-2">
                        {toggleScreen && (
                            <Tooltip title={screen === 'desktop' ? 'Switch to phone screen' : 'Switch to desktop screen'}>
                                <div
                                    className={clsx(
                                        ' w-6 h-6 p-0.5 cursor-pointer group-hover:opacity-100 opacity-0 text-gray-800 duration-300 bg-white rounded-lg '
                                    )}
                                    onClick={() => setScreen((prev) => (prev === 'desktop' ? 'phone' : 'desktop'))}
                                >
                                    {screen === 'desktop' ? <Monitor className="w-full h-full" /> : <Smartphone className="w-full h-full" />}
                                </div>
                            </Tooltip>
                        )}

                        <div
                            className={clsx(' w-6 h-6 p-0.5 cursor-pointer group-hover:opacity-100 opacity-0 duration-300 bg-white rounded-lg ', {
                                'text-red': isCurrentPin,
                                'text-gray-800': !isCurrentPin,
                            })}
                            onClick={() => handlePin()}
                        >
                            {isCurrentPin ? <ArrowsPointingInIcon /> : <ArrowsPointingOutIcon />}
                        </div>
                    </div>
                </div>
            )}
            {debug && (
                <div className="absolute top-0 left-0 flex items-center justify-center w-full h-full font-semibold">
                    <p className="p-2 bg-white text-red">{uid}</p>
                </div>
            )}
        </div>
    );
};

export default VideoPlayer;
