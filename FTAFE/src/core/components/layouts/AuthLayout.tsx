import Image from 'next/image';
import Link from 'next/link';
import * as React from 'react';

import { routes } from '../../routes';

interface AuthLayoutProps {
    children: React.ReactNode;
}

export const AuthLayout: React.FC<AuthLayoutProps> = ({ children }) => {
    return (
        <div className="flex min-h-screen ">
            <div className="flex flex-col items-center justify-center w-full">
                {/* <div className="z-10 w-full max-w-sm px-6 py-8 pb-16 bg-white rounded-lg shadow-lg fade-in">
                    <div className="mb-8 text-center">
                        <Link href={routes.home()}>
                            <a>
                                <Image width={64} height={64} src="/assets/images/logo-bg.png" alt="tripper" />
                            </a>
                        </Link>
                    </div> */}
                {children}
                {/* </div> */}

                {/* <div className="absolute z-0 w-full h-full">
                    <div className="absolute z-10 w-full h-full bg-gray-800 bg-opacity-30"></div>
                    <img src="/assets/images/login-bg.jpg" className="object-cover w-full h-full " />
                </div> */}
            </div>
        </div>
    );
};
