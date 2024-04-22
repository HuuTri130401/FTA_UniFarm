import _clsx from 'clsx';
import * as React from 'react';
import CurrencyInput, { CurrencyInputProps } from 'react-currency-input-field';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';
import { FormYupErrorMessage } from './FormYupErrorMessage';

type NumberFormatInputType = CurrencyInputProps & FieldWrapperProps;
interface NumberFormatInputProps extends NumberFormatInputType {
    name: string;
    label: string;
    isHiddenLabel?: boolean;
}

export const NumberFormatInput: React.FC<NumberFormatInputProps> = ({
    name,
    label,
    type = 'text',
    isHiddenError,
    required,
    isHiddenLabel = false,
    ...rest
}) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} isHiddenError={isHiddenError} required={required}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => (
                    <CurrencyInput
                        value={field.value}
                        {...rest}
                        onValueChange={(value, name) => {
                            field.onChange(value);
                        }}
                        type={type}
                        className={_clsx('ant-input')}
                    />
                )}
            />
        </FieldWrapper>
    );
};
