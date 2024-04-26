import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { AreaAPI } from '@core/api/area.api';
import { routes } from '@core/routes';
import AreaDetail from '@features/admin/user/areas/AreaDetail';
import { UserRole } from '@models/user';
import { useQuery } from '@tanstack/react-query';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const { data } = useQuery({
        queryKey: ['area', id],
        queryFn: async () => await AreaAPI.getOne(id),
    });

    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ToggleProvider>
                <DashboardHeaderLayout
                    title=""
                    breadcrumbs={[
                        {
                            key: '1',
                            element: 'Danh sách khu vực',
                            path: routes.admin.area(),
                        },
                        {
                            key: '2',
                            element: 'Chi tiết',
                        },
                    ]}
                >
                    <AreaDetail value={data?.payload} />
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
