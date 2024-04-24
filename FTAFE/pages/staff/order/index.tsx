import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import BatchList from '@features/staff/batch/BatchList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout title="Danh sách đơn hàng">
                        <BatchList businessDayId={id} />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};
Page.getInitialProps = async (ctx): Promise<PageProps> => {
    const id = String(ctx.query.id || '');
    return {
        id,
    };
};

export default Page;
