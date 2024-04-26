import React from 'react';

interface OrderInterviewProps {}

const OrderInterview: React.FunctionComponent<OrderInterviewProps> = () => {
    const newseLatterData = [
        {
            id: '1',
            icon: { parentBg: '#beaaf7', childBg: 'rgb(131 88 255) ', svg: 'icon-wallet' },

            title: ['Set', 'up', 'your', 'wallet'],
            text: "Once you've set up your wallet of choice, connect it to OpenSeaby clicking the NFT Marketplacein the topright corner.",
        },
        {
            id: '2',
            icon: { parentBg: '#c4f2e3', childBg: 'rgb(16 185 129)', svg: 'icon-wallet' },

            title: ['Create', 'Your', 'Collection'],
            text: 'Click Create and set up your collection. Add social links, a description, profile & banner images, andset a secondary sales fee.',
        },
        {
            id: '3',
            icon: { parentBg: '#cddffb', childBg: 'rgb(66 138 248)', svg: 'icon-gallery' },
            title: ['Add', 'Your', 'NFTs'],
            text: 'Upload your work (image, video, audio, or 3D art), add a title and description, and customize your NFTswith properties, stats.',
        },
        {
            id: '4',
            icon: { parentBg: '#ffd0d0', childBg: 'rgb(239 68 68)', svg: 'icon-list' },
            title: ['List', 'Them', 'For', 'Sale'],
            text: 'Choose between auctions, fixed-price listings, and declining-price listings. You choose how you want tosell your NFTs!',
        },
    ];
    return (
        <section className="dark:bg-jacarta-800 relative py-24">
            <div className="container">
                <h1 className="font-display text-jacarta-700 mb-8 text-center text-3xl dark:text-white">Order Interview</h1>

                <div className="grid grid-cols-1 gap-12 md:grid-cols-2 lg:grid-cols-4">
                    {newseLatterData.map((item) => {
                        const { id, icon, title, text } = item;
                        return (
                            <div className="text-center newseLatter-item" key={id}>
                                <div className={`mb-6 inline-flex rounded-full p-3`} style={{ backgroundColor: icon.parentBg }}>
                                    <div
                                        className={`inline-flex h-12 w-12 items-center justify-center rounded-full`}
                                        style={{ backgroundColor: icon.childBg }}
                                    >
                                        <svg className="icon icon-wallet h-5 w-5 fill-white">
                                            <use xlinkHref={`/icons.svg#${icon.svg}`}></use>
                                        </svg>
                                    </div>
                                </div>
                                <h3 className="font-display text-jacarta-700 mb-4 text-lg dark:text-white">
                                    {id}. {title}
                                </h3>
                                <p className="dark:text-jacarta-300">{text}</p>
                            </div>
                        );
                    })}
                </div>

                <p className="text-jacarta-700 mx-auto mt-20 max-w-2xl text-center text-lg dark:text-white">
                    Join our mailing list to stay in the loop with our newest feature releases, NFT drops, and tips and tricks for navigating Xhibiter
                </p>

                <div className="mx-auto mt-7 max-w-md text-center">
                    <form className="relative" onSubmit={(e) => e.preventDefault()}>
                        <input
                            type="email"
                            placeholder="Email address"
                            className="dark:bg-jacarta-700 dark:border-jacarta-600 focus:ring-accent border-jacarta-100 w-full rounded-full border py-3 px-4 dark:text-white dark:placeholder-white"
                        />
                        <button className="hover:bg-accent-dark font-display bg-accent absolute top-2 right-2 rounded-full px-6 py-2 text-sm text-white">
                            Subscribe
                        </button>
                    </form>
                </div>
            </div>
        </section>
    );
};

export default OrderInterview;
