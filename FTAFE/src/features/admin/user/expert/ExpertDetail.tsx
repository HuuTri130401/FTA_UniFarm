import SkillWeightProgress from '@components/progress/SkillWeightProgress';
import { TableBodyCell, TableHeaderCell } from '@components/tables';
import { TableBuilderDescription } from '@components/tables/TableBuilderDescription';
import { routes } from '@core/routes';
import { ExpertItem } from '@models/expert';
import { SkillLevel } from '@models/skillLevel';
import { momentHelper } from '@utils/moment.helper';
import { convertTextToAvatar } from '@utils/string.helper';
import { Button, Descriptions, Image } from 'antd';
import moment from 'moment';
import Link from 'next/link';
import * as React from 'react';

import UpdateUserModal from '../components/UpdateUserModal';

interface ExpertDetailProps {
    expert: ExpertItem;
}

const ExpertDetail: React.FunctionComponent<ExpertDetailProps> = ({ expert }) => {
    const [openUpdateModal, setOpenUpdateModal] = React.useState<boolean>(false);

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
                    extra={
                        <Button type="primary" onClick={() => setOpenUpdateModal(!openUpdateModal)}>
                            Update
                        </Button>
                    }
                >
                    <Descriptions.Item label="Avatar" span={1}>
                        <Image
                            height={80}
                            width={80}
                            className="rounded overflow-hidden"
                            src={expert.user.avatar ? expert.user.avatar : convertTextToAvatar(expert.user.firstName)}
                            alt={expert.user.firstName}
                        />
                    </Descriptions.Item>
                    <Descriptions.Item label="Fullname" span={2}>
                        {expert.user.firstName}
                    </Descriptions.Item>
                    {/* <Descriptions.Item label="Status" span={1}>
                        {expert.user.status}
                    </Descriptions.Item> */}
                    <Descriptions.Item label="Created at" span={1}>
                        {moment(expert.createdAt).format('DD/MM/YYYY HH:mm')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Last Updated At" span={1}>
                        {moment(expert.updatedAt).format('DD/MM/YYYY HH:mm')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Number of Skill Levels" span={1}>
                        {expert.skillLevels.length}
                    </Descriptions.Item>
                </Descriptions>
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    title={'Skill Levels'}
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item span={3}>
                        <TableBuilderDescription<SkillLevel>
                            data={expert.skillLevels}
                            isLoading={false}
                            rowKey="id"
                            columns={[
                                {
                                    title: () => <TableHeaderCell key="name" sortKey="name" label="Name" />,
                                    width: 500,
                                    key: 'name',
                                    render: ({ ...props }: SkillLevel) => {
                                        return (
                                            <TableBodyCell
                                                label={
                                                    <div className="w-full">
                                                        <a className="text-black">{props.name}</a>
                                                    </div>
                                                }
                                            />
                                        );
                                    },
                                },

                                {
                                    title: () => <TableHeaderCell key="weight" sortKey="weight" label="Weight" />,
                                    width: 500,
                                    key: 'weight',
                                    render: ({ ...props }: SkillLevel) => {
                                        return (
                                            <TableBodyCell
                                                className="line-clamp-4"
                                                label={
                                                    <div className="w-full">
                                                        <SkillWeightProgress total={expert.skillLevels.length} weight={props.weight} />
                                                    </div>
                                                }
                                            />
                                        );
                                    },
                                },
                                {
                                    title: () => <TableHeaderCell key="createAt" sortKey="createAt" label="Create at" />,
                                    width: 500,
                                    key: 'createAt',
                                    render: ({ ...props }: SkillLevel) => {
                                        return (
                                            <TableBodyCell
                                                className="line-clamp-4"
                                                label={
                                                    <div className="w-full">
                                                        <a className="text-black"> {moment(props.createdAt).format('DD/MM/YYYY HH:mm')}</a>
                                                    </div>
                                                }
                                            />
                                        );
                                    },
                                },
                                {
                                    title: () => <TableHeaderCell key="updateAt" sortKey="updateAt" label="Update at" />,
                                    width: 500,
                                    key: 'updateAt',
                                    render: ({ ...props }: SkillLevel) => {
                                        return (
                                            <TableBodyCell
                                                className="line-clamp-4"
                                                label={
                                                    <div className="w-full">
                                                        <a className="text-black">{moment(props.updatedAt).format('DD/MM/YYYY HH:mm')}</a>
                                                    </div>
                                                }
                                            />
                                        );
                                    },
                                },
                            ]}
                        />
                    </Descriptions.Item>
                </Descriptions>
            </div>
            <UpdateUserModal
                currentValue={{
                    avatar: expert.user.avatar,
                    fullName: expert.user.firstName,
                    id: expert.user.id,
                    phone: expert.user.phoneNumber,
                }}
                open={openUpdateModal}
                afterClose={() => setOpenUpdateModal(false)}
            />
        </>
    );
};

export default ExpertDetail;
