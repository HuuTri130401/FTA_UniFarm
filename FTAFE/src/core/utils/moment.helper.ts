import moment from 'moment';

export const momentHelper = (date: any) => moment(date).utcOffset(date);
