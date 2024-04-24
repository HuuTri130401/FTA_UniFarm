import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { StaffType, staffApi } from '@core/api/staff.api';
import { useDebounce } from '@hooks/useDebounce';
import { Staff } from '@models/staff';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { convertTextToAvatar } from '@utils/string.helper';
import { Button, DatePicker, Descriptions, Image, Input, Modal, ModalProps } from 'antd';
import moment from 'moment';
import { useState } from 'react';
import { toast } from 'react-toastify';
const { Search } = Input;
const { RangePicker } = DatePicker;

interface ListStaffNotWorkingModalProps extends ModalProps {
    staffType: StaffType;
    collectId?: string;
    stationId?: string;
}
const ListStaffNotWorkingModal: React.FunctionComponent<ListStaffNotWorkingModalProps> = ({ staffType, collectId, stationId, ...rest }) => {
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['staffNotWorking', staffType],
        queryFn: async () => {
            const res = await staffApi.getStaffNotWorking(staffType);
            setTotalItem(res?.payload.length ? res?.payload.length : 0);
            return res;
        },
    });
    const addToCollectMutation = useMutation(async (id: string) => collectId && (await staffApi.addStaffToCollect(id, collectId)));
    const addToStationMutation = useMutation(async (id: string) => stationId && (await staffApi.addStaffToStation(id, stationId)));
    const queryClient = useQueryClient();

    const [searchText, setSearchText] = useState<string>('');

    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });

    const listStaff: Staff[] = (data && data?.payload) || [];

    const filterData =
        listStaff.filter(
            (i) =>
                i.firstName.toLowerCase().includes(debouncedValue.toLowerCase()) ||
                i.lastName.toLowerCase().includes(debouncedValue.toLowerCase()) ||
                i.email.toLowerCase().includes(debouncedValue.toLowerCase()) ||
                i.phoneNumber?.toLowerCase().includes(debouncedValue.toLowerCase())
        ) || [];

    return (
        <Modal width={1400} {...rest}>
            <Descriptions
                title="Danh sách nhân viên"
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                className="p-4 bg-white rounded-lg"
            >
                <div className="flex flex-col w-full gap-2">
                    <Search
                        placeholder="Tìm kiếm"
                        allowClear
                        enterButton="Tìm kiếm"
                        size="middle"
                        onChange={(e) => setSearchText(e.target.value)} // Update search text
                        style={{ marginBottom: '1rem', marginTop: '1rem' }}
                    />{' '}
                    {/* <RangePicker defaultValue={[moment('2015/01/01', 'YYYY/MM/DD'), moment('2015/01/01', 'YYYY/MM/DD')]} format={'YYYY/MM/DD'} /> */}
                    <TableBuilder<Staff>
                        rowKey="id"
                        isLoading={isLoading}
                        data={filterData}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="image" sortKey="image" label="Hình ảnh" />,
                                width: 400,
                                key: 'image',
                                render: ({ ...props }: Staff) => (
                                    <TableBodyCell
                                        label={
                                            <Image
                                                alt=""
                                                width={64}
                                                height={64}
                                                className="overflow-hidden rounded"
                                                src={props.avatar ? props.avatar : convertTextToAvatar(props.firstName)}
                                            />
                                        }
                                    />
                                ),
                            },
                            {
                                title: () => <TableHeaderCell key="firstName" label="Tên" />,
                                width: 400,
                                key: 'firstName',
                                render: ({ ...props }: Staff) => <span>{props.firstName}</span>,
                                sorter: (a, b) => a.firstName.localeCompare(b.firstName),
                            },
                            {
                                title: () => <TableHeaderCell key="lastName" label="Họ" />,
                                width: 400,
                                key: 'lastName',
                                render: ({ ...props }: Staff) => <span>{props.lastName}</span>,
                                sorter: (a, b) => a.lastName.localeCompare(b.lastName),
                            },
                            {
                                title: () => <TableHeaderCell key="email" label="email" />,
                                width: 400,
                                key: 'email',
                                render: ({ ...props }: Staff) => <span>{props.email}</span>,
                                sorter: (a, b) => a.email.localeCompare(b.email),
                            },
                            {
                                title: () => <TableHeaderCell key="phoneNumber" label="Số điện thoại" />,
                                width: 300,
                                key: 'phoneNumber',
                                render: ({ ...props }: Staff) => <span>{props.phoneNumber}</span>,
                                sorter: (a, b) => a.phoneNumber?.localeCompare(b.phoneNumber),
                            },
                            {
                                title: () => <TableHeaderCell key="createdAt" label="Ngày tạo" />,
                                width: 400,
                                key: 'createdAt',
                                render: ({ ...props }: Staff) => <p className="m-0">{moment(props.createdAt).format('DD/MM/YYYY HH:mm:ss')}</p>,
                                sorter: (a, b) => moment(a.createdAt).valueOf() - moment(b.createdAt).valueOf(),
                            },
                            {
                                title: () => <TableHeaderCell key="action" label="" />,
                                width: 400,
                                key: 'action',
                                render: ({ ...props }: Staff) => (
                                    <Button
                                        onClick={() => {
                                            if (stationId) {
                                                addToStationMutation.mutateAsync(props.id, {
                                                    onSuccess: () => {
                                                        toast.success('Thêm nhân viên thành công');
                                                        rest.afterClose && rest.afterClose();
                                                        queryClient.invalidateQueries(['staffNotWorking', staffType]);
                                                        collectId && queryClient.invalidateQueries([['collected-hub-staff', collectId]]);
                                                        stationId && queryClient.invalidateQueries(['station-staff', stationId]);
                                                    },
                                                    onError: (e: any) => {
                                                        console.log(e);
                                                        toast.error('something happen');
                                                    },
                                                });
                                            }
                                            if (collectId) {
                                                addToCollectMutation.mutateAsync(props.id, {
                                                    onSuccess: () => {
                                                        toast.success('Thêm nhân viên thành công');
                                                        rest.afterClose && rest.afterClose();
                                                        queryClient.invalidateQueries(['staffNotWorking', staffType]);
                                                        collectId && queryClient.invalidateQueries([['collected-hub-staff', collectId]]);
                                                        stationId && queryClient.invalidateQueries(['station-staff', stationId]);
                                                    },
                                                    onError: (e: any) => {
                                                        console.log(e);
                                                        toast.error('something happen');
                                                    },
                                                });
                                            }
                                        }}
                                    >
                                        Thêm
                                    </Button>
                                ),
                            },
                        ]}
                    />
                </div>
            </Descriptions>
        </Modal>
    );
};
export default ListStaffNotWorkingModal;
