import { store } from '@store/index';
import { userThunk } from '@store/user/thunks';
import * as React from 'react';

interface GetCurrentUserWrapperProps extends React.PropsWithChildren {}

const GetCurrentUserWrapper: React.FunctionComponent<GetCurrentUserWrapperProps> = ({ children }) => {
    React.useEffect(() => {
        store.dispatch(userThunk.getCurrentUser());
    }, []);

    return <>{children}</>;
};

export default GetCurrentUserWrapper;
