import { interviewApi } from '@core/api/interview.api';
import { useQuery } from '@tanstack/react-query';
import AgoraRTCSdk, { ILocalVideoTrack } from 'agora-rtc-sdk-ng';
import * as React from 'react';

interface VideoShareScreenProps {
    appId: string;
    channel: string;
    setEnable: React.Dispatch<React.SetStateAction<boolean>>;
    enable: boolean;
    token: string;
}

const streamClient = AgoraRTCSdk.createClient({ mode: 'rtc', codec: 'vp8', role: 'host' });
AgoraRTCSdk.setLogLevel(4);
const VideoShareScreen: React.FunctionComponent<VideoShareScreenProps> = ({ appId, channel, setEnable, enable, token }) => {
    const [track, setTrack] = React.useState<ILocalVideoTrack | null>(null);

    const [isJoin, setIsJoin] = React.useState(false);

    React.useEffect(() => {
        if (enable && token && !isJoin) {
            setIsJoin(true);
            streamClient.join(appId, channel, token, null).then((uid) => {
                AgoraRTCSdk.createScreenVideoTrack({
                    optimizationMode: 'detail',
                })
                    .then((track) => {
                        streamClient.publish(track);
                        if (Array.isArray(track)) {
                            setTrack(track[0]);
                        } else {
                            setTrack(track);
                        }
                    })
                    .catch((err) => {
                        console.log(err);
                    });
            });
        }
    }, [appId, channel, enable, token]);

    const handleLeave = async () => {
        track?.close();
        await streamClient.leave();
        await streamClient.removeAllListeners();

        setIsJoin(false);
        setEnable(false);
    };

    track?.on('track-ended', () => {
        handleLeave();
    });

    React.useEffect(() => {
        return () => {
            handleLeave();
        };
    }, []);

    React.useEffect(() => {
        if (!enable) {
            handleLeave();
        }
    }, [enable]);

    return <></>;
};

export default VideoShareScreen;
