import { DateInput, DateRangerInput, FormWrapper, TextInput } from '@components/forms';
import { BusinessDayAPI } from '@core/api/business-day.api';
import { CreateBusinessDay } from '@models/business-day';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateBusinessDayModalProps extends ModalProps {}
const defaultValues: CreateBusinessDay = {
    endOfRegister: new Date(),
    name: '',
    openDay: new Date(),
    regiterDay: new Date(),
};
const CreateBusinessDayModal: React.FC<CreateBusinessDayModalProps> = ({ ...rest }) => {
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
        const endOfRegisterTime = new Date(data.endOfRegister).getTime();

        const registerDayTime = new Date(data.regiterDay).getTime();

        const openDayTime = new Date(data.openDay).getTime();

        const msInTwoDays = 2 * 24 * 60 * 60 * 1000;

        const msInDay = 24 * 60 * 60 * 1000;

        const differenceInMs = endOfRegisterTime - registerDayTime;

        if (differenceInMs - msInTwoDays < 0) {
            methods.setError('endOfRegister', {
                message: 'End of register greatThe end date must be 2 days greater than the start date',
                type: 'manual',
            });
            return;
        }
        if (openDayTime - endOfRegisterTime - msInDay < 0) {
            methods.setError('openDay', {
                message: 'Open day > End of register day 1 day',
                type: 'manual',
            });
            return;
        }
        const _data: CreateBusinessDay = {
            ...data,
            regiterDay: new Date(data.regiterDay).toISOString(),
            endOfRegister: new Date(data.endOfRegister).toISOString(),
            openDay: new Date(data.openDay).toISOString(),
        };
        mutate(_data, {
            onSuccess: () => {
                queryClient.invalidateQueries(['businessDays']);
                rest.afterClose && rest.afterClose();
                toast.success('Tạo ngày bán thành công');
            },
        });
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
                        <TextInput name="name" label="name" required />
                        {errors.name && <p style={{ color: 'red' }}>{errors.name.message}</p>}
                    </div>
                    {/* <div>
                        <DateRangerInput
                            label="Register date"
                            startDateLabel="regiterDay"
                            startDateName="regiterDay"
                            endDateLabel="endOfRegister"
                            endDateName="endOfRegister"
                        />
                        {errors.endOfRegister && <p style={{ color: 'red' }}>{errors.endOfRegister.message}</p>}
                    </div> */}
                    <div>
                        <DateInput name="openDay" label="open day" />
                        {errors.openDay && <p style={{ color: 'red' }}>{errors.openDay.message}</p>}
                    </div>
                </form>
            </Modal>
        </FormWrapper>
    );
};
export default CreateBusinessDayModal;
