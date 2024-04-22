import { NextSeo } from 'next-seo';
import * as React from 'react';

interface CommonSeoProps {
    title: string;
}

export const CommonSeo: React.FC<CommonSeoProps> = ({ title }) => {
    return <NextSeo title={`${title} | UniFarm`} />;
};
