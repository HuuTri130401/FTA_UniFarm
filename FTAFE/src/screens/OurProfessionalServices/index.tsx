import Image from 'next/image';
import React from 'react';
import { Slide } from 'react-awesome-reveal';

import CustomizeIconImage from '../../../public/images/customize-icon.svg';
import FastIconImage from '../../../public/images/fast-icon.svg';
import ReliableIconImage from '../../../public/images/reliable-icon.svg';
import defaultCardImage from '../../../public/images/shield-icon.svg';
import SimpleIconImage from '../../../public/images/simple-icon.svg';
import SupportIconImage from '../../../public/images/support-icon.svg';
interface OurProfessionalServicesProps {}
const cards = [
    {
        imageSrc: defaultCardImage,
        title: 'Ads Management',
        description: 'We create and manage ads that you need, from creation to deployment. Lorem ipsum donor sit amet consicou.',
    },
    { imageSrc: SupportIconImage, title: 'Video Marketing' },
    { imageSrc: CustomizeIconImage, title: 'Customer Relation' },
    { imageSrc: ReliableIconImage, title: 'Product Outreach' },
    { imageSrc: FastIconImage, title: 'PR Campaign' },
    { imageSrc: SimpleIconImage, title: 'Product Expansion' },
];
const OurProfessionalServices: React.FunctionComponent<OurProfessionalServicesProps> = () => {
    return (
        <Slide direction="left" triggerOnce>
            <div className="relative">
                <div className="flex flex-col items-center md:items-stretch md:flex-row flex-wrap md:justify-center max-w-screen-xl mx-auto py-20 md:py-24">
                    <h4 className="font-bold text-primary-900 mb-4 text-center lg:text-left text-lg">Services</h4>
                    <h2 className="text-4xl sm:text-5xl font-black tracking-wide text-center w-full">
                        Our Professional <span className="text-primary-900">Services</span>
                    </h2>
                    <p className="mt-4 text-sm md:text-base lg:text-lg font-medium leading-relaxed text-gray-500 w-full max-w-4xl text-center">
                        Lorem ipsum dolor sit amet, Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
                        consequat.
                    </p>
                    {cards.map((card, i) => (
                        <div className="md:w-1/2 lg:w-1/3 px-6 flex" key={i}>
                            <div className="flex flex-col mx-auto max-w-xs items-center px-6 py-10 border-2 border-dashed border-primary-900 rounded-lg mt-12">
                                <span className="border-2 border-primary-900 text-center rounded-full p-6 flex-shrink-0 relative">
                                    {<Image src={card.imageSrc} alt={card.title} width={30} height={30} className="w-5 h-5 text-primary-900" />}
                                </span>
                                <span className="mt-6 text-center">
                                    <span className="mt-2 font-bold text-xl leading-none text-primary-900">{card.title || 'Fully Secure'}</span>
                                    <p className="description">
                                        {card.description ||
                                            'Lorem ipsum donor amet siti ceali ut enim ad minim veniam, quis nostrud. Sic Semper Tyrannis. Neoas Calie artel.'}
                                    </p>
                                </span>
                            </div>
                        </div>
                    ))}
                </div>
                <div className="pointer-events-none absolute right-0 bottom-0 w-64 opacity-25 transform translate-x-32 translate-y-48" />
            </div>
        </Slide>
    );
};

export default OurProfessionalServices;
