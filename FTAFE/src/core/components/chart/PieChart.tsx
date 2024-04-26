import { Card, Skeleton } from 'antd';
import Highcharts from 'highcharts';
import HighchartsReact, { HighchartsReactProps, HighchartsReactRefObject } from 'highcharts-react-official';
import React, { useEffect } from 'react';

export interface PieChartData {
    category: string;
    value: number;
}

interface PieChartProps {
    name: string;
    data: PieChartData[];
    title: string;
    suffixValue?: string;
    colors?: string[];
    isLoading?: boolean;
}

const PieChart: React.FunctionComponent<PieChartProps> = ({ data, name, title, suffixValue = '', colors, isLoading }) => {
    const ref = React.useRef<HighchartsReactRefObject>(null);
    const options = React.useMemo<HighchartsReactProps['options']>(() => {
        return {
            chart: {
                type: 'pie',
            },
            title: {
                text: title,
            },
            xAxis: {
                categories: data.map((item) => item.category),
            },
            yAxis: {
                labels: {
                    format: `{value} ${suffixValue}`,
                },
            },
            series: [
                {
                    type: 'pie',
                    name: name,
                    data: data.map((item) => ({ name: item.category, y: item.value })),
                },
            ],
            colors: colors ? colors : Highcharts.getOptions().colors,
        };
    }, [title, data, suffixValue, name, colors]);

    useEffect(() => {
        if (!ref.current) return;
        if (isLoading) {
            ref.current.chart.showLoading();
        } else {
            ref.current.chart.hideLoading();
        }
    }, [isLoading]);

    return <Card>{Boolean(data) ? <HighchartsReact ref={ref} highcharts={Highcharts} options={options} /> : <Skeleton />}</Card>;
};

export default PieChart;
