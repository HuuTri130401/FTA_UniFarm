import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { StationFilter } from '@core/api/station.api';
import { routes } from '@core/routes';
import StationList from '@features/admin/user/station/StationList';
import { defaultPagingProps } from '@models/interface';
import { UserRole } from '@models/user';
import { objectHelper } from '@utils/index';
import { NextPage } from 'next';

interface PageProps {
    filter: Partial<StationFilter>;
}

const Page: NextPage<PageProps> = ({ filter }) => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Danh sách trạm vận chuyển"
                        // breadcrumbs={[
                        //     { key: '1', element: 'Dashboard', path: routes.admin.home() },
                        //     { key: '2', element: 'Danh sách trạm' },
                        // ]}
                    >
                        <StationList filter={filter} />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

Page.getInitialProps = async (ctx): Promise<PageProps> => {
    return {
        filter: objectHelper.getObjectWithDefault<Partial<StationFilter>>(ctx.query, {
            ...defaultPagingProps,
            name: '',
            description: '',
            address: '',
        }),
    };
};
export default Page;
