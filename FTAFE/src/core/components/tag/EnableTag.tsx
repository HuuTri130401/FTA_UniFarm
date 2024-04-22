import { Tag } from 'antd';
import * as React from 'react';

interface EnableTagProps {
    value: boolean;
}

const EnableTag: React.FunctionComponent<EnableTagProps> = ({ value }) => {
    return <Tag color={value ? 'green' : 'red'}>{value ? 'Enable' : 'Disable'}</Tag>;
};

export default EnableTag;
