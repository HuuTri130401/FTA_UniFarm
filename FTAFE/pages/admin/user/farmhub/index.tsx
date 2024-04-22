import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import FarmHubList from '@features/admin/user/farmhub/FarmHubList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface PageProps {
    // filter: Partial<IV1GetFilterCandidate>;
}

const Page: NextPage<PageProps> = () => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Quản lý FarmHub"
                        // breadcrumbs={[
                        //     { key: '1', element: 'Dashboard', path: routes.admin.home() },
                        //     { key: '2', element: 'Danh sách trang trại' },
                        // ]}
                    >
                        <FarmHubList />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default Page;
