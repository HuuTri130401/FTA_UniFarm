import React from 'react';

interface DownloadsProps {}

const Downloads: React.FunctionComponent<DownloadsProps> = () => {
    return (
        <>
            <section className="relative py-20 ">
                <picture className="absolute inset-0 pointer-events-none -z-10 dark:hidden">
                    <img src="/images/gradient_light.jpg" alt="gradient" className="w-full h-full" />
                </picture>
                <picture className="absolute inset-0 hidden pointer-events-none -z-10 dark:block">
                    <img src="/images/gradient_dark.jpg" alt="gradient dark" className="w-full h-full" />
                </picture>
                <div className="container">
                    <div className="flex flex-col items-center space-y-10 lg:flex-row lg:space-y-0 lg:space-x-4">
                        <div className="mb-10 lg:order-1 lg:w-2/4 xl:w-[30%]">
                            <div className="text-lg text-center lg:text-left">
                                <h2 className="mb-6 text-xl font-display text-jacarta-700 dark:text-white lg:pr-4">
                                    Tải về ứng dụng FTA
                                    <span className="text-jacarta-600"> Để cập nhật những mặt hàng tươi mới của nông trại</span>
                                </h2>
                                <div className="inline-flex w-full space-x-4">
                                    <a
                                        href="#"
                                        className="flex items-center p-4 font-semibold text-center transition-all bg-white rounded-full group text-jacarta-500 shadow-white-volume hover:bg-primary-800 hover:text-white hover:shadow-accent-volume"
                                    >
                                        <img src="/images/crypto-app/apple_store.png" className="inline-block mr-2" alt="app store" />
                                        App Store
                                    </a>
                                    <a
                                        href="#"
                                        className="flex items-center p-4 font-semibold text-center transition-all bg-white rounded-full text-jacarta-500 shadow-white-volume hover:bg-primary-800 hover:text-white hover:shadow-accent-volume"
                                    >
                                        <img src="/images/crypto-app/play_store.png" className="inline-block mr-2" alt="play store" />
                                        Google play
                                    </a>
                                </div>
                            </div>
                        </div>
                        {/* End :lg=prder-1 */}

                        <div className="order-3 text-center lg:order-2 lg:w-1/4 lg:self-end xl:w-[40%]">
                            <img src="/images/fta/download-screen.png" width={300} className="inline-block" alt="" />
                        </div>
                        {/* mobile app */}

                        <div className="mb-10 hidden lg:order-3 lg:block lg:w-2/4 xl:w-[30%]">
                            <div className="flex items-center space-x-8 lg:pl-6">
                                <div className="inline-block flex-shrink-0 rounded-2.5xl border border-jacarta-100 bg-white p-6">
                                    <img src="/images/qr.png" alt="" />
                                </div>
                                <div className="text-left">
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        viewBox="0 0 24 24"
                                        className="w-6 h-6 mb-2 fill-jacarta-700 dark:fill-white"
                                    >
                                        <g>
                                            <path fill="none" d="M0 0h24v24H0z" />
                                            <path d="M7.828 11H20v2H7.828l5.364 5.364-1.414 1.414L4 12l7.778-7.778 1.414 1.414z" />
                                        </g>
                                    </svg>
                                    <span className="text-lg text-jacarta-700 dark:text-white">Quét để tải ứng dụng FTA</span>
                                </div>
                            </div>
                        </div>
                        {/*scan downaload app */}
                    </div>
                </div>
            </section>
        </>
    );
};

export default Downloads;
