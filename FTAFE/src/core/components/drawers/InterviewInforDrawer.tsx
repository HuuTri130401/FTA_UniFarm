import SkillWeightProgress from '@components/progress/SkillWeightProgress';
import { TableBuilderDescription } from '@components/tables/TableBuilderDescription';
import InterviewStatusTag from '@components/tag/InterviewStatusTag';
import { routes } from '@core/routes';
import { useQueryInterviewById } from '@hooks/api/interview.hook';
import { CriteriaResult } from '@models/criteriaResult';
import { InterviewStatus } from '@models/interview';
import { SkillLevelItem } from '@models/skillLevel';
import { stringHelper } from '@utils/index';
import { momentHelper } from '@utils/moment.helper';
import { Button, Descriptions, Drawer, DrawerProps, Popover } from 'antd';
import * as React from 'react';

import { TableBodyCell, TableHeaderCell } from '..';

interface InterviewInfoDrawerProps extends DrawerProps {
    interviewResultId: string;
}

const InterviewInfoDrawer: React.FunctionComponent<InterviewInfoDrawerProps> = ({ interviewResultId, ...rest }) => {
    const { data, refetch } = useQueryInterviewById(interviewResultId);

    React.useEffect(() => {
        refetch();
    }, [rest.open]);

    return (
        <Drawer {...rest} title={'Interview Information'}>
            <div className="flex flex-col w-full gap-4">
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    bordered
                    title={'Basic Information'}
                    className="p-4 bg-white rounded-lg"
                    extra={
                        <div className="flex gap-2">
                            {data?.status === InterviewStatus.DONE && (
                                <a href={routes.expert.interview.detail(interviewResultId)} target="_blank" rel="noopener noreferrer">
                                    <Button type="primary">View result</Button>
                                </a>
                            )}

                            <a href={routes.expert.interview.cv(interviewResultId)} target="_blank" rel="noopener noreferrer">
                                <Button type="default">View Candidate CV</Button>
                            </a>
                        </div>
                    }
                >
                    <Descriptions.Item label="Job Title" span={1}>
                        {data.jobLevel.job.title}
                    </Descriptions.Item>
                    <Descriptions.Item label="Job Level" span={1}>
                        {data.jobLevel.title}
                    </Descriptions.Item>
                    <Descriptions.Item label="Created at" span={1}>
                        {momentHelper(data.createdAt).format('DD/MM/YYYY HH:mm')}
                    </Descriptions.Item>
                    <Descriptions.Item label="Last Updated At" span={1}>
                        {momentHelper(data.updatedAt).format('DD/MM/YYYY HH:mm')}
                    </Descriptions.Item>

                    <Descriptions.Item label="Duration" span={1}>
                        {data.duration * 30} minutes
                    </Descriptions.Item>
                    <Descriptions.Item label="Candidate" span={1}>
                        {data.candidate.user.firstName}
                    </Descriptions.Item>
                    <Descriptions.Item label="Expert" span={1}>
                        {data.expert.user.lastName}
                    </Descriptions.Item>
                    <Descriptions.Item label="Price" span={1}>
                        {stringHelper.formatMoneyVND(data.jobLevel.price)}
                    </Descriptions.Item>
                    <Descriptions.Item label="Status" span={1}>
                        <InterviewStatusTag value={data.status} />
                    </Descriptions.Item>
                    <Descriptions.Item label="Strength" span={3}>
                        {data.strength}
                    </Descriptions.Item>
                    <Descriptions.Item label="Weakness" span={3}>
                        {data.weakness}
                    </Descriptions.Item>
                    <Descriptions.Item label="Summary" span={3}>
                        {data.summary}
                    </Descriptions.Item>
                </Descriptions>
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    title={'Skill Level Required'}
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item span={3}>
                        <TableBuilderDescription<SkillLevelItem>
                            data={data.jobLevel.skillLevels}
                            isLoading={false}
                            rowKey="id"
                            columns={[
                                {
                                    title: () => <TableHeaderCell key="name" sortKey="name" label="Name" />,
                                    width: 500,
                                    key: 'name',
                                    render: ({ ...props }: SkillLevelItem) => {
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
                                    key: 'weight',
                                    width: 500,
                                    render: ({ ...props }: SkillLevelItem) => {
                                        return (
                                            <TableBodyCell
                                                className="line-clamp-4"
                                                label={<SkillWeightProgress total={props.skill.skillLevels.length} weight={props.weight} />}
                                            />
                                        );
                                    },
                                },
                            ]}
                        />
                    </Descriptions.Item>
                </Descriptions>
                <Descriptions
                    labelStyle={{
                        fontWeight: 'bold',
                    }}
                    title={'Criteria Description'}
                    className="p-4 bg-white rounded-lg"
                >
                    <Descriptions.Item span={3}>
                        <TableBuilderDescription<CriteriaResult>
                            data={data.criteriaResults}
                            isLoading={false}
                            rowKey="id"
                            columns={[
                                {
                                    title: () => <TableHeaderCell key="name" sortKey="name" label="Name" />,
                                    width: 300,
                                    key: 'name',
                                    render: ({ ...props }: CriteriaResult) => {
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
                                    title: () => <TableHeaderCell key="description" sortKey="description" label="Description" />,
                                    key: 'description',
                                    render: ({ ...props }: CriteriaResult) => {
                                        return (
                                            <TableBodyCell
                                                className="line-clamp-4"
                                                label={
                                                    <Popover content={<p className="max-w-3xl">{props.description}</p>}>{props.description}</Popover>
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
        </Drawer>
    );
};

export default InterviewInfoDrawer;
