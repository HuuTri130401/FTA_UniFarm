import { TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { StaffType } from '@core/api/staff.api';
import { StationAPI } from '@core/api/station.api';
import ListStaffNotWorkingModal from '@features/staff/components/ListStaffNotWorkingModal';
import { Staff, Station } from '@models/staff';
import { useQuery } from '@tanstack/react-query';
import { convertTextToAvatar } from '@utils/string.helper';
import { Badge, Button, Descriptions, Image } from 'antd';
import { useState } from 'react';
import moment from 'moment';
interface StationsDetailProps {
    value: Station;
}

const StationsDetail: React.FC<StationsDetailProps> = ({ value }) => {
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['station-staff', value && value.id],
        queryFn: async () => {
            const res = await StationAPI.getStaff(value && value.id);
            setTotalItem(res?.payload.length);
            return res;
        },
    });
    const staffList: Staff[] = data?.payload;
    const [openState, setOpenState] = useState(false);
    return (
        <div className="flex flex-col w-full gap-4">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={'Thông tin trạm'}
                className="p-4 bg-white rounded-lg"
            >
                <Descriptions.Item label="Ảnh đại diện" span={1}>
                    <Image
                        height={80}
                        width={80}
                        className="rounded overflow-hidden"
                        src={value && value.image ? value.image : convertTextToAvatar(value ? value.name : '')}
                        alt={value && value.name ? value.name : ''}
                    />
                </Descriptions.Item>
                <Descriptions.Item label="Mô tả" span={3}>
                    {value?.description}
                </Descriptions.Item>
                <Descriptions.Item label="Mã" span={1}>
                    {value?.code}
                </Descriptions.Item>

                <Descriptions.Item label="Trạng thái" span={1}>
                    <Badge
                        status={value?.status === 'Active' ? 'success' : 'error'}
                        text={value?.status === 'Active' ? 'Hoạt động' : 'Không hoạt động'}
                    />
                </Descriptions.Item>
                <Descriptions.Item label="Thời gian tạo" span={2}>
                    {moment(value?.createdAt).format('DD/MM/YYYY HH:mm:ss')}
                </Descriptions.Item>
            </Descriptions>
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={'Danh sách nhân viên'}
                className="p-4 bg-white rounded-lg"
                extra={
                    <Button
                        onClick={() => {
                            setOpenState(!openState);
                        }}
                    >
                        Thêm nhân viên
                    </Button>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Staff>
                        rowKey="id"
                        isLoading={isLoading}
                        data={staffList}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="avatar" sortKey="avatar" label="Ảnh đại diện" />,
                                width: 400,
                                key: 'firstName',
                                render: ({ ...props }: Staff) => (
                                    <Image
                                        height={80}
                                        width={80}
                                        className="rounded overflow-hidden"
                                        src={
                                            props.avatar
                                                ? props.avatar
                                                : 'https://static.vecteezy.com/system/resources/previews/009/292/244/original/default-avatar-icon-of-social-media-user-vector.jpg'
                                        }
                                        alt={props.firstName}
                                    />
                                ),
                            },
                            {
                                title: () => <TableHeaderCell key="firstName" sortKey="firstName" label="Tên" />,
                                width: 400,
                                key: 'firstName',
                                render: ({ ...props }: Staff) => <p className="m-0">{props.firstName}</p>,
                            },
                            {
                                title: () => <TableHeaderCell key="lastName" sortKey="lastName" label="Họ" />,
                                width: 400,
                                key: 'lastName',
                                render: ({ ...props }: Staff) => <p className="m-0">{props.lastName}</p>,
                            },
                            {
                                title: () => <TableHeaderCell key="email" sortKey="email" label="email" />,
                                width: 400,
                                key: 'email',
                                render: ({ ...props }: Staff) => <p className="m-0">{props.email}</p>,
                            },
                            {
                                title: () => <TableHeaderCell key="phoneNumber" sortKey="phoneNumber" label="Số điện thoại" />,
                                width: 400,
                                key: 'phoneNumber',
                                render: ({ ...props }: Staff) => <p className="m-0">{props.phoneNumber}</p>,
                            },
                        ]}
                    />
                </div>
            </Descriptions>
            <ListStaffNotWorkingModal
                staffType={StaffType.station}
                stationId={value && value.id}
                afterClose={() => {
                    setOpenState(false);
                }}
                open={openState}
                onCancel={() => {
                    setOpenState(false);
                }}
            />
        </div>
    );
};

export default StationsDetail;
