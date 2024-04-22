import { interviewApi } from '@core/api/interview.api';
import { InterviewItem } from '@models/interview';
import { UserRole } from '@models/user';
import { useStoreExpert, useStoreUser } from '@store/index';
import { useQuery } from '@tanstack/react-query';
import * as AgoraRTCReact from 'agora-rtc-react';
import {
    createCameraVideoTrack,
    createClient,
    createMicrophoneAudioTrack,
    IAgoraRTCRemoteUser,
    ICameraVideoTrack,
    ILocalVideoTrack,
    IRemoteVideoTrack,
    UID,
} from 'agora-rtc-react';
import AgoraRTCSdk from 'agora-rtc-sdk-ng';
import * as React from 'react';

import ControlButtons from './ControlButtons';
import ExpertActionBar from './ExpertActionBar';
import VideoPlayer from './VideoPlayer';
import VideoShareScreen from './VideoScreenShare';

export interface VideoCallProps {
    channel: string;
    callbackEndCall: () => void;
    interview: InterviewItem;
    defaultMuteCamera: boolean;
    defaultMuteMicrophone: boolean;
    defaultCameraDeviceId?: string;
    defaultMicrophoneDeviceId?: string;
    token: string;
}

const appId: string = process.env.NEXT_PUBLIC_AGORA_ID_APP || '';

const useClient = createClient({ mode: 'rtc', codec: 'vp8' });

type MediaType = 'audio' | 'video';

// const chanel = 'test';

const VideoCall: React.FunctionComponent<VideoCallProps> = ({
    interview,
    channel,
    callbackEndCall,
    defaultMuteCamera,
    defaultMuteMicrophone,
    defaultCameraDeviceId,
    defaultMicrophoneDeviceId,
    token,
}) => {
    const useCameraTrack = createCameraVideoTrack({ cameraId: defaultCameraDeviceId });
    const useMicrophoneTrack = createMicrophoneAudioTrack();

    const camera = useCameraTrack();
    const microphone = useMicrophoneTrack();

    const [users, setUsers] = React.useState<IAgoraRTCRemoteUser[]>([]);

    const [uid, setUid] = React.useState<UID | null>(null);

    const client = useClient();

    React.useEffect(() => {
        AgoraRTCReact.default.setLogLevel(4);
    }, []);

    const [isJoined, setIsJoined] = React.useState<boolean>(false);

    React.useEffect(() => {
        if (!isJoined) return;
        setIsCameraMuted(defaultMuteCamera);
        setIsMicMuted(defaultMuteMicrophone);
    }, [isJoined]);

    const handleJoined = async (user: IAgoraRTCRemoteUser) => {
        setUsers((previousUsers) => [...previousUsers, user]);
        console.log('defaultMuteMicrophone', user);
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
        }
        setUsers(client.remoteUsers.filter((u) => u.uid !== user.uid && Boolean(u.videoTrack)));
    };

    const handleTrackEnded = (mediaType: MediaType) => {
        if (mediaType === 'audio' && microphone.track) {
            microphone.track.stop();
            microphone.track.close();
        }

        if (mediaType === 'video' && camera.track) {
            camera.track.stop();
            camera.track.close();
        }
    };

    // Join channel
    React.useEffect(() => {
        if (!microphone.ready) {
            return;
        }

        client.join(appId, channel, token, null).then((uid) => {
            setUid(uid);
            setIsJoined(true);
            if (microphone.track) {
                client.publish(microphone.track);
            }
        });

        client.on('user-joined', handleJoined);
        client.on('user-left', handleLeft);
        client.on('user-published', handlePublished);
        client.on('user-unpublished', handleUnpublished);

        camera.track?.on('track-ended', () => handleTrackEnded('video'));
        microphone.track?.on('track-ended', () => handleTrackEnded('audio'));

        return () => {
            client.leave();
            client.removeAllListeners();
            client.off('user-joined', handleJoined);
            client.off('user-left', handleLeft);
            client.off('user-published', handlePublished);
            client.off('user-unpublished', handleUnpublished);
            camera.track?.off('track-ended', () => handleTrackEnded('video'));
            microphone.track?.off('track-ended', () => handleTrackEnded('audio'));
        };
    }, [microphone.ready]);

    // Publish camera
    React.useEffect(() => {
        if (!camera.track || !isJoined) {
            return;
        }

        client.publish(camera.track);
    }, [camera.track, isJoined]);

    const [isMicMuted, setIsMicMuted] = React.useState<boolean>(false);
    const [isCameraMuted, setIsCameraMuted] = React.useState<boolean>(false);

    // Mute or unmute
    const handleMute = async (mediaType: MediaType, isMute: boolean) => {
        if (mediaType === 'video' && camera.track) {
            if (isMute) {
                await camera.track.setEnabled(false);
                await client.unpublish(camera.track);
                setIsCameraMuted(true);
            } else {
                await camera.track.setEnabled(true);
                await client.publish(camera.track);
                setIsCameraMuted(false);
            }
        }

        console.log('handleMute', mediaType, isMute);
        if (mediaType === 'audio' && microphone.track) {
            if (isMute) {
                await microphone.track.setEnabled(false);
                await client.unpublish(microphone.track);
                setIsMicMuted(true);
            } else {
                await microphone.track.setEnabled(true);
                await client.publish(microphone.track);
                setIsMicMuted(false);
            }
        }
    };

    // Camera change or unplugged then end call

    AgoraRTCSdk.onCameraChanged = async (info) => {
        if (info.state === 'INACTIVE' && camera.track) {
            handleEndCall();
        }
    };

    // End call
    const handleEndCall = async () => {
        if (camera.track) {
            camera.track.stop();
            camera.track.close();
        }

        if (microphone.track) {
            microphone.track.stop();
            microphone.track.close();
        }

        setIsShareScreen(false);
        callbackEndCall();
        window.location.reload();
    };

    // Select camera not work, then disable it!
    const [cameraAvailable, setCameraAvailable] = React.useState<boolean>(false);

    React.useEffect(() => {
        AgoraRTCSdk.createCameraVideoTrack({ cameraId: defaultCameraDeviceId })
            .then(() => {
                setCameraAvailable(true);
                handleMute('video', false);
            })
            .catch(() => {
                setCameraAvailable(false);
                console.log('Camera not available');
            });
    }, []);

    React.useEffect(() => {
        if (camera.track && defaultCameraDeviceId) {
            camera.track.setDevice(defaultCameraDeviceId);
        }
    }, [defaultCameraDeviceId, camera.track]);

    React.useEffect(() => {
        if (microphone.track && defaultMicrophoneDeviceId) {
            microphone.track.setDevice(defaultMicrophoneDeviceId);
        }
    }, [defaultMicrophoneDeviceId, microphone.track]);

    const [pinnedTrack, setPinnedTrack] = React.useState<ILocalVideoTrack | IRemoteVideoTrack | ICameraVideoTrack | null | undefined>(null);
    const videos = React.useMemo(() => {
        return users.map(
            (user) =>
                user.videoTrack &&
                user.videoTrack !== pinnedTrack && (
                    <VideoPlayer key={user.uid} uid={user.uid} videoTrack={user.videoTrack} setPinnedTrack={setPinnedTrack} />
                )
        );
    }, [users, pinnedTrack]);

    const [isShareScreen, setIsShareScreen] = React.useState<boolean>(false);

    const expert = useStoreExpert();
    const user = useStoreUser();

    const { data: tokenShareScreening, isSuccess } = useQuery<string>(
        ['token', channel],
        async () => {
            const res = await interviewApi.v1AgoraGetToken(channel, 1);
            return res as string;
        },
        { enabled: Boolean(channel) }
    );

    return (
        <div className="absolute top-0 left-0 z-50 flex flex-col w-full h-screen max-h-screen bg-gray-700">
            <div className="flex items-center justify-between flex-shrink-0 w-full h-16 gap-2 px-4 bg-gray-800 border-b border-gray-900 border-solid shadow-lg">
                <div className="flex items-center gap-2 text-white">
                    <p className="m-0 text-lg font-semibold">
                        {interview.jobLevel.job.title} - {interview.jobLevel.title}
                    </p>
                </div>
                {/* control */}
                <ControlButtons
                    cameraAvailable={cameraAvailable}
                    handleEndCall={handleEndCall}
                    isJoined={isJoined}
                    isCameraMuted={isCameraMuted}
                    isMicMuted={isMicMuted}
                    isShareScreen={isShareScreen}
                    handleMuteCamera={() => handleMute('video', !isCameraMuted)}
                    handleMuteMic={() => handleMute('audio', !isMicMuted)}
                    setIsShareScreen={setIsShareScreen}
                />
            </div>
            <div className="flex w-full h-[calc(100vh-64px)]">
                {user.roleName === UserRole.ADMIN && interview.expert.id === expert.id && <ExpertActionBar interview={interview} />}

                <div className="flex w-full h-full gap-3 p-4">
                    <div className="flex items-center justify-center w-full h-full p-3 bg-gray-800 rounded-lg">
                        {Boolean(pinnedTrack) ? (
                            <VideoPlayer
                                toggleScreen={true}
                                isCurrentPin={true}
                                uid={6666666}
                                videoTrack={pinnedTrack}
                                setPinnedTrack={setPinnedTrack}
                            />
                        ) : (
                            <p className="text-2xl font-medium text-white">Pin screen to see it here</p>
                        )}
                    </div>
                    <div className="flex flex-col flex-shrink-0 w-56 h-full gap-3 p-3 bg-gray-800 rounded-lg">
                        {/* local user */}

                        {uid && pinnedTrack !== camera.track && (
                            <div className="flex-shrink-0 w-full">
                                <VideoPlayer uid={uid} videoTrack={camera.track} setPinnedTrack={setPinnedTrack} />
                            </div>
                        )}

                        {/* remote users */}
                        {videos}
                        {/* share screen */}
                        {isShareScreen && (
                            <VideoShareScreen
                                token={tokenShareScreening as string}
                                appId={appId}
                                channel={channel}
                                setEnable={setIsShareScreen}
                                enable={isShareScreen}
                            />
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default VideoCall;
