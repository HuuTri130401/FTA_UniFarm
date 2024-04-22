import { CommonSeo } from '@components/commons';
import { NextPage } from 'next';
import * as React from 'react';
import LandingPage from 'src/screens';

interface HomePageProps {}

const HomePage: NextPage<HomePageProps> = () => {
    return (
        <React.Fragment>
            <CommonSeo title="Home" />
            <LandingPage />
            {/* <ProtectWrapper acceptRoles={[]}>
                <div className="w-full h-screen">
                    <div className="flex items-center justify-center w-full h-full">
                        <div className="text-4xl font-bold">Chào mừng đến với Farm To Apartment</div>
                    </div>
                </div>
            </ProtectWrapper> */}
        </React.Fragment>
    );
};

export default HomePage;
