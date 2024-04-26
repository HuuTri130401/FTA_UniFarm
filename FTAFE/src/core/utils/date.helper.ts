// import 'moment/locale/vi';

import moment from 'moment';

export const getDateFormat = (date: Date) => {
    date.setHours(0);
    return date;
};

export const dateFormat = (date: string) => {
    return moment(date).locale('en').format('LLL');
};
