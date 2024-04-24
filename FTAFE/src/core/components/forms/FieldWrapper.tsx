import React from 'react';

import { FormErrorMessage } from './FormErrorMessage';

export interface FieldWrapperProps {
    name: string;
    label?: string | React.ReactNode;
    required?: boolean;
    isHiddenError?: boolean;
}

export const FieldWrapper: React.FC<FieldWrapperProps & React.PropsWithChildren> = ({
    label = '',
    name,
    children,
    required = false,
    isHiddenError = false,
}) => {
    return (
        <div className="w-full space-y-2">
            {Boolean(label) && (
                <label htmlFor={name} className="block text-sm font-medium text-gray-700">
                    {label}
                    {required && <span className="text-red"> *</span>}
                </label>
            )}
            {children}
            {!isHiddenError && <FormErrorMessage className="text-sm text-red" name={name} label={label} />}
        </div>
    );
};
