import VideoPlayer from '@components/video/VideoPlayer';
import { ClientConfig, createClient, IAgoraRTCRemoteUser } from 'agora-rtc-react';
import clsx from 'clsx';
import * as React from 'react';

export interface VideoRecordingProps {
    channel: string;
    token: string;
}

const appId: string = process.env.NEXT_PUBLIC_AGORA_ID_APP || '';
type MediaType = 'audio' | 'video';

const config: ClientConfig = { mode: 'rtc', codec: 'vp8', role: 'audience' };

const useClient = createClient(config);

const VideoRecording: React.FunctionComponent<VideoRecordingProps> = ({ channel, token }) => {
    const client = useClient();
    const [users, setUsers] = React.useState<IAgoraRTCRemoteUser[]>([]);

    const handleJoined = async (user: IAgoraRTCRemoteUser) => {
        setUsers((previousUsers) => [...previousUsers, user]);
    };

    const handleLeft = (user: IAgoraRTCRemoteUser) => {
        setUsers((prevUsers) => {
            return prevUsers.filter((User) => User.uid !== user.uid);
        });
    };

    const handlePublished = async (user: IAgoraRTCRemoteUser, mediaType: MediaType) => {
        await client.subscribe(user, mediaType);

        if (mediaType === 'audio') {
            user.audioTrack?.play();
        }

        if (mediaType === 'video') {
            setTimeout(() => {
                setUsers(client.remoteUsers);
            }, 1000);
        }
    };

    const handleUnpublished = async (user: IAgoraRTCRemoteUser, mediaType: MediaType) => {
        if (mediaType === 'audio') {
            user.audioTrack?.stop();
        }

        if (mediaType === 'video') {
            user.videoTrack?.stop();
            setUsers(client.remoteUsers.filter((u) => u.uid !== user.uid && Boolean(u.videoTrack)));
        }
    };

    React.useEffect(() => {
        client.join(appId, channel, token, null);

        client.on('user-joined', handleJoined);
        client.on('user-left', handleLeft);
        client.on('user-published', handlePublished);
        client.on('user-unpublished', handleUnpublished);

        return () => {
            client.leave();
            client.removeAllListeners();
            client.off('user-joined', handleJoined);
            client.off('user-left', handleLeft);
            client.off('user-published', handlePublished);
            client.off('user-unpublished', handleUnpublished);
        };
    }, [channel]);

    const videos = React.useMemo(() => {
        return users.map(
            (user) =>
                user.videoTrack && (
                    <div className="flex items-center justify-center grid-cols-1">
                        <VideoPlayer key={user.uid} uid={user.uid} videoTrack={user.videoTrack} setPinnedTrack={() => {}} />
                    </div>
                )
        );
    }, [users]);

    return (
        <div className="absolute top-0 left-0 z-50 flex flex-col w-full h-screen max-h-screen bg-gray-700">
            <div className="flex w-full h-screen">
                <div className="flex w-full h-full gap-3 p-4">
                    <div
                        className={clsx('grid flex-shrink-0 w-full h-full grid-cols-1 gap-3 p-3 bg-gray-800 rounded-lg', {
                            'grid-cols-1': users.length === 1,
                            'grid-cols-2': users.length <= 4 && users.length > 1,
                            'grid-cols-3': users.length <= 9 && users.length > 4,
                            'grid-cols-4': users.length <= 16 && users.length > 9,
                        })}
                    >
                        {videos}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default VideoRecording;
