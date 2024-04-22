import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { StationAPI } from '@core/api/station.api';
import { routes } from '@core/routes';
import StationsDetail from '@features/admin/user/station/StationDetail';
import { UserRole } from '@models/user';
import { useQuery } from '@tanstack/react-query';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const { data } = useQuery({
        queryKey: ['station', id],
        queryFn: async () => await StationAPI.getById(id),
    });

    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ToggleProvider>
                <DashboardHeaderLayout
                    title="Thông tin trạm"
                    breadcrumbs={[
                        { key: '1', element: 'Danh sách trạm', path: routes.admin.user.station.list() },
                        { key: '2', element: 'Chi tiết' },
                    ]}
                >
                    <StationsDetail value={data?.payload} />
                </DashboardHeaderLayout>
            </ToggleProvider>
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
