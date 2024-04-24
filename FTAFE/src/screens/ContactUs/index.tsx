import React from 'react';
import { Slide } from 'react-awesome-reveal';

interface ContactUsProps {}

const ContactUs: React.FunctionComponent<ContactUsProps> = () => {
    return (
        <Slide direction="right" triggerOnce>
            <div className="relative px-20">
                <div className="flex flex-col md:flex-row justify-between max-w-screen-xl mx-auto py-20 md:py-24">
                    <div className="w-full max-w-md mx-auto md:max-w-none md:mx-0">
                        <div
                            className={`bg-[url("https://www.nittan.quantumx.com/images/envelope.png")] rounded bg-contain bg-no-repeat bg-center h-full`}
                        />
                    </div>
                    <div className="w-full max-w-md mx-auto md:max-w-none md:mx-0 md:w-7/12 mt-16 md:mt-0 md:mr-12 lg:mr-16 md:order-first">
                        <div className="lg:py-8 text-center md:text-left">
                            <h4 className="font-bold text-primary-900 mb-4 text-center lg:text-left text-lg">Contact Us</h4>
                            <h2 className="text-4xl sm:text-5xl font-black tracking-wide text-center lg:text-left">
                                Feel free to <span className="text-primary-900">get in touch</span>
                                <wbr /> with us.
                            </h2>
                            <p className="max-w-xl text-center mx-auto lg:mx-0 lg:text-left lg:max-w-none leading-relaxed text-sm sm:text-base lg:text-lg font-medium mt-4 text-secondary-100">
                                Here are some frequently asked questions about our hotels from our loving customers. Should you have any other
                                questions, feel free to reach out via the contact form below.
                            </p>
                            <div className="mt-8 md:mt-10 text-sm flex flex-col lg:flex-row">
                                <input
                                    className="border-2 px-5 py-3 rounded focus:outline-none font-medium transition duration-300 hocus:border-primary-500"
                                    type="email"
                                    name="email"
                                    placeholder="Your Email Address"
                                />
                                <button
                                    className="px-8 py-3 font-bold rounded bg-primary-900 text-gray-100 hocus:bg-primary-700 hocus:text-gray-200 focus:shadow-outline focus:outline-none transition duration-300 inline-block lg:ml-6 mt-6 lg:mt-0"
                                    type="submit"
                                >
                                    Send Us
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </Slide>
    );
};

export default ContactUs;
