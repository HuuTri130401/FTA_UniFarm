import { DashboardHeaderLayout } from '@components/layouts';
import { ProtectWrapper } from '@components/wrappers';
// import AddStaff from '@features/admin/user/staff/AddStaff';
import { UserRole } from '@models/user';
import * as React from 'react';

interface AddExpertPageProps {}

const AddExpertPage: React.FunctionComponent<AddExpertPageProps> = () => {
    return (
        <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
            <DashboardHeaderLayout title="Create Staff">
                {/* <AddStaff /> */}
                <></>
            </DashboardHeaderLayout>
        </ProtectWrapper>
    );
};

export default AddExpertPage;
