import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import CreateTransfer from '@features/staff/transfer/component/CreateTransfer';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { useRouter } from 'next/router';

interface PageProps {}

const Page: NextPage<PageProps> = () => {
    const router = useRouter();
    const stationId = String(router.query.stationId) || '';
    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout title="Danh sách đơn hàng chuyển giao">
                        <CreateTransfer stationId={stationId} />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default Page;
