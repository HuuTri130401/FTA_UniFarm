import { store } from '@store/index';
import { userThunk } from '@store/user/thunks';
import { useMutation } from '@tanstack/react-query';
import { FacebookAuthProvider, GoogleAuthProvider } from 'firebase/auth';
import Image from 'next/image';
import * as React from 'react';
import { toast } from 'react-toastify';

// import { auth } from '../../../../config/firebase';
import { routes } from '@core/routes';
import { useRouter } from 'next/router';
import { authApi } from '../../../core/api';
import { constant } from '../../../core/constant';
import { LoginTokenPayload } from './interface';

interface LoginSocialProps {}

export const LoginSocial: React.FC<LoginSocialProps> = ({}) => {
    const router = useRouter();

    const { isLoading, mutate } = useMutation<any, any, LoginTokenPayload>(async (input) => await authApi.vLoginGoogle(input), {
        onSuccess: async (data) => {
            localStorage.setItem(constant.TOKEN_KEY, data);
            store.dispatch(userThunk.getCurrentUser());
            toast.success('Login success');
        },
    });
    const handleOnSubmit = async (data: LoginTokenPayload) => {
        mutate(data);
    };
    const googleAuth = new GoogleAuthProvider();
    // const handleGoogleLogin = async () => {
    //     try {
    //         const res = await signInWithPopup(auth, googleAuth);
    //         res.user.getIdToken().then((token) => {
    //             const payload: LoginTokenPayload = {
    //                 accessToken: token,
    //             };
    //             handleOnSubmit(payload);
    //         });
    //     } catch (error) {}
    // };

    const facebookAuth = new FacebookAuthProvider();
    const handleFacebookLogin = async () => {};

    return (
        <div className="mt-4 space-y-4">
            <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
                <div className="flex flex-col items-center space-y-4 min-h-[214px] justify-center">
                    <button
                        onClick={() => {
                            router.push(routes.auth.loginSocial());
                        }}
                        className="flex justify-center items-center px-6 py-2.5 text-black font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-primary/90 hover:text-white hover:shadow-lg focus:shadow-lg focus:outline-none focus:ring-0 active:shadow-lg transition duration-150 ease-in-out w-full mb-3 bg-gradient-to-r cursor-pointer gap-2 
                        border border-gray-300"
                    >
                        <Image src="/assets/images/login/gg.webp" alt="image banner" width="20" height="20" />
                        Login With Google
                    </button>
                    {/* <button
                        onClick={handleFacebookLogin}
                        className="flex justify-center items-center px-6 py-2.5 text-black font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-blue hover:text-white hover:shadow-lg focus:shadow-lg focus:outline-none focus:ring-0 active:shadow-lg transition duration-150 ease-in-out w-full mb-3 bg-gradient-to-r cursor-pointer gap-2 dark:text-white dark:bg-blue  border border-gray-300"
                    >
                        <Image src="/assets/images/login/fb.webp" alt="image banner" width="20" height="20" />
                        Login With Facebook
                    </button> */}
                </div>
            </div>
        </div>
    );
};
