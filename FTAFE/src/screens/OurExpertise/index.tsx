import { BriefcaseIcon, CurrencyDollarIcon } from '@heroicons/react/24/solid';
import React from 'react';
import { Slide } from 'react-awesome-reveal';

interface OurExpertiseProps {}
const features = [
    {
        Icon: BriefcaseIcon,
        title: 'Professionalism',
        description: 'We have the best professional marketing people across the globe just to work with you.',
    },
    {
        Icon: CurrencyDollarIcon,
        title: 'Affordable',
        description: 'We promise to offer you the best rate we can - at par with the industry standard.',
    },
];
const OurExpertise: React.FunctionComponent<OurExpertiseProps> = () => {
    return (
        <Slide direction="right" triggerOnce>
            <div className="relative px-20">
                <div className="flex flex-col md:flex-row justify-between max-w-screen-xl mx-auto py-20 md:py-24">
                    <div className="w-full max-w-md mx-auto md:max-w-none md:mx-0 md:w-5/12 flex-shrink-0 h-80 md:h-auto">
                        <div
                            className={`
                    bg-[url("http://cutting-edge-agency.com/assets/images/team/devs.svg")] rounded bg-contain bg-no-repeat bg-center h-full`}
                        ></div>
                    </div>
                    <div className="w-full max-w-md mx-auto md:max-w-none md:mx-0 md:w-7/12 mt-16 md:mt-0 md:mr-12 lg:mr-16 md:order-first">
                        <div className="lg:py-8 text-center md:text-left">
                            <h4 className="font-bold text-primary-900 mb-4 text-center lg:text-left text-lg">Our Expertise</h4>
                            <h2 className="tracking-wide mt-4 font-black text-3xl sm:text-4xl lg:text-5xl text-center md:text-left leading-tight">
                                We have the most <span className="text-primary-900">professional experts.</span>
                            </h2>
                            <p className="mt-4 text-center md:text-left text-sm md:text-base lg:text-lg font-medium leading-relaxed text-secondary-100 text-gray-500">
                                Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna
                                aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                            </p>
                            <div className="mt-8 max-w-sm mx-auto md:mx-0">
                                {features.map((feature, index) => (
                                    <div className="mt-8 flex items-start flex-col md:flex-row" key={index}>
                                        <div className="mx-auto inline-block border border-primary-900 text-center rounded-full p-2 flex-shrink-0">
                                            {<feature.Icon className="w-4 h-4 text-primary-900" />}
                                        </div>
                                        <div className="mt-4 md:mt-0 md:ml-4 text-center md:text-left">
                                            <div className="font-bold text-lg text-primary-900">{feature.title}</div>
                                            <div className="mt-1 text-sm">{feature.description}</div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <button className="px-8 py-3 mt-10 font-bold rounded bg-primary-900 text-gray-100 hocus:bg-primary-700 hocus:text-gray-200 focus:shadow-outline focus:outline-none transition duration-300">
                                See Our Portfolio
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </Slide>
    );
};

export default OurExpertise;
