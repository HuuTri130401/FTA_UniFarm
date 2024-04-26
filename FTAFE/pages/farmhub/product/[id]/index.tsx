import { DashboardHeaderLayout } from '@components/layouts';
import { productAPI } from '@core/api/product.api';
import { routes } from '@core/routes';
import { Product } from '@models/product';
import { useStoreUser } from '@store/index';
import { useQuery } from '@tanstack/react-query';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';
interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const { data } = useQuery({
        queryFn: async () => await productAPI.getProductById(id),
        queryKey: ['product', id],
    });
    const product: Product = data?.payload;
    const { farmHub } = useStoreUser();
    return (
        <ToggleProvider>
            <DashboardHeaderLayout
                title=" Thông tin sản phẩm"
                // breadcrumbs={[
                //     { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                //     { key: '2', element: <span className="text-primary">Danh sách bán</span>, path: routes.farmhub.product.list() },
                //     // { key: '3', element: 'Chi tiết sản phẩm' },
                // ]}
            >
                {/* {farmHub?.id ? <ProductDetailFarmHub product={product} farmHubId={farmHub?.id} /> : <CustomSkeleton isFetched={true} />} */}
                <></>
            </DashboardHeaderLayout>
        </ToggleProvider>
    );
};
Page.getInitialProps = async (ctx): Promise<PageProps> => {
    const id = String(ctx.query.id || '');

    return {
        id,
    };
};
export default Page;
