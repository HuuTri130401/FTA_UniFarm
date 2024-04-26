import { FarmHub } from '@models/user';
import * as React from 'react';

interface StaffDetailProps {
    farmhub: FarmHub;
}

const ProductList: React.FunctionComponent<StaffDetailProps> = ({ farmhub }) => {
    return (
        <>
            <div className="flex flex-col w-full gap-4"></div>
        </>
    );
};

export default ProductList;
