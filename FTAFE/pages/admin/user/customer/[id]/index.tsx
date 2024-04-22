import { DashboardHeaderLayout } from '@components/layouts';
import { UserRole } from '@models/user';
import { NextPage } from 'next';
import { ToggleProvider } from 'react-toggle-hook';

interface PageProps {
    id: string;
}

const Page: NextPage<PageProps> = ({ id }) => {
    // const { data } = useQueryCandidateById(id);
    const SAMEPLE_DATA = {
        user: {
            id: '1',
            type: UserRole.CUSTOMER,
            avatar: 'https://images.unsplash.com/photo-1706361635623-6606c945503e?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D',
            email: 'user@example.com',
            fullName: 'John Doe',
            phone: '123456789',
            status: 'active',
            createdAt: new Date().toString(),
            updatedAt: new Date().toString(),
            isDeleted: false,
            address:
                'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only ive centuries, but also the leap into electronic typesetting remaining essentially unchanged',
            gender: 'male',
            username: 'john123',
            password: '123456789',
            job_title: ' lorem',
        },
    };

    return (
        // <ProtectWrapper acceptRoles={[UserRole.ADMIN]}>
        <ToggleProvider>
            <DashboardHeaderLayout title="Customer Detail">
                <></>
                {/* <CustomerDetail customer={SAMEPLE_DATA} /> */}
            </DashboardHeaderLayout>
        </ToggleProvider>
        // </ProtectWrapper>
    );
};

Page.getInitialProps = async (ctx): Promise<PageProps> => {
    const id = String(ctx.query.id || '');
    return {
        id,
    };
};

export default Page;
