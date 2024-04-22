import { TransactionStatus, TransactionType } from '@models/transaction';
import { Tag } from 'antd';
import * as React from 'react';

interface TransactionStatusTagProps {
    value: TransactionStatus;
}

const TransactionStatusTag: React.FunctionComponent<TransactionStatusTagProps> = ({ value }) => {
    const handleColor = (status: TransactionStatus) => {
        switch (status) {
            case TransactionStatus.PENDING:
                return 'blue';
            case TransactionStatus.SUCCESS:
                return 'green';
            default:
                return 'gray';
        }
    };

    return <Tag color={handleColor(value)}>{value}</Tag>;
};

export default TransactionStatusTag;
