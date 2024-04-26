import * as React from 'react';

const namespaces = { default: 'default', headerLoading: 'headerLoading' };

export interface LoadingProps {
    getLoading: (namespace?: string) => boolean;
    handleOnStartLoading: (namespace?: string) => void;
    handleOnEndLoading: (namespace?: string) => void;
    isLoadings: Record<string, boolean>;
    defaultNamespace: Record<'default' | 'headerLoading', string>;
}

export const LoadingContext = React.createContext<LoadingProps>({
    getLoading: () => false,
    handleOnStartLoading: () => {},
    handleOnEndLoading: () => {},
    isLoadings: {},
    defaultNamespace: namespaces,
});

interface LoadingProviderProps {
    children: React.ReactNode;
}

export const LoadingProvider: React.FC<LoadingProviderProps> = ({ children }) => {
    const [isLoadings, setIsLoadings] = React.useState<Record<string, boolean>>({ [namespaces.default]: false, [namespaces.headerLoading]: false });

    const handleOnStartLoading = (namespace = namespaces.default) => {
        setIsLoadings((pre) => ({ ...pre, [namespace]: true }));
    };
    const handleOnEndLoading = (namespace = namespaces.default) => setIsLoadings((pre) => ({ ...pre, [namespace]: false }));
    const getLoading = (namespace = namespaces.default) => isLoadings[namespace] || false;

    return (
        <LoadingContext.Provider value={{ defaultNamespace: namespaces, isLoadings, getLoading, handleOnStartLoading, handleOnEndLoading }}>
            {children}
        </LoadingContext.Provider>
    );
};

export function useLoading() {
    const methods = React.useContext(LoadingContext);

    return {
        ...methods,
    };
}
