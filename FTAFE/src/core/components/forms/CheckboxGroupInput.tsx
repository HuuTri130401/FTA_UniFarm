import { Checkbox } from 'antd';
import { CheckboxGroupProps } from 'antd/lib/checkbox';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type CheckboxGroupOptionType = CheckboxGroupProps & FieldWrapperProps;

interface CheckboxGroupInputProps extends CheckboxGroupOptionType {}

export const CheckboxGroupInput: React.FC<CheckboxGroupInputProps & React.PropsWithChildren> = ({
    children,
    name,
    label,
    required,
    isHiddenError,
    ...rest
}) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => (
                    <Checkbox.Group
                        {...field}
                        onChange={(checkedValue) => {
                            field.onChange(checkedValue);
                            console.log(checkedValue);
                        }}
                        {...rest}
                        className="w-full"
                    >
                        <div className="w-full">{children}</div>
                    </Checkbox.Group>
                )}
            />
        </FieldWrapper>
    );
};
