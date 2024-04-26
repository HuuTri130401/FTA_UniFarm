import * as React from 'react';

import { store } from '../../store';
import { userActions } from '../../store/user';

interface AuthLoginWrapperProps {
    children: React.ReactNode;
}

export const AutoLoginWrapper: React.FC<AuthLoginWrapperProps> = ({ children }) => {
    React.useEffect(() => {
        store.dispatch(userActions.autoLogin());
    }, []);

    return <>{children}</>;
};
