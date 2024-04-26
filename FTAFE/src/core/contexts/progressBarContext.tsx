import * as React from 'react';

export interface IProgressBarProps {
    isLoading: boolean;
    handleSetLoading: (isLoading: boolean) => void;
}

export const progressBarContext = React.createContext<IProgressBarProps>({
    isLoading: false,
    handleSetLoading: () => {},
});

export interface IProgressBarProviderProps extends React.PropsWithChildren {}
export const ProgressBarProvider: React.FC<IProgressBarProviderProps> = ({ children }) => {
    const [isLoading, setIsLoading] = React.useState<boolean>(false);

    const handleSetLoading = (isLoading: boolean) => setIsLoading(isLoading);

    return <progressBarContext.Provider value={{ isLoading, handleSetLoading }}>{children}</progressBarContext.Provider>;
};

export const useProgressBar = (watcher?: boolean) => {
    const methods = React.useContext(progressBarContext);

    React.useEffect(() => {
        if (watcher !== undefined) {
            methods.handleSetLoading(watcher);
        }
    }, [watcher]);

    return { ...methods };
};
