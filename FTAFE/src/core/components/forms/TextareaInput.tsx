import { Input } from 'antd';
import { TextAreaProps } from 'antd/lib/input';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

const { TextArea } = Input;

type TextareaInputType = TextAreaProps & FieldWrapperProps;
interface TextareaInputProps extends TextareaInputType {}

export const TextareaField: React.FC<TextareaInputProps> = ({ name, label, required, isHiddenError, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller control={control} name={name} render={({ field }) => <TextArea {...field} {...rest} />} />
        </FieldWrapper>
    );
};
