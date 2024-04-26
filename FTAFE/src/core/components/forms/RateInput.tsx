import { Rate, RateProps } from 'antd';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type RateInputType = RateProps & FieldWrapperProps;

interface RateInputProps extends RateInputType {}

const RateInput: React.FunctionComponent<RateInputProps> = ({ label, name, required, isHiddenError, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller control={control} name={name} render={({ field }) => <Rate {...field} {...rest} />} />
        </FieldWrapper>
    );
};

export default RateInput;
