import { useStoreApi } from '@store/index';
import * as React from 'react';
import { FormProvider, UseFormReturn } from 'react-hook-form';

interface FormWrapperProps {
    methods: UseFormReturn<any, any>;
    children: React.ReactNode;
}

export const FormWrapper: React.FC<FormWrapperProps> = ({ children, methods }) => {
    return <FormProvider {...methods}>{children}</FormProvider>;
};
