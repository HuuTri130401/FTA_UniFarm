import { FormWrapper, PasswordInput, TextInput } from '@components/forms';
//import { IV1UserCreateExpert } from '@core/api';
// import { expertApi, IV1AdminAddExpert } from '@core/api/expert.api';
//import { useMutationUserCreateExpert } from '@hooks/api/user.hook';
import { Button } from 'antd';
import { useRouter } from 'next/router';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface AddExpertProps {}

export interface IV1UserCreateExpert {
    email: string;
    fullName: string;
    password: string;
}

const defaultValues: IV1UserCreateExpert = {
    email: '',
    fullName: '',
    password: '',
};

// useMutationUserCreateExpert
const useMutationUserCreateExpert = () => {
    const [isLoading, setIsLoading] = React.useState(false);

    const mutateCreateExpert = async (data: IV1UserCreateExpert, options?: any) => {
        setIsLoading(true);
        try {
            // const res = await expertApi.addExpert(data);
            // if (options?.onSuccess) {
            //     options.onSuccess(res);
            // }
            toast.success('Add expert successfully');
        } catch (error) {
            toast.error('Add expert failed');
        } finally {
            setIsLoading(false);
        }
    };

    return { mutateCreateExpert, isLoading };
};

const AddExpert: React.FunctionComponent<AddExpertProps> = () => {
    const methods = useForm<IV1UserCreateExpert>({ defaultValues });

    const router = useRouter();

    const { mutateCreateExpert, isLoading } = useMutationUserCreateExpert();

    const onSubmit = (data: IV1UserCreateExpert) => {
        mutateCreateExpert(data, {
            // onSuccess: (res) => {
            //     toast.success('Add expert successfully');
            //     methods.reset();
            //     // router.push(routes.admin.user.expert.list());
            // },
        });
    };

    return (
        <div className="max-w-3xl">
            <FormWrapper methods={methods}>
                <form
                    onSubmit={methods.handleSubmit(onSubmit)}
                    className="col-span-2 p-10 space-y-10 bg-white border-2 border-gray-200 divide-y rounded-lg shadow-lg divide-gray-900/10"
                >
                    <div className="grid w-full grid-cols-2 gap-2">
                        <div className="col-span-1 px-4 sm:px-0">
                            <h2 className="text-base font-semibold leading-7 text-gray-900">Information User</h2>
                            <p className="mt-1 text-sm leading-6 text-gray-600">
                                This information will be used by experts to log in as well as identify them in the system.
                            </p>
                        </div>
                        <div className="col-span-1 shadow-sm bg-gray-50 ring-1 ring-gray-900/5 sm:rounded-xl">
                            <div className="px-4 py-6 sm:p-8">
                                <div className="flex flex-col w-full gap-4">
                                    <div className="sm:col-span-6">
                                        <TextInput name="fullName" label="Fullname" />
                                    </div>
                                    <div className="sm:col-span-6">
                                        <TextInput name="email" label="Email" />
                                    </div>

                                    <div className="sm:col-span-6">
                                        <PasswordInput name="password" label="Password" />
                                    </div>
                                    <div className="flex justify-end sm:col-span-6">
                                        <Button type="primary" htmlType="submit" disabled={isLoading}>
                                            Create
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </FormWrapper>
        </div>
    );
};

export default AddExpert;
