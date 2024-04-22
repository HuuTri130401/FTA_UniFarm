import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import PaymentList from '@features/admin/payment/PaymentList';
import { defaultPagingProps } from '@models/interface';
import { FilterPayment } from '@models/payment';
import { UserRole } from '@models/user';
import { objectHelper } from '@utils/index';
import { NextPage } from 'next';
interface PageProps {
    filter: Partial<FilterPayment>;
}

const Page: NextPage<PageProps> = ({ filter }) => {
    return (
        <>
            <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
                <ModalProvider>
                    <TableUtilProvider>
                        <DashboardHeaderLayout title="">
                            <PaymentList filter={{ ...filter, orderBy: 'PaymentDay' }} />
                        </DashboardHeaderLayout>
                    </TableUtilProvider>
                </ModalProvider>
            </ProtectWrapper>
        </>
    );
};
Page.getInitialProps = async (ctx): Promise<PageProps> => {
    return {
        filter: objectHelper.getObjectWithDefault<Partial<FilterPayment>>(ctx.query, {
            ...defaultPagingProps,
            from: '',
            to: '',
        }),
    };
};
export default Page;
