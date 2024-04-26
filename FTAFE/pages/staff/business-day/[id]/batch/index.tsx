import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import BatchList from '@features/staff/batch/BatchList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { useRouter } from 'next/router';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const router = useRouter();

    return (
        <ProtectWrapper acceptRoles={[UserRole.COLLECTED_STAFF]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title={`Danh sách chuyến hàng `}
                        breadcrumbs={[
                            { key: '1', element: 'Danh sách ngày bán', path: routes.staff.businessDay.list() },
                            { key: '2', element: 'Chuyến hàng' },
                        ]}
                    >
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
