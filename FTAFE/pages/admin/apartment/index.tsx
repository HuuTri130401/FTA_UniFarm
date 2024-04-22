import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import ApartmentList from '@features/admin/apartment/ApartmentList';
import { ApartmentFilter } from '@models/apartment';
import { defaultPagingProps } from '@models/interface';
import { UserRole } from '@models/user';
import { objectHelper } from '@utils/index';
import { NextPage } from 'next';

interface PageProps {
    filter: Partial<ApartmentFilter>;
}

const Page: NextPage<PageProps> = ({ filter }) => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout title="Quản lý căn hộ">
                        <ApartmentList filter={filter} />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

Page.getInitialProps = async (ctx): Promise<PageProps> => {
    return {
        filter: objectHelper.getObjectWithDefault<Partial<ApartmentFilter>>(ctx.query, {
            ...defaultPagingProps,
            address: '',
            name,
        }),
    };
};
export default Page;
