import 'antd/dist/antd.css';

import { Table, Tabs } from 'antd';

const { TabPane } = Tabs;

const columns = [
    {
        title: 'Name',
        dataIndex: 'name',
        key: 'name',
    },
    {
        title: 'Age',
        dataIndex: 'age',
        key: 'age',
    },
    {
        title: 'Address',
        dataIndex: 'address',
        key: 'address',
    },
];

const data1 = [
    {
        key: '1',
        name: 'John Brown',
        age: 32,
        address: 'New York No. 1 Lake Park',
    },
    {
        key: '2',
        name: 'Jim Green',
        age: 42,
        address: 'London No. 1 Lake Park',
    },
    {
        key: '3',
        name: 'Joe Black',
        age: 32,
        address: 'Sidney No. 1 Lake Park',
    },
];

const data2 = [
    {
        key: '1',
        name: 'Edward King',
        age: 32,
        address: 'London, Park Lane no. 1',
    },
    {
        key: '2',
        name: 'James Bond',
        age: 42,
        address: 'London, Baker Street no. 2',
    },
    {
        key: '3',
        name: 'Clark Kent',
        age: 29,
        address: 'Metropolis, Daily Planet building',
    },
];

const StaffList = () => {
    return (
        <div style={{ padding: 20 }}>
            <Tabs defaultActiveKey="1">
                <TabPane tab="Tab 1" key="1">
                    <Table columns={columns} dataSource={data1} />
                </TabPane>
                <TabPane tab="Tab 2" key="2">
                    <Table columns={columns} dataSource={data2} />
                </TabPane>
            </Tabs>
        </div>
    );
};

export default StaffList;
