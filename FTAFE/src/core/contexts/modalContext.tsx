import * as React from 'react';
import { v4 as uuidv4 } from 'uuid';
export interface IModalItem {
    value: any;
    isOpen: boolean;
    queryKey: string;
}

export interface ModalHookOptions {
    extraCloseAction?: () => void;
}

export interface IModalContext {
    modals: Record<string, IModalItem>;
    handleOpenModal: (key: string, value: any) => void;
    handleCloseModal: (key: string) => void;
    isOpen: (key: string) => boolean;
}

export const ModalContext = React.createContext<IModalContext>({
    modals: {},
    handleOpenModal: () => {},
    handleCloseModal: () => {},
    isOpen: () => false,
});

interface ModalProviderProps {
    children: React.ReactNode;
}

export const ModalProvider: React.FC<ModalProviderProps> = ({ children }) => {
    const [modals, setModals] = React.useState<Record<string, IModalItem>>({});

    const handleOpenModal = (key: string, value: any) => {
        setModals((pre) => ({
            ...pre,
            [key]: {
                isOpen: true,
                value,
                queryKey: uuidv4(),
            },
        }));
    };

    const handleCloseModal = (key: string) => {
        setModals((pre) => ({
            ...pre,
            [key]: {
                isOpen: false,
                value: undefined,
                queryKey: uuidv4(),
            },
        }));
    };

    const isOpen = (key: string) => {
        return modals[key]?.isOpen || false;
    };

    return <ModalContext.Provider value={{ isOpen, handleCloseModal, handleOpenModal, modals }}>{children}</ModalContext.Provider>;
};

export const useModalContext = <T,>(key: string, otp?: ModalHookOptions) => {
    const context = React.useContext(ModalContext);

    const open = (value?: T) => context.handleOpenModal(key, value);
    const close = () => {
        context.handleCloseModal(key);
    };
    const isOpen = () => context.isOpen(key);

    React.useEffect(() => {
        if (!context.isOpen(key)) {
            otp?.extraCloseAction?.();
        }
    }, [context.modals[key], otp]);

    return {
        value: context.modals[key]?.value as T,
        open,
        queryKey: context.modals[key]?.queryKey,
        close,
        isOpen,
    };
};
