import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import BatchListFarmHub from '@features/farmhub/batch/BatchList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
interface PageProps {}

const Page: NextPage<PageProps> = () => {
    return (
        <>
            <ProtectWrapper acceptRoles={[UserRole.FARM_HUB]}>
                <ModalProvider>
                    <TableUtilProvider>
                        <DashboardHeaderLayout
                            title="Danh sách lô hàng của FarmHub"
                            breadcrumbs={[
                                { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                                { key: '3', element: 'Danh sách lô hàng' },
                            ]}
                        >
                            <BatchListFarmHub />
                        </DashboardHeaderLayout>
                    </TableUtilProvider>
                </ModalProvider>
            </ProtectWrapper>
        </>
    );
};
export default Page;
