import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import TransferList from '@features/staff/transfer/TransferList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface PageProps {}

const Page: NextPage<PageProps> = () => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout title="Danh sách đơn hàng vận chuyển">
                        <TransferList />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default Page;
