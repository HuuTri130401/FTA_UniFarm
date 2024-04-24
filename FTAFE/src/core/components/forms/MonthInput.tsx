import { DatePicker, DatePickerProps } from 'antd';
import moment from 'moment';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type DateInputType = Omit<DatePickerProps, 'label'> & FieldWrapperProps;
interface MonthInputProps extends DateInputType {}

export const MonthInput: React.FC<MonthInputProps> = ({ label, name, isHiddenError, required, ...rest }) => {
    const { control, setValue, clearErrors } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} isHiddenError={isHiddenError} required={required}>
            <Controller
                control={control}
                name={name}
                render={({ field }) => {
                    return (
                        <DatePicker
                            picker="month"
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
                            {...field}
                            value={field ? moment(`${moment().year()}-${field.value}-01`, 'YYYY-MM-DD') : undefined}
                            onChange={(value) => {
                                clearErrors();
                                if (value) {
                                    console.log('value:', value.month());
                                    setValue(name, value.month() + 1);
                                } else {
                                    setValue(name, undefined);
                                }
                            }}
                            {...rest}
                        />
                    );
                }}
            />
        </FieldWrapper>
    );
};
