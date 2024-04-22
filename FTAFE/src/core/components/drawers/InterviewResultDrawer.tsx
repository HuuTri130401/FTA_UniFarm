import RateInput from '@components/forms/RateInput';
import { IV1UpdateCriteriaResult } from '@core/api/criteria-result.api';
import { IV1UpdateInterviewResult } from '@core/api/interview.api';
import { joiResolver } from '@hookform/resolvers/joi';
import { useCriteriaResultUpdateResultMutation } from '@hooks/api/criteriaResult.hook';
import { useInterviewUpdateDoneStatusMutation, useInterviewUpdateResultMutation, useQueryInterviewById } from '@hooks/api/interview.hook';
import { InterviewStatus } from '@models/interview';
import { Button, Drawer, DrawerProps, Popconfirm, Tabs } from 'antd';
import clsx from 'clsx';
import joi from 'joi';
import * as React from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

import { FormErrorMessage, FormWrapper, TextareaField } from '..';

interface InterviewResultDrawerProps extends DrawerProps {
    interviewResultId: string;
}

interface InterviewResultForm extends IV1UpdateInterviewResult, IV1UpdateCriteriaResult {}

const defaultValues: InterviewResultForm = {
    id: '',
    summary: '',
    strength: '',
    weakness: '',
    point: 0,
    criteriaResults: [],
};

const InterviewResultDrawer: React.FunctionComponent<InterviewResultDrawerProps> = ({ interviewResultId, ...rest }) => {
    const methods = useForm<InterviewResultForm>({
        defaultValues,
        resolver: joiResolver(
            joi.object({
                id: joi.any().required(),
                summary: joi.string().required().label('Summary'),
                point: joi.number().min(0).max(5).required().label('Point'),
                strength: joi.string().required().label('Strength'),
                weakness: joi.string().required().label('Weakness'),
                criteriaResults: joi.array().items(
                    joi.object({
                        id: joi.any(),
                        mark: joi.number().min(0).max(5).required().label('Mark'),
                        comment: joi.string().required().label('Comment'),
                    })
                ),
            })
        ),
    });

    React.useEffect(() => {
        if (Object.keys(methods.formState.errors).length > 0) {
            console.log(methods.formState.errors);
            toast.error('Please fill all field');
        }
    }, [methods.formState.errors]);

    const { data, refetch, isSuccess } = useQueryInterviewById(interviewResultId);

    React.useEffect(() => {
        const { summary, strength, weakness, point, criteriaResults } = data;

        const sortedCriteriaResults = criteriaResults
            .sort((a, b) => a.name.localeCompare(b.name))
            .map((item) => ({ id: item.id, mark: item.mark, comment: item.comment }));
        methods.setValue('id', interviewResultId);
        methods.setValue('criteriaResults', sortedCriteriaResults);
        methods.setValue('summary', summary || '');
        methods.setValue('point', point || 0);
        methods.setValue('strength', strength || '');
        methods.setValue('weakness', weakness || '');
    }, [data, isSuccess]);

    const { mutationInterviewUpdateResultAsync, isLoading: updateResultLoading } = useInterviewUpdateResultMutation();
    const { mutationCriteriaResultUpdateResultAsync, isLoading: updateCriteriaResultLoading } = useCriteriaResultUpdateResultMutation();

    const onSubmit = async (data: InterviewResultForm) => {
        methods.clearErrors('criteriaResults');

        try {
            const { criteriaResults, ...interviewResult } = data;

            await mutationCriteriaResultUpdateResultAsync({ criteriaResults });
            await mutationInterviewUpdateResultAsync(interviewResult);

            toast.success('Update interview result successfully');
            refetch();
        } catch (err) {}
    };

    const { mutationInterviewUpdateDoneStatus, isLoading } = useInterviewUpdateDoneStatusMutation();

    // const onMarkAsDone = async () => {
    //     mutationInterviewUpdateDoneStatus(interviewResultId);
    // };

    const [isDisabled, setIsDisabled] = React.useState(false);

    React.useEffect(() => {
        if (data.status === InterviewStatus.DONE) {
            setIsDisabled(true);
            return;
        }

        setIsDisabled(false);
    }, [data, isSuccess]);

    return (
        <FormWrapper methods={methods}>
            <Drawer
                {...rest}
                title={'Evaluation Criteria & Overall'}
                extra={
                    <div className="flex gap-2">
                        {isDisabled ? (
                            <p className="m-0 text-base font-semibold whitespace-nowrap text-red">Interview is done, no longer editable</p>
                        ) : (
                            <>
                                {data.summary && (
                                    <Popconfirm
                                        title={
                                            <span>
                                                If you mark as done, the interview no longer editable
                                                <br /> And make sure you already click &quot;Save&quot;
                                            </span>
                                        }
                                        okButtonProps={{ loading: isLoading }}
                                        onConfirm={() => {
                                            mutationInterviewUpdateDoneStatus(interviewResultId, {
                                                onSuccess: () => {
                                                    refetch();
                                                },
                                            });
                                        }}
                                    >
                                        <Button danger type="primary" htmlType="button">
                                            Mark as done
                                        </Button>
                                    </Popconfirm>
                                )}
                                <Button
                                    htmlType="submit"
                                    type="primary"
                                    disabled={isDisabled}
                                    loading={updateResultLoading || updateCriteriaResultLoading}
                                    onClick={() => methods.handleSubmit(onSubmit)()}
                                >
                                    Save
                                </Button>
                            </>
                        )}
                    </div>
                }
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full">
                    <div className="flex flex-col w-full gap-4 ">
                        {Boolean(data.criteriaResults.length) && (
                            <React.Fragment>
                                <Tabs
                                    tabPosition="left"
                                    className="h-[312px] [&>.ant-tabs-nav>.ant-tabs-nav-wrap>.ant-tabs-nav-list>.ant-tabs-tab-active]:!bg-primary [&>.ant-tabs-nav>.ant-tabs-nav-wrap>.ant-tabs-nav-list>.ant-tabs-tab-active>.ant-tabs-tab-btn]:!text-white"
                                >
                                    {data.criteriaResults
                                        .sort((a, b) => {
                                            if (a.name < b.name) {
                                                return -1;
                                            }
                                            if (a.name > b.name) {
                                                return 1;
                                            }
                                            return 0;
                                        })
                                        .map((item, index) => (
                                            <Tabs.TabPane
                                                tab={
                                                    <span
                                                        className={clsx({
                                                            'text-red font-semibold': Boolean(methods.formState.errors.criteriaResults?.[index]),
                                                        })}
                                                    >
                                                        {item.name}
                                                    </span>
                                                }
                                                key={`criteria-${item.id}`}
                                            >
                                                <div className="flex w-full gap-2">
                                                    <div className="flex flex-col w-full gap-3">
                                                        <p className="m-0 text-lg font-semibold">{item.name}</p>
                                                        <RateInput name={`criteriaResults.${index}.mark`} label="Mark" disabled={isDisabled} />
                                                        <div>
                                                            <TextareaField
                                                                name={`criteriaResults.${index}.comment`}
                                                                label="Comment"
                                                                rows={6}
                                                                disabled={isDisabled}
                                                            />
                                                        </div>
                                                    </div>
                                                </div>
                                            </Tabs.TabPane>
                                        ))}
                                </Tabs>
                                <div className="w-full my-5 bg-gray-900/20 h-0.5" />
                            </React.Fragment>
                        )}
                    </div>

                    <div className="flex flex-col w-full gap-4">
                        <RateInput name="point" label="Point" disabled={isDisabled} />
                        <TextareaField name="strength" label="Strength" rows={4} disabled={isDisabled} />
                        <TextareaField name="weakness" label="Weakness" rows={4} disabled={isDisabled} />
                        <TextareaField name="summary" label="Summary" rows={4} disabled={isDisabled} />

                        <div className="flex justify-end col-span-full"></div>
                    </div>
                </form>
            </Drawer>
        </FormWrapper>
    );
};

export default InterviewResultDrawer;
