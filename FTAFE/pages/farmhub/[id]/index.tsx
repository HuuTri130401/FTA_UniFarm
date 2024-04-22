import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { routes } from '@core/routes';
import BusinessDayDetail from '@features/farmhub/business-day/BusinessDayDetail';
import { useQueryGetDetailBusinessDay } from '@hooks/api/businessDay.hook';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const { data: businessDays } = useQueryGetDetailBusinessDay(id);

    return (
        <ProtectWrapper acceptRoles={[UserRole.FARM_HUB]}>
            <ToggleProvider>
                <DashboardHeaderLayout
                    title={`${businessDays?.name || 'Ngày đăng bán'}`}
                    breadcrumbs={[
                        { key: '1', element: <span className="text-primary">Danh sách ngày đăng bán</span>, path: routes.farmhub.businessDay.list() },
                        { key: '2', element: 'Chi tiết ngày đăng bán' },
                    ]}
                >
                    <BusinessDayDetail value={businessDays} />
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
