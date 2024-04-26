import { Input, InputProps } from 'antd';
import _clsx from 'clsx';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type PasswordInputType = InputProps & FieldWrapperProps;
interface PasswordInputProps extends PasswordInputType {
    name: string;
    label: string;
}

export const PasswordInput: React.FC<PasswordInputProps> = ({ name, required, label, type = 'text', isHiddenError, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller control={control} name={name} render={({ field }) => <Input.Password {...field} {...rest} />} />
        </FieldWrapper>
    );
};
