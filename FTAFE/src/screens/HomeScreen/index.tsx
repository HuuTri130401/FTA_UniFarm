import { routes } from '@core/routes';
import Link from 'next/link';
import React from 'react';

interface HomeScreenProps {}

const HomeScreen: React.FunctionComponent<HomeScreenProps> = () => {
    return (
        <section className="relative pt-20 pb-10 md:pt-32 h-1527">
            <picture className="absolute inset-x-0 top-0 block h-full pointer-events-none -z-10 dark:hidden">
                <img src="/images/gradient.jpg" alt="gradient" className="w-full h-full" />
            </picture>
            <picture className="absolute inset-x-0 top-0 hidden pointer-events-none -z-10 dark:block">
                <img src="/images/gradient_dark.jpg" alt="gradient dark" className="w-full h-full" />
            </picture>

            <div className="container h-full mx-auto">
                <div className="grid items-center h-full gap-4 md:grid-cols-12">
                    <div className="flex flex-col items-center justify-center h-full col-span-6 py-10 md:items-start md:py-20 xl:col-span-4">
                        <h1 className="mb-6 text-5xl font-bold text-center text-jacarta-700 font-display dark:text-white md:text-left lg:text-6xl xl:text-7xl">
                            Empower Your Success!
                        </h1>
                        <p className="mb-8 text-lg text-center dark:text-jacarta-200 md:text-left">
                            Unlock Your Potential with Web Interview - Your Gateway to Success!
                        </p>
                        <div className="flex space-x-4">
                            <Link href={`${routes.job.list()}`}>
                                <a className="px-8 py-3 font-semibold text-center text-white transition-all rounded-full bg-accent shadow-accent-volume hover:bg-accent-dark w-36 hover:text-white">
                                    Book Now
                                </a>
                            </Link>
                            <Link href="/collection/explore_collection">
                                <a className="px-8 py-3 font-semibold text-center transition-all bg-white rounded-full text-accent shadow-white-volume hover:bg-accent-dark hover:shadow-accent-volume w-36 hover:text-white">
                                    Explore
                                </a>
                            </Link>
                        </div>
                    </div>

                    {/* <!-- Hero image --> */}
                    <div className="col-span-6 xl:col-span-8">
                        <div className="relative text-center md:pl-8 md:text-right">
                            <img
                                src="/images/hero/hero.jpg"
                                alt=""
                                className="hero-img mt-8 inline-block w-72 rotate-[8deg] sm:w-full lg:w-[24rem] xl:w-[35rem]"
                            />
                            <img src="/images/hero/3D_elements.png" alt="" className="animate-fly absolute top-0 md:-right-[10%]" />
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default HomeScreen;
