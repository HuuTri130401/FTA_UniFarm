import { Input, InputProps } from 'antd';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type TextInputType = InputProps & FieldWrapperProps;

interface TextInputProps extends TextInputType {}

export const TextInput: React.FC<TextInputProps> = ({ name, label, required, isHiddenError, type = 'text', ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller control={control} name={name} render={({ field }) => <Input {...field} {...rest} type={type} />} />
        </FieldWrapper>
    );
};
