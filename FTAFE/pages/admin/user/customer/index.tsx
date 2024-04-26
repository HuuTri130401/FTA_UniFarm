import { DashboardHeaderLayout } from '@components/layouts';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import CustomerList from '@features/admin/user/customer/CustomerList';
import { CustomerFilter } from '@models/customer';
import { defaultPagingProps } from '@models/interface';
import { objectHelper } from '@utils/index';
import { NextPage } from 'next';

interface PageProps {
    filter: Partial<CustomerFilter>;
}

const Page: NextPage<PageProps> = ({ filter }) => {
    return (
        // <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
        <ModalProvider>
            <TableUtilProvider>
                <DashboardHeaderLayout
                    title="Quản lý khách hàng"
                    // breadcrumbs={[
                    //     { key: '1', element: 'Dashboard', path: routes.admin.home() },
                    //     { key: '2', element: 'Danh sách khách hàng' },
                    // ]}
                >
                    <CustomerList filter={filter} />
                </DashboardHeaderLayout>
            </TableUtilProvider>
        </ModalProvider>
        // </ProtectWrapper>
    );
};

Page.getInitialProps = async (ctx): Promise<PageProps> => {
    return {
        filter: objectHelper.getObjectWithDefault<Partial<CustomerFilter>>(ctx.query, {
            ...defaultPagingProps,
            email: '',
            firstName: '',
            lastName: '',
            phoneNumber: '',
        }),
    };
};
export default Page;
