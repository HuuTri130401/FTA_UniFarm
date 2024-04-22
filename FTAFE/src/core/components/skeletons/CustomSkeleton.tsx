import { Skeleton } from 'antd';
import * as React from 'react';

interface CustomSkeletonProps extends React.PropsWithChildren {
    isFetched: boolean;
}

const CustomSkeleton: React.FunctionComponent<CustomSkeletonProps> = ({ isFetched, children }) => {
    return !isFetched ? (
        <Skeleton active paragraph={{ rows: 10 }} title={{ width: '100%' }} avatar={{ shape: 'circle', size: 'large' }} />
    ) : (
        <>{children}</>
    );
};

export default CustomSkeleton;
