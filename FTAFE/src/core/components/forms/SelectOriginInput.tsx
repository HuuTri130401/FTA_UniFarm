import { Select, SelectProps } from 'antd';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type SelectOriginInputType = SelectProps & FieldWrapperProps;
interface SelectOriginInputProps extends SelectOriginInputType {}

export const SelectOriginInput: React.FC<SelectOriginInputProps> = ({ name, label, required, isHiddenError, onChange: customChange, ...rest }) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller
                control={control}
                name={name}
                render={({ field: { onChange, ...field } }) => (
                    <Select
                        {...field}
                        {...rest}
                        onChange={(...props) => {
                            onChange(...props);
                            customChange?.(...props);
                        }}
                    />
                )}
            />
        </FieldWrapper>
    );
};
