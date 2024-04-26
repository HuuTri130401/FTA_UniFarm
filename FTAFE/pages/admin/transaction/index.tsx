import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import { ModalProvider } from '@context/modalContext';
import { TableUtilProvider } from '@context/tableUtilContext';
import TransactionsList from '@features/farmhub/transaction/TransactionsList';
import { UserRole } from '@models/user';
import { useStoreUser } from '@store/index';
import { NextPage } from 'next';

interface TransactionPageProps {}

const TransactionPage: NextPage<TransactionPageProps> = () => {
    const { farmHub } = useStoreUser();
    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <ModalProvider>
                <TableUtilProvider>
                    <DashboardHeaderLayout
                        title="Lịch sử giao dịch"
                        // breadcrumbs={[
                        //     { key: '1', element: <span className="text-primary">Dashboard</span>, path: routes.farmhub.home() },
                        //     { key: '2', element: 'Lịch sử giao dịch' },
                        // ]}
                    >
                        <TransactionsList />
                    </DashboardHeaderLayout>
                </TableUtilProvider>
            </ModalProvider>
        </ProtectWrapper>
    );
};

export default TransactionPage;
