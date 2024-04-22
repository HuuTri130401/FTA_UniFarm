import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import TransferDetail from '@features/staff/transfer/TransferDetail';
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
                    <DashboardHeaderLayout
                        title={`Đơn hàng mã ${id}`}
                        breadcrumbs={[
                            { key: '1', element: 'Danh sách hàng ', path: routes.staff.transfer.list() },
                            { key: '2', element: 'Chi tiết' },
                        ]}
                    >
                        <TransferDetail id={id} />
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
