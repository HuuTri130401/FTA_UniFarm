import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
import BatchDetails from '@features/farmhub/batch/BatchDetail';
import { useQueryGetDetailBatch } from '@hooks/api/batch.hook';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    const { data: batchDetail } = useQueryGetDetailBatch(id as string);

    return (
        <ProtectWrapper acceptRoles={[UserRole.FARM_HUB]}>
            <ToggleProvider>
                <DashboardHeaderLayout title={`Lô hàng ${batchDetail?.collectedHubName || 'Lô hàng'}`}>
                    <BatchDetails value={batchDetail} />
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
