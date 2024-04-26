import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import BusinessDayList from '@features/admin/user/business-day/BusinessDayList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface PageProps {}

const Page: NextPage<PageProps> = ({}) => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Danh sách ngày bán"
                        // breadcrumbs={[{ key: '1', element: 'Danh sách ngày bán' }]}
                    >
                        <BusinessDayList />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default Page;
