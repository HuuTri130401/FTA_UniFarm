import { BusinessDayAPI, BusinessDayStat } from '@core/api/business-day.api';
import { useQuery } from '@tanstack/react-query';
import { Spin } from 'antd';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import React from 'react';

interface ProductItemSellingProps {
    id: string;
}

const BusinessDayStatChart: React.FC<ProductItemSellingProps> = ({ id }) => {
    const { data, isLoading } = useQuery({
        queryKey: ['businessDay', id, 'stat'],
        queryFn: async () => await BusinessDayAPI.getStatistic(id),
    });
    const statData: BusinessDayStat = data?.payload || [];

    const options = {
        chart: {
            type: 'column',
        },
        title: {
            text: 'Thống kê',
        },
        xAxis: {
            categories: [
                'Tổng đơn hàng',
                'Tổng đơn đang vận chuyển',
                'Tổng đơn đã vận chuyển',
                'Tổng đơn hàng bị hủy bởi khách',
                'Tổng đơn hàng bị hủy bởi trang trại',
                'Tổng đơn hàng hủy bởi hệ thống',
                'Tổng đơn hàng hết hạn',
                'Tổng đơn hàng đang chờ được duyệt',
                'Tổng đơn hàng đã xác nhận',
            ],
        },
        yAxis: {
            title: {
                text: 'Số lượng',
            },
        },
        series: [
            {
                name: 'Số lượng',
                data: [
                    statData.totalOrder,
                    statData.totalOrderDelivering,
                    statData.totalOrderSuccess,
                    statData.totalOrderCancelByCustomer,
                    statData.totalOrderCancelByFarm,
                    statData.totalOrderCancelBySystem,
                    statData.totalOrderExpired,
                    statData.totalOrderPending,
                    statData.totalOrderConfirmed,
                ],
            },
        ],
    };

    const pieOptions = {
        isLoading,
        chart: {
            type: 'pie',
        },
        title: {
            text: 'Doanh thu',
        },
        series: [
            {
                name: 'Doanh thu',
                data: [{ name: 'Tổng Doanh thu', y: statData.totalRevenue, color: 'orange' }],
                innerSize: '50%',
                showInLegend: true,
            },
        ],
    };

    return (
        <div className="text-center w-full">
            <div className="text-xl my-3">Tổng doanh thu: {statData.totalRevenue || 0} VND</div>
            <div className="flex flex-row-reverse">
                <div className="py-20 min-h-min w-full">
                    {isLoading ? <Spin size="large" /> : <HighchartsReact highcharts={Highcharts} options={options} />}
                </div>
                {/* <div className="py-20 min-h-min">
                {isLoading ? <Spin size="large" /> : <HighchartsReact highcharts={Highcharts} options={pieOptions} />}
            </div> */}
            </div>
        </div>
    );
};

export default BusinessDayStatChart;
