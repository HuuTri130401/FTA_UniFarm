import { InputNumber, InputNumberProps } from 'antd';
import _clsx, { clsx } from 'clsx';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type NumberInputType = InputNumberProps & FieldWrapperProps;
interface NumberInputProps extends NumberInputType {
    name: string;
    label: string;
    required?: boolean;
}

export const NumberInput: React.FC<NumberInputProps> = ({ name, label, required, className, isHiddenError, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => <InputNumber {...field} {...rest} className={clsx('!w-full', className)} />}
            />
        </FieldWrapper>
    );
};
