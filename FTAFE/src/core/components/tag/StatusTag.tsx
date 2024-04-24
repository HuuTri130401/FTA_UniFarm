import { IOption } from '@models/interface';
import { Tag } from 'antd';
import _get from 'lodash/get';
import React, { useEffect } from 'react';

interface StatusTagProps {
    data: any;
    options: IOption[];
}

export const StatusTag: React.FC<StatusTagProps> = ({ data = '', options }) => {
    const [option, setOption] = React.useState<IOption>({
        label: 'Trống',
        value: '',
        origin: 'yellow',
    });

    useEffect(() => {
        const _option = options.find((item) => item.value.toUpperCase() === data.toString().toUpperCase());

        if (_option) {
            setOption(_option);
        }
    }, [data, options]);

    return <Tag color={option.origin}>{option.label}</Tag>;
};
