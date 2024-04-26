import { routes } from '@core/routes';
import Link from 'next/link';
import React from 'react';
import { Slide } from 'react-awesome-reveal';

interface HomeScreenProps {}

const Banner: React.FunctionComponent<HomeScreenProps> = () => {
    return (
        <Slide direction="left" triggerOnce>
            <div
                className={`relative -mx-8 bg-center bg-cover bg-[url("https://i.upanh.org/2024/04/17/Untitledb029bb52eb99c350.png")] pt-20 h-[calc(100vh-79px)]`}
            >
                <div className="absolute inset-0 z-10 opacity-25 bg-primary-900"></div>
                <div className="relative z-10 max-w-screen-xl px-4 mx-auto sm:px-8">
                    <div className="flex flex-col items-center justify-around px-4 pt-32 pb-32 lg:flex-row">
                        <div className="flex flex-col items-center lg:block">
                            {/* <span className="inline-block py-1 pl-3 my-4 text-sm font-medium text-gray-100 border-l-4 border-blue-500">
                                We have started operations at the university.
                            </span> */}
                            <h1 className="text-3xl font-black leading-none text-center text-gray-100 lg:text-left sm:text-4xl lg:text-5xl xl:text-6xl">
                                <span className="inline-block mt-2">Quản lí</span>
                                <br />
                                <span className="relative text-primary-900 px-4 -mx-4 py-2 before:content-[''] before:absolute before:inset-0 before:bg-gray-100 before:transform before:-skew-x-12 before:-z-10">
                                    nông sản và phân phối hiệu quả hơn
                                </span>
                            </h1>
                            <Link href={`${routes.auth.login()}`}>
                                <button className="px-8 py-3 mt-10 text-sm font-bold transition duration-300 bg-gray-100 rounded shadow sm:text-base sm:mt-16 sm:px-8 sm:py-4 text-primary-900 hocus:bg-primary-900 hocus:text-gray-100 focus:shadow-outline">
                                    Bắt đầu ngay
                                </button>
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </Slide>
    );
};

export default Banner;
