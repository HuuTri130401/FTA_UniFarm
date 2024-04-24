import React from 'react';

import { FormErrorMessage } from './FormErrorMessage';

interface FieldsWrapperProps {
    label: string;
    fields: { name: string; label: string }[];
    children: React.ReactNode;
}

export const FieldsWrapper: React.FC<FieldsWrapperProps> = ({ label, fields, children }) => {
    return (
        <div className="space-y-2">
            {Boolean(label) && <label className="block text-sm font-medium text-gray-700">{label}</label>}
            {children}
            {fields.map((field) => (
                <FormErrorMessage key={field.name} className="text-sm text-red-500" name={field.name} label={field.label} />
            ))}
        </div>
    );
};
