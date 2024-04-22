import { InterviewStatus } from '@models/interview';
import { Tag } from 'antd';
import * as React from 'react';

interface InterviewStatusTagProps {
    value: InterviewStatus;
}

const InterviewStatusTag: React.FunctionComponent<InterviewStatusTagProps> = ({ value }) => {
    const handleColor = (status: InterviewStatus) => {
        switch (status) {
            case InterviewStatus.ACCEPTED:
                return 'green';
            case InterviewStatus.REJECTED:
                return 'red';
            case InterviewStatus.DONE:
                return 'blue';
            case InterviewStatus.PENDING:
                return 'orange';
            case InterviewStatus.CANCEL:
                return 'red';
            default:
                return 'green';
        }
    };

    const handleText = (status: InterviewStatus) => {
        switch (status) {
            case InterviewStatus.ACCEPTED:
                return 'Accepted';
            case InterviewStatus.REJECTED:
                return 'Rejected';
            case InterviewStatus.DONE:
                return 'Done';
            case InterviewStatus.PENDING:
                return 'Pending';
            case InterviewStatus.CANCEL:
                return 'Cancel';
            default:
                return 'Accepted';
        }
    };

    return <Tag color={handleColor(value)}>{handleText(value)}</Tag>;
};

export default InterviewStatusTag;
