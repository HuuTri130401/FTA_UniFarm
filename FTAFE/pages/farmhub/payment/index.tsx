import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import { routes } from '@core/routes';
import PaymentList from '@features/farmhub/transaction/PaymentList';
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
            <ProtectWrapper acceptRoles={[UserRole.FARM_HUB]}>
                <ModalProvider>
                    <TableUtilProvider>
                        <DashboardHeaderLayout
                            title="Rút tiền"
                            // breadcrumbs={[
                            //     { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                            //     { key: '2', element: 'Rút tiền' },
                            // ]}
                        >
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
