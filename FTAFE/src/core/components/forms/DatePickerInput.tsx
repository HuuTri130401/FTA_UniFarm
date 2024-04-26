import { DatePicker } from 'antd';
import moment from 'moment';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

interface DatePickerInputProps extends FieldWrapperProps {
    format?: string;
    props?: any;
    onBlur?: (e: any) => void;
}

export const DatePickerInput: React.FC<DatePickerInputProps> = ({
    label,
    name,
    required,
    isHiddenError,
    onBlur,
    format = 'YYYY-MM-DD',
    props = {},
    ...rest
}) => {
    const { control, setValue } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} isHiddenError={isHiddenError} required={required}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => (
                    <DatePicker
                        {...props}
                        className="w-full"
                        format={format}
                        // {...field}
                        allowClear={false}
                        value={moment(field.value)}
                        onChange={(value) => {
                            setValue(name, value?.format('YYYY-MM-DD'));
                        }}
                        onBlur={onBlur}
                        {...rest}
                    />
                )}
            />
        </FieldWrapper>
    );
};
