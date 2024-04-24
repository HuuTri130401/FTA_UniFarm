import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { IV1GetFilterCandidate } from '@core/api/candidate';
import { routes } from '@core/routes';
import MenuList from '@features/farmhub/menu/MenuList';
import { UserRole } from '@models/user';
import { NextPage } from 'next';

interface MenuListPageProps {
    filter: Partial<IV1GetFilterCandidate>;
}

const MenuListPage: NextPage<MenuListPageProps> = ({ filter }) => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.FARM_HUB]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Các danh sách bán"
                        // breadcrumbs={[
                        //     { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                        //     { key: '2', element: 'Các danh sách sản phẩm' },
                        // ]}
                    >
                        <MenuList />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default MenuListPage;
