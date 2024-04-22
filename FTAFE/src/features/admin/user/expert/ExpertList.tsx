import { TextInput } from '@components/forms';
import FormFilterWrapper from '@components/forms/FormFilterWrapper';
import { SelectOriginInput } from '@components/forms/SelectOriginInput';
import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { IV1GetFilterExpert } from '@core/api/expert.api';
// import { expertApi, IV1GetFilterExpert } from '@core/api/expert.api';
import { PlusIcon } from '@heroicons/react/24/outline';
import { useQueryExpertFilter } from '@hooks/api/expert.hook';
import { useQuerySkillFilter } from '@hooks/api/skill.hook';
import { ExpertItem } from '@models/expert';
import { SortOrder } from '@models/interface';
import { stringHelper } from '@utils/index';
// import { ExpertList } from '@models/expert';
import { Image } from 'antd';
import { useRouter } from 'next/router';
import * as React from 'react';

interface ExpertListProps {
    filter: Partial<IV1GetFilterExpert>;
}

const ExpertList: React.FunctionComponent<ExpertListProps> = ({ filter }) => {
    const router = useRouter();

    const { data: skills } = useQuerySkillFilter({ name: '', order: SortOrder.ASC, orderBy: 'id', pageSize: 999999 }, false);

    const { data, isLoading } = useQueryExpertFilter(filter);
    return (
        <div className="flex flex-col w-full gap-2">
            <div className="flex flex-col items-end w-full gap-2 ">
                <button
                    // onClick={() => router.push(routes.admin.user.expert.create())}
                    className="flex items-center gap-1 px-3 py-1 text-white duration-300 hover:text-white hover:bg-primary/90 bg-primary"
                >
                    <PlusIcon className="w-5 h-5 text-white" />
                    <span>Create New Hub</span>
                </button>
            </div>

            <FormFilterWrapper<IV1GetFilterExpert> defaultValues={{ ...filter }}>
                <div className="w-56">
                    <TextInput name="name" label="Name" />
                </div>
                <div className="w-56">
                    <TextInput name="email" label="Email" />
                </div>
                <div className="min-w-[360px]">
                    <SelectOriginInput
                        label="Skill Level"
                        name="skillLevels"
                        mode="multiple"
                        className="w-full"
                        options={skills.map((skill) => {
                            const options = skill.skillLevels
                                .sort((a, b) => a.weight - b.weight)
                                .map((item) => ({
                                    label: `${skill.name} - ${item.name}`,
                                    value: item.id,
                                }));
                            return {
                                label: skill.name,
                                options,
                            };
                        })}
                    />
                </div>
            </FormFilterWrapper>

            <TableBuilder<ExpertItem>
                rowKey="id"
                isLoading={isLoading}
                data={data}
                columns={[
                    {
                        title: () => <TableHeaderCell key="avatar" sortKey="avatar" label="Avatar" />,
                        width: 400,
                        key: 'avatar',
                        render: ({ ...props }: ExpertItem) => (
                            <TableBodyCell
                                label={
                                    <Image
                                        alt="avatar"
                                        width={64}
                                        height={64}
                                        className="overflow-hidden rounded"
                                        src={props.user.avatar ? props.user.avatar : stringHelper.convertTextToAvatar(props.user.email)}
                                    />
                                }
                            />
                        ),
                    },
                    {
                        title: () => <TableHeaderCell key="fullName" sortKey="fullName" label="Fullname" />,
                        width: 400,
                        key: 'fullName',
                        render: ({ ...props }: ExpertItem) => (
                            // <TableBodyCell label={<Link href={routes.admin.user.expert.detail(props.id)}>{props.user.fullName}</Link>} />
                            <></>
                        ),
                    },
                    {
                        title: () => <TableHeaderCell key="email" sortKey="email" label="Email" />,
                        width: 400,
                        key: 'email',
                        render: ({ ...props }: ExpertItem) => <TableBodyCell label={props.user.email} />,
                    },
                    {
                        title: () => <TableHeaderCell key="skillLevels" sortKey="skillLevels" label="Skill Levels" />,
                        width: 400,
                        key: 'skillLevels',
                        render: ({ ...props }: ExpertItem) => <TableBodyCell label={props.skillLevels.length.toString()} />,
                    },

                    {
                        title: () => <TableHeaderCell key="" sortKey="" label="" />,
                        width: 400,
                        key: 'action',
                        render: ({ ...props }) => {
                            // return <TableBodyCell label={<Link href={routes.admin.user.expert.detail(props.id)}>View detail</Link>} />;
                            return <></>;
                        },
                    },
                ]}
            />
        </div>
    );
};

export default ExpertList;
