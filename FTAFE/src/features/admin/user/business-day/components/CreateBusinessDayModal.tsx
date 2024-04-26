import { FormWrapper, TextInput } from '@components/forms';
import { DateInputFilter } from '@components/forms/DateInputFilter';
import { BusinessDayAPI } from '@core/api/business-day.api';
import { CreateBusinessDay } from '@models/business-day';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import moment from 'moment';
import React from 'react'; // Add the missing import statement for React
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateBusinessDayModalProps extends ModalProps {
    listOpenDate?: string[];
}
const defaultValues: CreateBusinessDay = {
    name: '',
    openDay: new Date(),
    endOfRegister: new Date(),
    regiterDay: new Date(),
};
const CreateBusinessDayModal: React.FC<CreateBusinessDayModalProps> = ({ listOpenDate, ...rest }) => {
    const methods = useForm({
        defaultValues,
    });
    const queryClient = useQueryClient();

    const { errors } = methods.formState;

    const { mutate } = useMutation(async (data: CreateBusinessDay) => await BusinessDayAPI.createOne(data));

    const onSubmit = async (data: CreateBusinessDay) => {
        if (data.name === '') {
            methods.setError('name', {
                type: 'manual',
                message: 'Tên không được để trống',
            });
            return;
        }

        const _data: CreateBusinessDay = {
            ...data,
            openDay: new Date(data.openDay).toISOString(),
        };
        mutate(_data, {
            onSuccess: () => {
                queryClient.invalidateQueries(['businessDays']);
                rest.afterClose && rest.afterClose();
                toast.success(' Tạo ngày bán thành công');
            },
            onError: (err: any) => {
                const error = err.data.errors[0].message || '';

                toast.error(error);
            },
        });
    };

    const isDateDisabledAndBeforeToday = (current: moment.Moment | null): boolean => {
        if (!current) return true;
        const currentDate: string = current.format('YYYY-MM-DDT00:00:00');

        return listOpenDate?.includes(currentDate) || current.isBefore(moment().startOf('day'));
    };

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title=" Tạo ngày bán"
                width={600}
                footer={[
                    <Button key="close" type="default" onClick={rest.onCancel}>
                        Huỷ
                    </Button>,
                    <Button key="create" type="primary" onClick={() => methods.handleSubmit(onSubmit)()}>
                        Lưu
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col w-full gap-2">
                    <div>
                        <TextInput name="name" label="Tên" required />
                        {errors.name && <p style={{ color: 'red' }}>{errors.name.message}</p>}
                    </div>
                    <div>
                        {/* <DateInput name="openDay" label="Ngày mở bán" format="YYYY-MM-DD" /> */}
                        <DateInputFilter name="openDay" label="Ngày mở bán" format="YYYY-MM-DD" disabledDate={isDateDisabledAndBeforeToday} />
                        {errors.openDay && <p style={{ color: 'red' }}>{errors.openDay.message}</p>}
                    </div>
                </form>
            </Modal>
        </FormWrapper>
    );
};
export default CreateBusinessDayModal;
