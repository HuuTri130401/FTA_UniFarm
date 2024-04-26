import { routes } from '@core/routes';
import { Button, Result } from 'antd';
import Link from 'next/link';
import * as React from 'react';

interface NotFoundPageProps {}

const NotFoundPage: React.FC<NotFoundPageProps> = () => {
    return (
        <div className="h-screen w-screen flex justify-center items-center">
            <Result
                status="404"
                title="Developing"
                subTitle="Sorry, the page you visited is under further development."
                extra={
                    <Link href={routes.home()}>
                        <Button type="primary">Back Home</Button>
                    </Link>
                }
            />
        </div>
    );
};

export default NotFoundPage;
