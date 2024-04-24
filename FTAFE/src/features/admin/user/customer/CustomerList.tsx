import { TableBodyCell, TableBuilder, TableHeaderCell } from '@components/tables';
import { useTableUtil } from '@context/tableUtilContext';
import { CustomerAPI } from '@core/api/customer.api';
import { useDebounce } from '@hooks/useDebounce';
import { Customer, CustomerFilter } from '@models/customer';
import { useQuery } from '@tanstack/react-query';
import { convertTextToAvatar } from '@utils/string.helper';
import { Descriptions, Image, Input } from 'antd';
// import { expertApi, IV1GetFilterExpert } from '@core/api/expert.api';
// import { ExpertList } from '@models/expert';
import * as React from 'react';

interface CustomerListProps {
    filter: Partial<CustomerFilter>;
}
const { Search } = Input;

const CustomerList: React.FunctionComponent<CustomerListProps> = ({ filter }) => {
    const { setTotalItem } = useTableUtil();
    const { data, isLoading } = useQuery({
        queryKey: ['customers', filter],
        queryFn: async () => {
            const res = await CustomerAPI.getAll({ ...filter, pageSize: 999 });
            return res;
        },
    });
    const listCustomer: Customer[] = data?.payload || [];
    React.useEffect(() => {
        setTotalItem(listCustomer.length);
    }, [listCustomer]);

    const [searchText, setSearchText] = React.useState('');
    const { debouncedValue } = useDebounce({
        delay: 300,
        value: searchText,
    });
    const filterData = listCustomer.filter(
        (i) =>
            i.firstName.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.lastName.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.email.toLowerCase().includes(debouncedValue.toLowerCase()) ||
            i.phoneNumber?.toLowerCase().includes(debouncedValue.toLowerCase())
    );

    return (
        <div className="flex flex-col w-full gap-2">
            <Descriptions
                labelStyle={{
                    fontWeight: 'bold',
                }}
                bordered
                title={`Danh sách Khách hàng`}
                className="p-4 bg-white rounded-lg"
                extra={
                    <div className="flex items-center w-full gap-5">
                        <Search
                            placeholder="Tìm kiếm"
                            allowClear
                            enterButton="Tìm kiếm"
                            size="middle"
                            onChange={(e) => setSearchText(e.target.value)} // Update search text
                            style={{ marginBottom: '1rem', marginTop: '1rem', width: '700px' }}
                        />
                    </div>
                }
            >
                <div className="flex flex-col w-full gap-2">
                    <TableBuilder<Customer>
                        rowKey="id"
                        isLoading={isLoading}
                        data={filterData}
                        columns={[
                            {
                                title: () => <TableHeaderCell key="image" sortKey="image" label="Hình ảnh" />,
                                width: 400,
                                key: 'image',
                                render: ({ ...props }: Customer) => (
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
                                render: ({ ...props }: Customer) => <p className="m-0">{props.firstName}</p>,
                                sorter: (a, b) => a.firstName.localeCompare(b.firstName),
                            },
                            {
                                title: () => <TableHeaderCell key="lastName" label="Họ" />,
                                width: 400,
                                key: 'lastName',
                                render: ({ ...props }: Customer) => <p className="m-0">{props.lastName}</p>,
                                sorter: (a, b) => a.lastName.localeCompare(b.lastName),
                            },
                            {
                                title: () => <TableHeaderCell key="email" label="Email" />,
                                width: 400,
                                key: 'email',
                                render: ({ ...props }: Customer) => <p className="m-0">{props.email}</p>,
                                sorter: (a, b) => a.email.localeCompare(b.email),
                            },
                            {
                                title: () => <TableHeaderCell key="phoneNumber" label="Sđt" />,
                                width: 400,
                                key: 'phoneNumber',
                                render: ({ ...props }: Customer) => <p className="m-0">{props.phoneNumber}</p>,
                            },
                            // {
                            //     title: () => <TableHeaderCell key="status" sortKey="status" label="Status" />,
                            //     width: 400,
                            //     key: 'status',
                            //     render: ({ ...props }: FarmHub) => {
                            //         return (
                            //             <Tag
                            //                 className={clsx(`text-sm whitespace-normal`)}
                            //                 color={typeof props.status === 'string' && props.status === 'Active' ? 'geekblue' : 'volcano'}
                            //             >
                            //                 {props.status}
                            //             </Tag>
                            //         );
                            //     },
                            // },
                            // {
                            //     title: () => <TableHeaderCell key="" sortKey="" label="" />,
                            //     width: 400,
                            //     key: 'action',
                            //     render: ({ ...props }) => {
                            //         return (
                            //             <Dropdown
                            //                 overlay={
                            //                     <Menu>
                            //                         <Menu.Item key="1">
                            //                             <Button
                            //                                 onClick={() => {
                            //                                     toast.warn('chưa có api');
                            //                                 }}
                            //                             >
                            //                                 Điều chỉnh
                            //                             </Button>
                            //                         </Menu.Item>

                            //                         <Menu.Item key="2">
                            //                             <Button
                            //                                 onClick={() => {
                            //                                     toast.warn('chưa có api');
                            //                                 }}
                            //                             >
                            //                                 Xoá
                            //                             </Button>
                            //                         </Menu.Item>
                            //                     </Menu>
                            //                 }
                            //                 trigger={['click']}
                            //             >
                            //                 <DashOutlined />
                            //             </Dropdown>
                            //         );
                            //     },
                            // },
                        ]}
                    />
                </div>
            </Descriptions>
        </div>
    );
};

export default CustomerList;
