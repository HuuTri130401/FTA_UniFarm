import { ProtectWrapper } from '@components/wrappers';
import { MonthFilter } from '@core/api/admin.api';
import { UserRole } from '@models/user';
import { objectHelper } from '@utils/index';
import { NextPage } from 'next';
import * as React from 'react';
import AdminChart from 'src/screens/AdminChart';

interface DashboardAdminPageProps {
    filter: Partial<MonthFilter>;
}

const DashboardAdminPage: NextPage<DashboardAdminPageProps> = ({ filter }) => {
    return (
        <React.Fragment>
            <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
                <AdminChart filter={filter} />
            </ProtectWrapper>
        </React.Fragment>
    );
};
DashboardAdminPage.getInitialProps = async (ctx): Promise<DashboardAdminPageProps> => {
    return {
        filter: objectHelper.getObjectWithDefault<Partial<MonthFilter>>(ctx.query, {
            month: new Date().getMonth() + 1,
        }),
    };
};
export default DashboardAdminPage;
