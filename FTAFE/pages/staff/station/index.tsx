import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import StationList from '@features/staff/transfer/component/StationList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
interface PageProps {}

const Page: NextPage<PageProps> = () => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout title="Danh sách trạm hàng hóa">
                        <StationList />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default Page;
