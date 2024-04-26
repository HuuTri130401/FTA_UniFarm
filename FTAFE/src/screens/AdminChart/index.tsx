import { MonthFilter } from '@core/api/admin.api';
import { useQueryGetDashboard } from '@hooks/api/admin.hook';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import React from 'react';

import OverviewData from './OverviewData';
import Profit from './Profit';
import SkillsCharts from './SkillsCharts';
import TopExpert from './TopExpert';
import WelcomeBanner from './WelcomeBanner';

interface AdminChartProps {
    filter: Partial<MonthFilter>;
}

const AdminChart: React.FunctionComponent<AdminChartProps> = ({ filter }) => {
    console.log('filter: AdminChart', filter);
    const { data, isSuccess } = useQueryGetDashboard();

    const options_totalRevenue = {
        chart: {
            type: 'column',
        },
        title: {
            text: 'Thống kê doanh thu và lợi nhuận theo tháng',
        },
        xAxis: {
            categories: data?.map((item) => 'Th' + item.month) || [],
        },
        yAxis: {
            title: {
                text: 'Doanh thu và lợi nhuận',
            },
            labels: {
                format: '{value} VNĐ',
            },
        },
        series: [
            {
                name: 'Doanh thu',
                data: data?.map((item) => item.totalRevenue) || [],
            },
            {
                name: 'Lợi nhuận',
                data: data?.map((item) => item.totalBenefit) || [],
            },
        ],
    };
    const options_totalOrder = {
        chart: {
            type: 'column',
        },
        title: {
            text: 'Thống kê số lượng đơn hàng theo tháng',
        },
        xAxis: {
            categories: data?.map((item) => 'Th' + item.month) || [],
        },
        yAxis: {
            title: {
                text: 'Số lượng đơn hàng',
            },
            labels: {
                format: '{value} đơn',
            },
        },
        series: [
            {
                name: 'Số lượng đơn hàng',
                data: data?.map((item) => item.totalOrder) || [],
            },
            {
                name: 'Số lượng đơn hàng thành công',
                data: data?.map((item) => item.totalOrderSuccess) || [],
            },
            {
                name: 'Số lượng đơn hàng hủy',
                data: data?.map((item) => item.totalOrderCancel) || [],
            },
            {
                name: 'Số lượng đơn hàng hết hạn',
                data: data?.map((item) => item.totalOrderExpired) || [],
            },
        ],
    };
    return (
        <div className="flex flex-col gap-10 p-10">
            <WelcomeBanner />
            {isSuccess && (
                <>
                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2 md:gap-6 xl:grid-cols-4 2xl:gap-6">
                        <OverviewData
                            totalRevenue={data?.reduce((acc, cur) => acc + cur.totalRevenue, 0)}
                            totalOrder={data?.reduce((acc, cur) => acc + cur.totalOrder, 0)}
                            totalBenefit={data?.reduce((acc, cur) => acc + cur.totalBenefit, 0)}
                            totalUser={data?.reduce((acc, cur) => acc + cur.totalNewCustomer + cur.totalNewFarmHub, 0)}
                        />
                    </div>
                    <div className="flex justify-between gap-6">
                        <div className="w-full md:w-1/2 p-3">
                            <Profit options={options_totalRevenue} />
                        </div>
                        {/* <RealtimeChart /> */}
                        <div className="w-full md:w-1/2 p-3">
                            <HighchartsReact highcharts={Highcharts} options={options_totalOrder} />
                        </div>
                    </div>
                    <SkillsCharts
                        filter={
                            filter ?? {
                                month: new Date().getMonth() + 1,
                            }
                        }
                    />
                    <TopExpert />
                </>
            )}
        </div>
    );
};

export default AdminChart;
