import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import BusinessDayList from '@features/farmhub/business-day/BusinessDayList';
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
                            title="Ngày Đăng Bán Hiện Có"
                            // breadcrumbs={[
                            //     { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                            //     { key: '3', element: 'Danh sách ngày đăng bán' },
                            // ]}
                        >
                            <BusinessDayList />
                        </DashboardHeaderLayout>
                    </TableUtilProvider>
                </ModalProvider>
            </ProtectWrapper>
        </>
    );
};
export default Page;
