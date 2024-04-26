import { Switch, SwitchProps } from 'antd';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type SwitchInputType = SwitchProps & FieldWrapperProps;
interface SwitchInputProps extends SwitchInputType {}

export const SwitchInput: React.FC<SwitchInputProps> = ({ name, label, required, isHiddenError, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller control={control} name={name} render={({ field }) => <Switch {...field} {...rest} checked={field.value} />} />
        </FieldWrapper>
    );
};
