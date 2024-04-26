import { Tag } from 'antd';
import moment from 'moment';
import * as React from 'react';

interface NewTagProps {
    value: string;
}

const NewTag: React.FunctionComponent<NewTagProps> = ({ value }) => {
    const [isShow, setIsShow] = React.useState(false);

    React.useEffect(() => {
        const isValidDate = moment(value).isValid();

        if (!isValidDate) {
            return;
        }

        const now = moment();

        const date = moment(value);

        const diff = now.diff(date, 'days');

        if (diff <= 1) {
            setIsShow(true);
        }
    }, [value]);

    if (!isShow) {
        return <></>;
    }

    return <Tag color={'purple'}>New</Tag>;
};

export default NewTag;
