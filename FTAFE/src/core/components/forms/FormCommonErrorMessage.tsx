import { useStoreApi } from '@store/index';
import * as React from 'react';

interface FormCommonErrorMessageProps {}

const FormCommonErrorMessage: React.FunctionComponent<FormCommonErrorMessageProps> = () => {
    const { errorMessage } = useStoreApi();
    return <p className="m-0 mt-2 text-sm text-red">{errorMessage}</p>;
};

export default FormCommonErrorMessage;
