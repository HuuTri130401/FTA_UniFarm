import Link from 'next/link';
import React from 'react';
import { Swiper, SwiperSlide } from 'swiper/react';

interface TechnologyProps {}

const Technology: React.FunctionComponent<TechnologyProps> = () => {
    const Browse_category_data = [
        {
            id: '0Virtual Worlds',
            image: '/images/categories/category_4.png',
            title: 'Virtual Worlds',
            href: '#',
            bgColor: 'rgb(70 199 227)',
        },
        {
            id: '1Sport',
            image: '/images/categories/category_5.png',
            title: 'Sport',
            href: '#',
            bgColor: 'rgb(115 126 242)',
        },
        {
            id: '2Photography',
            image: '/images/categories/category_6.png',
            title: 'Photography',
            href: '#',
            bgColor: 'rgb(66 138 248)',
        },
        {
            id: '3Music',
            image: '/images/categories/category_7.png',
            title: 'Music',
            href: '#',
            bgColor: 'rgb(243 91 199)',
        },
        {
            id: '4Art',
            image: '/images/categories/category_1.png',
            title: 'Art',
            href: '#',
            bgColor: 'rgb(16 185 129)',
        },
        {
            id: '5Collectibles',
            image: '/images/categories/category_2.png',
            title: 'Collectibles',
            href: '#',
            bgColor: 'rgb(254 178 64)',
        },
        {
            id: '6Domain Names',
            image: '/images/categories/category_3.png',
            title: 'Domain Names',
            href: '#',
            bgColor: 'rgb(131 88 255)',
        },
        {
            id: '7Virtual Worlds',
            image: '/images/categories/category_4.png',
            title: 'Virtual Worlds',
            href: '#',
            bgColor: 'rgb(70 199 227)',
        },
        {
            id: '8Sport',
            image: '/images/categories/category_5.png',
            title: 'Sport',
            href: '#',
            bgColor: '#737EF2',
        },
        {
            id: '9Photography',
            image: '/images/categories/category_6.png',
            title: 'Photography',
            href: '#',
            bgColor: 'rgb(66 138 248)',
        },
        {
            id: '10Music',
            image: '/images/categories/category_7.png',
            title: 'Music',
            href: '#',
            bgColor: '#F35BC7',
        },
        {
            id: '11Art',
            image: '/images/categories/category_1.png',
            title: 'Art',
            href: '#',
            bgColor: 'rgb(16 185 129)',
        },
        {
            id: '12Virtual Worlds',
            image: '/images/categories/category_4.png',
            title: 'Virtual Worlds',
            href: '#',
            bgColor: '#46C7E3',
        },
        {
            id: '13Domain Names',
            image: '/images/categories/category_3.png',
            title: 'Domain Names',
            href: '#',
            bgColor: 'rgb(131 88 255)',
        },

        {
            id: '14Collectibles',
            image: '/images/categories/category_2.png',
            title: 'Collectibles',
            href: '#',
            bgColor: 'rgb(254 178 64)',
        },
    ];

    return (
        <>
            <h1 className="font-display text-jacarta-700 mb-8 text-center text-3xl dark:text-white">Technology</h1>
            <div className="relative">
                <div className="overflow-hidden">
                    <Swiper
                        slidesPerView="auto"
                        spaceBetween={10}
                        loop={true}
                        breakpoints={{
                            // when window width is >= 640px
                            100: {
                                slidesPerView: 3,
                                spaceBetween: 20,
                            },
                            // when window width is >= 768px
                            700: {
                                slidesPerView: 4,
                                spaceBetween: 20,
                            },
                            900: {
                                slidesPerView: 5,
                                spaceBetween: 20,
                            },
                            1200: {
                                slidesPerView: 7,
                                spaceBetween: 30,
                            },
                        }}
                        className=" card-slider-4-columns !py-5"
                        style={{ transform: 'scaleX(1.2)' }}
                    >
                        {Browse_category_data.map((item) => {
                            const { id, image, title, bgColor } = item;
                            return (
                                <SwiperSlide key={id}>
                                    <article>
                                        <Link href="/collection/explore_collection">
                                            <a className="dark:bg-jacarta-700 dark:border-jacarta-700 border-jacarta-100 rounded-2xl block border bg-white p-[1.1875rem] transition-shadow hover:shadow-lg">
                                                <figure
                                                    style={{ backgroundColor: bgColor }}
                                                    className={` rounded-t-[0.625rem] w-full rounded-[0.625rem`}
                                                >
                                                    <img src={image} alt="item 1" className="w-full" />
                                                </figure>
                                                <div className="mt-4 text-center">
                                                    <span className="font-display text-jacarta-700 text-lg dark:text-white">{title}</span>
                                                </div>
                                            </a>
                                        </Link>
                                    </article>
                                </SwiperSlide>
                            );
                        })}
                    </Swiper>
                </div>
            </div>
        </>
    );
};

export default Technology;
