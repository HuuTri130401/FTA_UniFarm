import {
    AppstoreOutlined,
    BarChartOutlined,
    CloudOutlined,
    ShopOutlined,
    TeamOutlined,
    UploadOutlined,
    UserOutlined,
    VideoCameraOutlined,
} from '@ant-design/icons';
import { Layout, Menu, MenuProps } from 'antd';
import * as React from 'react';

const { Sider } = Layout;

interface MainSideBarProps {}

const items: MenuProps['items'] = [
    UserOutlined,
    VideoCameraOutlined,
    UploadOutlined,
    BarChartOutlined,
    CloudOutlined,
    AppstoreOutlined,
    TeamOutlined,
    ShopOutlined,
].map((icon, index) => ({
    key: String(index + 1),
    icon: React.createElement(icon),
    label: `nav ${index + 1}`,
}));

const MainSideBar: React.FunctionComponent<MainSideBarProps> = () => {
    return (
        <Sider className="fixed top-0 bottom-0 left-0 h-[calc(100vh)] overflow-auto">
            <div className="logo" />
            <Menu theme="dark" mode="inline" defaultSelectedKeys={['4']} items={items} />
        </Sider>
    );
};

export default MainSideBar;
