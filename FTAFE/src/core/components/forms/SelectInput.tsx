import { Select, SelectProps } from 'antd';
import clsx from 'clsx';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type SelectInputType = SelectProps & FieldWrapperProps;

interface SelectInputProps extends SelectInputType {
    options: Array<{ label: string | React.ReactNode; value: any; origin: any }>;
}

export const SelectInput: React.FC<SelectInputProps> = ({
    name,
    label,
    required,
    options,
    mode,
    defaultValue,
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
        // }, [defaultValue, mode, name, options, setValue]);
    }, [defaultValue, mode]);

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
