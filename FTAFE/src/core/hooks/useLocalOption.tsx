import { IOption } from '@models/interface';
import * as React from 'react';

export enum SelectLocalOption {
    TOP_DRIVER,
}

export const useLocalSelectOption = (option: SelectLocalOption, value?: any) => {
    const [options, setOptions] = React.useState<IOption[]>([]);

    React.useEffect(() => {
        setOptions([
            {
                label: 'Trong tháng',
                value: 0,
                origin: 'month',
            },
            {
                label: 'Trong 2 tháng',
                value: 2,
                origin: 'month',
            },
            {
                label: 'Trong 6 tháng',
                value: 6,
                origin: 'month',
            },
        ]);
    }, [option, value]);

    return options;
};
