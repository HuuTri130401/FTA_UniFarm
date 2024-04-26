import { AutoComplete, AutoCompleteProps } from 'antd';
import clsx from 'clsx';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

interface AutoCompleteInputProps extends AutoCompleteProps, FieldWrapperProps {}

export const AutoCompleteInput: React.FC<AutoCompleteInputProps> = ({ name, label, options, className, required, isHiddenError, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => <AutoComplete {...field} className={clsx([className, 'w-full'], {})} options={options} {...rest} />}
            />
        </FieldWrapper>
    );
};
