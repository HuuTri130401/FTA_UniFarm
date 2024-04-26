import { DashboardHeaderLayout } from '@components/layouts';
import CustomSkeleton from '@components/skeletons/CustomSkeleton';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { IV1GetFilterCandidate } from '@core/api/candidate';
import { routes } from '@core/routes';
import ProductDetailFarmHub from '@features/farmhub/product/ProductDetailFarmHub';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { NextPage } from 'next';

interface ProductPageProps {
    filter: Partial<IV1GetFilterCandidate>;
}

const ProductPage: NextPage<ProductPageProps> = ({ filter }) => {
    const { farmHub } = useStoreUser();
    return (
        <ProtectWrapper acceptRoles={[UserRole.FARM_HUB]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Quản Lý Sản Phẩm"
                        // breadcrumbs={[
                        //     { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                        //     { key: '2', element: 'Danh sách sản phẩm' },
                        // ]}
                    >
                        {farmHub?.id ? <ProductDetailFarmHub farmHubId={farmHub?.id} /> : <CustomSkeleton isFetched={true} />}
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default ProductPage;
