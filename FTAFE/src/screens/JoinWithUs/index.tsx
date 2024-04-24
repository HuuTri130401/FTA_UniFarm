import { routes } from '@core/routes';
import Link from 'next/link';
import React from 'react';
import { Slide } from 'react-awesome-reveal';

interface JoinWithUsProps {}

const JoinWithUs: React.FunctionComponent<JoinWithUsProps> = () => {
    return (
        <Slide direction="up" triggerOnce>
            <div className="relative mb-20 lg:mb-24">
                <div className="max-w-screen-xl py-20 mx-auto lg:py-24">
                    <div className="py-16 lg:py-20 bg-[#d1b1f5] rounded-lg relative">
                        <div className="relative z-10 flex flex-col items-center justify-center px-4 mx-auto text-center sm:px-16 lg:flex-row lg:text-left">
                            <div className="max-w-lg lg:w-1/2">
                                <h6 className="text-5xl font-bold opacity-75 text-primary-800">Interested in LiveCV?</h6>
                                <h5 className="text-5xl font-bold text-primary-900">Join with us now.</h5>
                            </div>
                            <div className="flex flex-col justify-center max-w-lg mt-6 lg:w-1/2 lg:justify-end lg:mt-0 sm:flex-row">
                                <Link href={routes.job.list()}>
                                    <div className="w-full px-6 py-3 mt-4 text-sm font-bold tracking-wide text-gray-100 transition duration-300 border border-transparent rounded shadow sm:w-auto sm:text-base sm:px-8 sm:py-4 lg:px-10 lg:py-5 first:mt-0 sm:mt-0 sm:mr-8 sm:last:mr-0 focus:outline-none focus:shadow-outline hover:text-gray-300 bg-primary-900 hocus:bg-primary-800">
                                        Get Started
                                    </div>
                                </Link>

                                <div className="w-full px-6 py-3 mt-4 text-sm font-bold tracking-wide transition duration-300 bg-gray-100 border border-transparent rounded sm:w-auto sm:text-base sm:px-8 sm:py-4 lg:px-10 lg:py-5 first:mt-0 sm:mt-0 sm:mr-8 sm:last:mr-0 focus:outline-none focus:shadow-outline text-primary-800">
                                    Contact Us
                                </div>
                            </div>
                        </div>
                        <div className="absolute inset-0 overflow-hidden rounded-lg">
                            <div className="absolute bottom-0 left-0 transform -translate-x-20 translate-y-32 w-80 h-80 text-primary-900 opacity-5" />
                            {/* <DecoratorBlob2 /> */}
                            <div className="absolute z-0 w-40 h-40 rounded-full bg-primary-400 -right-20 -top-20"></div>
                            <div className="absolute z-0 w-40 h-40 rounded-full bg-primary-400 -left-20 -bottom-16"></div>
                        </div>
                    </div>
                </div>
            </div>
        </Slide>
    );
};

export default JoinWithUs;
