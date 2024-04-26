import { DashboardHeaderLayout } from '@components/layouts';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
// import { IV1GetFilterStaff } from '@core/api/staff.api';
// import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface StaffListPageProps {}

const StaffListPage: NextPage<StaffListPageProps> = () => {
    return (
        // <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
        <ModalProvider>
            <TableUtilProvider>
                <DashboardHeaderLayout title="Sản Phẩm">
                    {/* <StaffList filter={filter} /> */}
                    <></>
                </DashboardHeaderLayout>
            </TableUtilProvider>
        </ModalProvider>
        // </ProtectWrapper>
    );
};

export default StaffListPage;
