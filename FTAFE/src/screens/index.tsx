import React from 'react';

import Banner from './Banner';
import Downloads from './downloads';

interface LandingPageProps {}

const LandingPage: React.FunctionComponent<LandingPageProps> = () => {
    return (
        <div className="w-full overflow-hidden">
            <Banner />
            {/* <OurRecord /> */}
            {/* <OurProfessionalServices /> */}
            {/* <OurExpertise /> */}
            {/* <FAQ /> */}
            {/* <ContactUs /> */}
            {/* <JoinWithUs /> */}
            <Downloads />
        </div>
    );
};

export default LandingPage;
