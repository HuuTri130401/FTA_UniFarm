import { useTableUtil } from '@context/tableUtilContext';
import { Button } from 'antd';
import * as React from 'react';
import { useForm, UseFormReturn } from 'react-hook-form';

import { FormWrapper } from './FormWrapper';

interface FormFilterWrapperProps<T = any> {
    defaultValues?: T | Record<string, any>;
    filterButton?: React.ReactNode;
    children: React.ReactNode | ((methods: UseFormReturn, submit: () => void) => React.ReactNode);
}

const FormFilterWrapper = <T,>({ defaultValues, children, filterButton }: FormFilterWrapperProps<T>) => {
    const methods = useForm<T | any>({ defaultValues });

    const { handleChangeFilter } = useTableUtil();

    const handleOnSearch = (data: T | Record<string, any>) => {
        handleChangeFilter(data as Record<string, any>);
    };

    return (
        <FormWrapper methods={methods}>
            <form onSubmit={methods.handleSubmit(handleOnSearch)} className="grid w-full grid-cols-11 ">
                <div className="flex flex-wrap items-end w-full col-span-10 gap-2">
                    {typeof children === 'function'
                        ? children(methods, () => {
                              methods.handleSubmit(handleOnSearch)();
                          })
                        : children}
                </div>
                <div className="flex items-end justify-end col-span-1">
                    {filterButton ? (
                        filterButton
                    ) : (
                        <Button htmlType="submit" size="middle" type="primary">
                            Lọc
                        </Button>
                    )}
                </div>
            </form>
        </FormWrapper>
    );
};

export default FormFilterWrapper;
