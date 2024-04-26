import { TimePicker, TimePickerProps } from 'antd';
import moment, { Moment } from 'moment';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type TimePickerInputType = TimePickerProps & FieldWrapperProps;
interface TimePickerInputProps extends TimePickerInputType {
    format?: string;
}

export const TimePickerInput: React.FC<TimePickerInputProps> = ({ label, name, isHiddenError, format = 'HH:mm', required, ...rest }) => {
    const { control, setValue } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => (
                    <TimePicker
                        className="w-full"
                        {...field}
                        value={moment(field.value, 'HH:mm')}
                        format={format}
                        onChange={(value) => setValue(name, value?.format('HH:mm'))}
                        {...rest}
                    />
                )}
            />
        </FieldWrapper>
    );
};
