import Link from 'next/link';
import React from 'react';

interface ContactScreenProps {}

const ContactScreen: React.FunctionComponent<ContactScreenProps> = () => {
    return (
        <>
            <section
                className="relative py-32 bg-center bg-no-repeat bg-cover after:bg-jacarta-900/60 after:absolute after:inset-0"
                style={{ backgroundImage: `url(/images/page-title/knowledge_base_banner.jpg)` }}
            >
                <div className="container relative z-10">
                    <h1 className="text-4xl font-medium text-center text-white uppercase font-display">Tetcha Company</h1>

                    <form action="search" className="relative block max-w-md mx-auto mt-4">
                        <input
                            type="search"
                            className="text-jacarta-700 placeholder-jacarta-500 focus:ring-accent border-jacarta-100 w-full rounded-2xl border py-[0.6875rem] px-4 pl-10 bg-white"
                            placeholder="Search"
                        />
                        <span className="absolute top-0 left-0 flex items-center justify-center w-12 h-full rounded-2xl">
                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" className="w-4 h-4 fill-jacarta-500">
                                <path fill="none" d="M0 0h24v24H0z"></path>
                                <path d="M18.031 16.617l4.283 4.282-1.415 1.415-4.282-4.283A8.96 8.96 0 0 1 11 20c-4.968 0-9-4.032-9-9s4.032-9 9-9 9 4.032 9 9a8.96 8.96 0 0 1-1.969 5.617zm-2.006-.742A6.977 6.977 0 0 0 18 11c0-3.868-3.133-7-7-7-3.868 0-7 3.132-7 7 0 3.867 3.132 7 7 7a6.977 6.977 0 0 0 4.875-1.975l.15-.15z"></path>
                            </svg>
                        </span>
                    </form>
                </div>
            </section>

            <section className="relative py-24 dark:bg-jacarta-800">
                <picture className="absolute inset-0 pointer-events-none -z-10 dark:hidden">
                    <img src="/images/gradient_light.jpg" alt="gradient" className="w-full h-full" />
                </picture>
                <div className="container">
                    <div className="lg:flex">
                        {/* <!-- Contact Form --> */}
                        <div className="mb-12 lg:mb-0 lg:w-2/3 lg:pr-12">
                            <h2 className="mb-4 text-xl font-display text-jacarta-700 dark:text-white">Contact Us</h2>
                            <p className="mb-16 text-lg leading-normal dark:text-jacarta-300">
                                {" Have a question? Need help? Don't hesitate, drop us a line"}
                            </p>

                            <form id="contact-form" method="post">
                                <div className="flex space-x-7">
                                    <div className="w-1/2 mb-6">
                                        <label className="block mb-1 text-sm font-display text-jacarta-700 dark:text-white">
                                            Name<span className="text-red">*</span>
                                        </label>
                                        <input
                                            name="name"
                                            className="w-full py-3 rounded-lg contact-form-input dark:bg-jacarta-700 border-jacarta-100 hover:ring-accent/10 focus:ring-accent dark:border-jacarta-600 dark:placeholder:text-jacarta-300 hover:ring-2 dark:text-white"
                                            id="name"
                                            type="text"
                                            required
                                        />
                                    </div>

                                    <div className="w-1/2 mb-6">
                                        <label className="block mb-1 text-sm font-display text-jacarta-700 dark:text-white">
                                            Email<span className="text-red">*</span>
                                        </label>
                                        <input
                                            name="email"
                                            className="w-full py-3 rounded-lg contact-form-input dark:bg-jacarta-700 border-jacarta-100 hover:ring-accent/10 focus:ring-accent dark:border-jacarta-600 dark:placeholder:text-jacarta-300 hover:ring-2 dark:text-white"
                                            id="email"
                                            type="email"
                                            required
                                        />
                                    </div>
                                </div>

                                <div className="mb-4">
                                    <label className="block mb-1 text-sm font-display text-jacarta-700 dark:text-white">
                                        Message<span className="text-red">*</span>
                                    </label>
                                    <textarea
                                        id="message"
                                        className="w-full py-3 rounded-lg contact-form-input dark:bg-jacarta-700 border-jacarta-100 hover:ring-accent/10 focus:ring-accent dark:border-jacarta-600 dark:placeholder:text-jacarta-300 hover:ring-2 dark:text-white"
                                        required
                                        name="message"
                                        rows={5}
                                    ></textarea>
                                </div>

                                <div className="flex items-center mb-6 space-x-2">
                                    <input
                                        type="checkbox"
                                        id="contact-form-consent-input"
                                        name="agree-to-terms"
                                        className="self-start w-5 h-5 rounded cursor-pointer checked:bg-accent dark:bg-jacarta-600 text-accent border-jacarta-200 focus:ring-accent/20 dark:border-jacarta-500 focus:ring-offset-0"
                                    />
                                    <label className="text-sm dark:text-jacarta-200">
                                        I agree to the{' '}
                                        <Link href="/tarms">
                                            <p className="text-accent">Terms of Service</p>
                                        </Link>
                                    </label>
                                </div>

                                <button
                                    type="submit"
                                    className="px-8 py-3 font-semibold text-center text-white transition-all rounded-full bg-accent shadow-accent-volume hover:bg-accent-dark"
                                    id="contact-form-submit"
                                >
                                    Submit
                                </button>

                                <div id="contact-form-notice" className="relative hidden p-4 mt-4 border border-transparent rounded-lg"></div>
                            </form>
                        </div>

                        {/* <!-- Info --> */}
                        <div className="lg:w-1/3 lg:pl-5">
                            <h2 className="mb-4 text-xl font-display text-jacarta-700 dark:text-white">Information</h2>
                            <p className="mb-6 text-lg leading-normal dark:text-jacarta-300">
                                {
                                    "Don't hesitaste, drop us a line Collaboratively administrate channels whereas virtual. Objectively seize scalable metrics whereas proactive e-services."
                                }
                            </p>

                            <div className="dark:bg-jacarta-700 dark:border-jacarta-600 border-jacarta-100 rounded-2.5xl border bg-white p-10">
                                <div className="flex items-center mb-6 space-x-5">
                                    <span className="flex items-center justify-center border rounded-full dark:bg-jacarta-700 dark:border-jacarta-600 border-jacarta-100 bg-light-base h-11 w-11 shrink-0">
                                        <svg
                                            xmlns="http://www.w3.org/2000/svg"
                                            viewBox="0 0 24 24"
                                            width="24"
                                            height="24"
                                            className="fill-jacarta-400"
                                        >
                                            <path fill="none" d="M0 0h24v24H0z" />
                                            <path d="M9.366 10.682a10.556 10.556 0 0 0 3.952 3.952l.884-1.238a1 1 0 0 1 1.294-.296 11.422 11.422 0 0 0 4.583 1.364 1 1 0 0 1 .921.997v4.462a1 1 0 0 1-.898.995c-.53.055-1.064.082-1.602.082C9.94 21 3 14.06 3 5.5c0-.538.027-1.072.082-1.602A1 1 0 0 1 4.077 3h4.462a1 1 0 0 1 .997.921A11.422 11.422 0 0 0 10.9 8.504a1 1 0 0 1-.296 1.294l-1.238.884zm-2.522-.657l1.9-1.357A13.41 13.41 0 0 1 7.647 5H5.01c-.006.166-.009.333-.009.5C5 12.956 11.044 19 18.5 19c.167 0 .334-.003.5-.01v-2.637a13.41 13.41 0 0 1-3.668-1.097l-1.357 1.9a12.442 12.442 0 0 1-1.588-.75l-.058-.033a12.556 12.556 0 0 1-4.702-4.702l-.033-.058a12.442 12.442 0 0 1-.75-1.588z" />
                                        </svg>
                                    </span>

                                    <div>
                                        <span className="block text-base font-display text-jacarta-700 dark:text-white">Phone</span>
                                        <a href="tel:123-123-456" className="text-sm hover:text-accent dark:text-jacarta-300">
                                            (123) 123-456
                                        </a>
                                    </div>
                                </div>

                                <div className="flex items-center mb-6 space-x-5">
                                    <span className="flex items-center justify-center border rounded-full dark:bg-jacarta-700 dark:border-jacarta-600 border-jacarta-100 bg-light-base h-11 w-11 shrink-0">
                                        <svg
                                            xmlns="http://www.w3.org/2000/svg"
                                            viewBox="0 0 24 24"
                                            width="24"
                                            height="24"
                                            className="fill-jacarta-400"
                                        >
                                            <path fill="none" d="M0 0h24v24H0z" />
                                            <path d="M12 20.9l4.95-4.95a7 7 0 1 0-9.9 0L12 20.9zm0 2.828l-6.364-6.364a9 9 0 1 1 12.728 0L12 23.728zM12 13a2 2 0 1 0 0-4 2 2 0 0 0 0 4zm0 2a4 4 0 1 1 0-8 4 4 0 0 1 0 8z" />
                                        </svg>
                                    </span>

                                    <div>
                                        <span className="block text-base font-display text-jacarta-700 dark:text-white">Address</span>
                                        <address className="text-sm not-italic dark:text-jacarta-300">
                                            Lot E2a-7, Road D1, D. D1, Long Thanh My, Thu Duc City, Ho Chi Minh City 700000
                                        </address>
                                    </div>
                                </div>

                                <div className="flex items-center space-x-5">
                                    <span className="flex items-center justify-center border rounded-full dark:bg-jacarta-700 dark:border-jacarta-600 border-jacarta-100 bg-light-base h-11 w-11 shrink-0">
                                        <svg
                                            xmlns="http://www.w3.org/2000/svg"
                                            viewBox="0 0 24 24"
                                            width="24"
                                            height="24"
                                            className="fill-jacarta-400"
                                        >
                                            <path fill="none" d="M0 0h24v24H0z" />
                                            <path d="M2.243 6.854L11.49 1.31a1 1 0 0 1 1.029 0l9.238 5.545a.5.5 0 0 1 .243.429V20a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V7.283a.5.5 0 0 1 .243-.429zM4 8.133V19h16V8.132l-7.996-4.8L4 8.132zm8.06 5.565l5.296-4.463 1.288 1.53-6.57 5.537-6.71-5.53 1.272-1.544 5.424 4.47z" />
                                        </svg>
                                    </span>

                                    <div>
                                        <span className="block text-base font-display text-jacarta-700 dark:text-white">Email</span>
                                        <a href="mailto:office@xhibiter.com" className="text-sm not-italic hover:text-accent dark:text-jacarta-300">
                                            uni@gmail.com
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </>
    );
};

export default ContactScreen;
