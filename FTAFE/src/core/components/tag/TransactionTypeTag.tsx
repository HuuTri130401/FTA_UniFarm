import { TransactionType } from '@models/transaction';
import { Tag } from 'antd';
import * as React from 'react';

interface TransactionTypeTagProps {
    value: TransactionType;
}

const TransactionTypeTag: React.FunctionComponent<TransactionTypeTagProps> = ({ value }) => {
    const handleColor = (status: TransactionType) => {
        switch (status) {
            case TransactionType.DEPOSIT:
                return 'green';
            case TransactionType.PAYMENT:
                return 'blue';
            case TransactionType.REFUND:
                return 'green';
            case TransactionType.WITHDRAW:
                return 'red';
            default:
                return 'gray';
        }
    };

    return <Tag color={handleColor(value)}>{value}</Tag>;
};

export default TransactionTypeTag;
