import { Select, SelectProps } from 'antd';
import clsx from 'clsx';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type SelectInputType = SelectProps & FieldWrapperProps;

interface SelectInputProps extends SelectInputType {
    name: string;
    label: string;
    options: Array<{ label: string; value: any; origin: any }>;
}

export const SelectInput: React.FC<SelectInputProps> = ({
    name,
    label,
    options,
    mode,
    defaultValue,
    required,
    className,
    isHiddenError,
    ...rest
}) => {
    const { control, setValue } = useFormContext();

    React.useEffect(() => {
        if (mode !== 'multiple') {
            if (defaultValue) {
                setValue(name, defaultValue);
            } else if (options && options.length) {
                setValue(name, options[0].value);
            }
        }
    }, []);

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => <Select {...field} className={clsx([className, 'w-full'], {})} mode={mode} options={options} {...rest} />}
            />
        </FieldWrapper>
    );
};
