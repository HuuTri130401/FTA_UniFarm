import * as React from 'react';

import { useProgressBar } from '../../contexts/progressBarContext';

interface ProgressLoadingBarProps {
    className?: string;
}

export const ProgressLoadingBar: React.FC<ProgressLoadingBarProps> = ({ className = '' }) => {
    const [widthContent, setWidContent] = React.useState<string>('scale-x-0');
    const { isLoading } = useProgressBar();

    React.useEffect(() => {
        if (isLoading) {
            return setWidContent('scale-x-75 duration-[20000ms]');
        }

        if (!isLoading && widthContent !== 'scale-x-0') {
            setWidContent('scale-x-100 duration-[500ms]');
            setTimeout(() => {
                setWidContent('scale-x-0');
            }, 1000);
        }
    }, [isLoading]);

    return (
        <>
            <div className={`origin-left w-full   transform  ${className}  ${widthContent}`}></div>
        </>
    );
};
