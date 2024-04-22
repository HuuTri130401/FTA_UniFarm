import { Customer } from '@models/candidate';
import { convertTextToAvatar } from '@utils/string.helper';
import { Descriptions, Image } from 'antd';
import moment from 'moment';
import * as React from 'react';

interface StaffDetailProps {
    customer: Customer;
}

const CustomerDetail: React.FunctionComponent<StaffDetailProps> = ({ customer }) => {
    return (
        <>
            <div className="flex flex-col w-full gap-4">
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={'Basic Information'}
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item label="Avatar" span={1}>
                        <Image
                            height={80}
                            width={80}
                            className="overflow-hidden rounded"
                            src={customer.avatar ? customer.avatar : convertTextToAvatar(customer.firstName)}
                            alt={customer.firstName}
                        />
                    </Descriptions.Item>
                    <Descriptions.Item label="Firtname" span={2}>
                        {customer.firstName}
                    </Descriptions.Item>
                    <Descriptions.Item label="Email" span={1}>
                        {customer.email}
                    </Descriptions.Item>
                    {/* <Descriptions.Item label="Gender" span={1}>
                        {customer.gender}
                    </Descriptions.Item>
                    <Descriptions.Item label="Job" span={2}>
                        {customer.job_title}
                    </Descriptions.Item> */}
                    <Descriptions.Item label="Address" span={3}>
                        {customer.address}
                    </Descriptions.Item>

                    {/* <Descriptions.Item label="Status" span={1}>
                        {customer.status}
                    </Descriptions.Item> */}
                    <Descriptions.Item label="Created at" span={1}>
                        {moment(customer.createdAt).format('DD/MM/YYYY HH:mm')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Last Updated At" span={1}>
                        {moment(customer.updatedAt).format('DD/MM/YYYY HH:mm')}
                    </Descriptions.Item>
                </Descriptions>
            </div>
        </>
    );
};

export default CustomerDetail;
