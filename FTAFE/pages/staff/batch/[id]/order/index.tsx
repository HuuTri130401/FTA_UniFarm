import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import OrderList from '@features/staff/batch/OrderList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { useRouter } from 'next/router';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const router = useRouter();
    const businessDayId = String(router.query.businessDayId) || '';

    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Danh sách đơn hàng"
                        breadcrumbs={[
                            { key: '1', element: 'Danh sách ngày bán', path: routes.staff.businessDay.list() },
                            { key: '2', element: 'Chuyến hàng trong ngày', path: routes.staff.businessDay.batchList(businessDayId) },
                            { key: '3', element: 'Đơn hàng' },
                        ]}
                    >
                        <OrderList batchId={id} />
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
