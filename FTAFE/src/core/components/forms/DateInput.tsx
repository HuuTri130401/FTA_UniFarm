import { DatePicker, DatePickerProps } from 'antd';
import { RangePickerProps } from 'antd/lib/date-picker';
import moment from 'moment';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type DateInputType = Omit<DatePickerProps, 'label'> & FieldWrapperProps;
interface DateInputProps extends DateInputType {
    format?: string;
}

export const DateInput: React.FC<DateInputProps> = ({ label, name, format = 'YYYY-MM-DD HH:mm:ss', isHiddenError, required, ...rest }) => {
    const { control, setValue, clearErrors } = useFormContext();
    const disabledDate: RangePickerProps['disabledDate'] = (current) => {
        return current && current <= moment().endOf('day');
    };

    return (
        <FieldWrapper name={name} label={label} isHiddenError={isHiddenError} required={required}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => (
                    <DatePicker
                        disabledDate={disabledDate}
                        showTime
                        locale={{
                            timePickerLocale: {},
                            lang: {
                                placeholder: 'Chọn ngày',
                                rangePlaceholder: ['Từ ngày', 'Đến ngày'],
                                today: 'Hôm nay',
                                now: 'Bây giờ',
                                backToToday: 'Trở về hôm nay',
                                ok: 'Đồng ý',
                                clear: 'Xóa',
                                month: 'Tháng',
                                year: 'Năm',
                                timeSelect: 'Chọn thời gian',
                                dateSelect: 'Chọn ngày',
                                monthSelect: 'Chọn tháng',
                                yearSelect: 'Chọn năm',
                                decadeSelect: 'Chọn thập kỷ',
                                yearFormat: 'YYYY',
                                dateFormat: 'DD/MM/YYYY',
                                dayFormat: 'DD',
                                dateTimeFormat: 'DD/MM/YYYY HH:mm:ss',
                                monthBeforeYear: true,
                                previousMonth: 'Tháng trước (PageUp)',
                                nextMonth: 'Tháng sau (PageDown)',
                                previousYear: 'Năm trước (Control + left)',
                                nextYear: 'Năm sau (Control + right)',
                                previousDecade: 'Thập kỷ trước',
                                nextDecade: 'Thập kỷ sau',
                                previousCentury: 'Thế kỷ trước',
                                nextCentury: 'Thế kỷ sau',
                                locale: 'vi_VN',
                            },
                        }}
                        className="w-full"
                        format={format}
                        {...field}
                        value={moment(field.value)}
                        onChange={(value) => {
                            clearErrors();
                            setValue(name, value?.format('YYYY-MM-DD HH:mm:ss'));
                        }}
                        {...rest}
                    />
                )}
            />
        </FieldWrapper>
    );
};
