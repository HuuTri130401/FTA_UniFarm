import * as React from 'react';
interface Timer {
    hours: string;
    minutes: string;
    seconds: string;
}

export interface UserCountDownProps {
    endTime: Date;
}

export const useCountDown = ({ endTime }: UserCountDownProps) => {
    const [openLinkTimer, setOpenLinkTimer] = React.useState<Timer>({ hours: '0', minutes: '0', seconds: '0' });

    React.useEffect(() => {
        const intervalId = setInterval(() => {
            const countDownDate = new Date(endTime).getTime();
            const now = new Date().getTime();

            const distance1 = countDownDate - now;

            setOpenLinkTimer({
                hours: String(Math.floor(distance1 / (1000 * 60 * 60))).padStart(2, '0'),
                minutes: String(Math.floor((distance1 % (1000 * 60 * 60)) / (1000 * 60))).padStart(2, '0'),
                seconds: String(Math.floor((distance1 % (1000 * 60)) / 1000)).padStart(2, '0'),
            });
        }, 700);
        return () => {
            clearInterval(intervalId);
        };
    }, [endTime]);

    return { ...openLinkTimer };
};
