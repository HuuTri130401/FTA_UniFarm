import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { routes } from '@core/routes';
import FarmHubDetail from '@features/admin/user/farmhub/FarmHubDetail';
import { useQueryFarmHubById } from '@hooks/api/farmhub.hook';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const { data } = useQueryFarmHubById(id);

    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ToggleProvider>
                <DashboardHeaderLayout
                    title="Thông tin cơ bản"
                    breadcrumbs={[
                        { key: '1', element: 'Danh sách nông trại', path: routes.admin.user.farm_hub.list() },
                        { key: '2', element: 'Chi tiết' },
                    ]}
                >
                    <FarmHubDetail farmHub={data?.payload} />
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
