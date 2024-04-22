import { DashboardHeaderLayout } from '@components/layouts';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import CategoryList from '@features/catogories/CategoryList';
// import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface StaffListPageProps {}

const StaffListPage: NextPage<StaffListPageProps> = ({}) => {
    return (
        // <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
        <ModalProvider>
            <TableUtilProvider>
                <DashboardHeaderLayout title="">
                    <CategoryList />
                </DashboardHeaderLayout>
            </TableUtilProvider>
        </ModalProvider>
        // </ProtectWrapper>
    );
};

export default StaffListPage;
