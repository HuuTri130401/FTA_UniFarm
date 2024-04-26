import { CheckCircleFilled, CloseCircleOutlined, FileDoneOutlined, SyncOutlined } from '@ant-design/icons';

export const STATUS_INTERVIEW: any = {
    CANCEL: {
        label: 'Cancel',
        color: 'default',
        icon: <CloseCircleOutlined />,
    },
    DONE: {
        label: 'Done',
        color: 'green',
        icon: <FileDoneOutlined />,
    },
    PENDING: {
        label: 'Waiting Accept',
        color: 'blue',
        icon: <SyncOutlined />,
    },
    ACCEPTED: {
        label: 'Waiting Interview',
        color: 'green',
        icon: <CheckCircleFilled />,
    },
    REJECTED: {
        label: 'Rejected',
        color: 'red',
        icon: <CloseCircleOutlined />,
    },
};
