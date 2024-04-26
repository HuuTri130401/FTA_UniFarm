import * as React from 'react';
import { useFormContext } from 'react-hook-form';

interface FormYupErrorMessageProps {
    name: string;
}

export const FormYupErrorMessage: React.FC<FormYupErrorMessageProps> = ({ name }) => {
    const { formState } = useFormContext();
    const [errorMessage, setErrorMessage] = React.useState(formState.errors[`${name}`]?.message as string);

    React.useEffect(() => {
        setErrorMessage(formState.errors[`${name}`]?.message as string);
    }, [formState.errors[`${name}`]?.message]);

    return errorMessage ? <p className="text-left text-red-500">{errorMessage}</p> : <></>;
};
