import { ADMIN_API, MonthFilter } from '@core/api/admin.api';
import { useQueryGetProductStatistic } from '@hooks/api/admin.hook';
import { useQuery } from '@tanstack/react-query';
import { Tabs } from 'antd';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import React from 'react';

interface SkillsChartsProps {
    filter: Partial<MonthFilter>;
}
interface filterMonth {
    filter: MonthFilter;
}
type ReportChartModel = {
    day: number;
    month: number;
    totalOrder: number;
    totalOrderSuccess: number;
    totalOrderCancel: number;
    totalOrderExpired: number;
    totalRevenue: number;
    totalPayForFarmHub: number;
    totalBenefit: number;
};

const SkillsCharts: React.FunctionComponent<SkillsChartsProps> = ({ filter }) => {
    const { data, isSuccess } = useQueryGetProductStatistic();

    const options = {
        chart: {
            type: 'pie',
        },
        title: {
            text: 'Thống kê sản phẩm bán',
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}%</b>',
        },
        plotOptions: {
            pie: {
                dataLabels: {
                    enabled: true,
                    format: '{point.name}: {point.y}%',
                },
                showInLegend: true,
            },
        },
        series: [
            {
                name: 'Sản phẩm',
                data:
                    data
                        ?.sort((a, b) => b.soldQuantity - a.soldQuantity)
                        .slice(0, 8)
                        ?.filter((item) => item.percent > 0)
                        .map((item) => ({ name: item.productName, y: item.percent })) || [],
            },
        ],
    };

    let month = filter.month || new Date().getMonth() + 1;

    const getReportQuery = useQuery({
        queryKey: ['reports'],
        queryFn: async () => {
            const res = await ADMIN_API.gerReportByMonth(month);
            return res.payload;
        },
    });
    const listData: ReportChartModel[] = getReportQuery.data || [];
    const chartData = listData.map((item) => ({
        x: Date.UTC(2024, item.month - 1, item.day),
        y: item.totalRevenue,
    }));
    const formatDate = (date: Date): string => {
        return date.toLocaleString('vi-VN', { day: '2-digit' });
    };

    const formatYAxisLabel = (value: any) => {
        // Format the number with 1 decimal point
        const formattedNumber = value.toFixed(1);
        // Apply comma separator for thousands
        return formattedNumber.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    };
    let totalInMonth: number = 0;
    let totalPayForFarmHubs: number = 0;
    let totalBenefit: number = 0;
    listData.forEach((e) => {
        totalInMonth += e.totalRevenue;
        totalPayForFarmHubs += e.totalPayForFarmHub;
        totalBenefit += e.totalBenefit;
    });

    const options2: Highcharts.Options = {
        chart: {
            type: 'line',
        },
        title: {
            text: `Lợi nhuận tháng ${filter.month}: ${formatYAxisLabel(totalInMonth)} VND`,
        },
        xAxis: {
            type: 'datetime',
            title: {
                text: 'Ngày',
            },
            labels: {
                formatter: function () {
                    return formatDate(new Date(this.value));
                },
            },
        },
        yAxis: {
            title: {
                text: 'Lợi nhuận (VNĐ)',
            },
            labels: {
                formatter: function (this: Highcharts.AxisLabelsFormatterContextObject): string {
                    return formatYAxisLabel(this.value);
                },
            },
        },
        series: [{ type: 'line', name: 'Lợi nhuận', data: chartData }],
    };

    const totalOrderChartData = listData.map((item) => ({
        x: Date.UTC(2024, item.month - 1, item.day),
        y: item.totalOrder,
    }));
    const totalSuccessOrderChartData = listData.map((item) => ({
        x: Date.UTC(2024, item.month - 1, item.day),
        y: item.totalOrderSuccess,
    }));
    const totalExpiredOrderChartData = listData.map((item) => ({
        x: Date.UTC(2024, item.month - 1, item.day),
        y: item.totalOrderExpired,
    }));
    const totalCanceledOrderChartData = listData.map((item) => ({
        x: Date.UTC(2024, item.month - 1, item.day),
        y: item.totalOrderCancel,
    }));

    const totalOrderOptions: Highcharts.Options = {
        chart: {
            type: 'line',
        },
        title: {
            text: `Thống kê đơn hàng tháng ${filter.month}`,
        },
        xAxis: {
            type: 'datetime',
            title: {
                text: 'Ngày',
            },
            labels: {
                formatter: function () {
                    return formatDate(new Date(this.value));
                },
            },
        },
        yAxis: {
            title: {
                text: 'Tổng đơn',
            },
        },
        series: [
            {
                type: 'column',
                name: 'Tổng đơn hàng',
                data: totalOrderChartData,
            },
            {
                type: 'column',
                name: 'Đơn hàng thành công',
                data: totalSuccessOrderChartData,
            },
            {
                type: 'column',
                name: 'Đơn hàng quá hạn giao',
                data: totalExpiredOrderChartData,
            },
            {
                type: 'column',
                name: 'Đơn hàng bị hủy',
                data: totalCanceledOrderChartData,
            },
        ],
    };

    return (
        <div className="flex justify-between w-full gap-6">
            {isSuccess && (
                <>
                    {' '}
                    <div className="bg-white border rounded-sm shadow-lg  border-slate-200  basis-1/2">
                        <HighchartsReact highcharts={Highcharts} options={options} />
                    </div>
                </>
            )}
            {getReportQuery.isSuccess && (
                <>
                    <div className="bg-white border rounded-sm shadow-lg  border-slate-200  basis-1/2">
                        <div className="bg-white border rounded-sm shadow-lg col-span-full xl:col-span-8 border-slate-200 ">
                            <header className="px-5 py-2 border-b border-slate-100 ">
                                <h2 className="text-lg font-bold text-slate-800 ">Thống kê đơn hàng trong tháng</h2>

                                {/* <FormFilterWrapper<filterMonth> defaultValues={{ month }}>
                                    <MonthInput label="Tháng" name="month" />
                                </FormFilterWrapper> */}
                            </header>
                            <Tabs
                                defaultActiveKey="1"
                                className="bg-white rounded-lg"
                                items={[
                                    {
                                        label: <p className="p-0 m-0 text-black ">Thống kê doanh thu</p>,
                                        key: '1',
                                        children: (
                                            <>
                                                <h2 className="text-center text-lg font-bold text-slate-800 ">
                                                    {`Tiền lãi: ${formatYAxisLabel(totalBenefit)}`} VNĐ
                                                </h2>
                                                <h2 className="text-center text-lg font-bold text-slate-800 ">
                                                    {`Tiền trả cho nông trại: ${formatYAxisLabel(totalPayForFarmHubs)}`} VNĐ
                                                </h2>
                                                <HighchartsReact highcharts={Highcharts} options={options2} />,
                                            </>
                                        ),
                                    },
                                    {
                                        label: <p className="p-0 m-0 text-black ">Thống kê đơn hàng</p>,
                                        key: '2',
                                        children: <HighchartsReact highcharts={Highcharts} options={totalOrderOptions} />,
                                    },
                                ]}
                            />
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};

export default SkillsCharts;
