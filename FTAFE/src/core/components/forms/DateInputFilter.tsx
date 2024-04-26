import { DatePicker, DatePickerProps } from 'antd';
import moment, { Moment } from 'moment';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type DateInputType = Omit<DatePickerProps, 'label'> & FieldWrapperProps;
interface DateInputProps extends DateInputType {
    format?: string;
    disabledDate?: (current: Moment | null) => boolean; // Add the disabledDate prop
}

export const DateInputFilter: React.FC<DateInputProps> = ({ label, name, format = 'YYYY-MM-DD', isHiddenError, required, disabledDate, ...rest }) => {
    const { control, setValue, clearErrors } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} isHiddenError={isHiddenError} required={required}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => (
                    <DatePicker
                        {...rest}
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
                        value={moment(field.value)}
                        onChange={(value) => {
                            clearErrors();
                            setValue(name, value?.format('YYYY-MM-DD HH:mm:ss'));
                        }}
                        disabledDate={disabledDate} // Pass the disabledDate function
                    />
                )}
            />
        </FieldWrapper>
    );
};
