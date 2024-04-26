import { Card, Skeleton } from 'antd';
import Highcharts from 'highcharts';
import HighchartsExporting from 'highcharts/modules/exporting';
import HighchartsReact, { HighchartsReactProps, HighchartsReactRefObject } from 'highcharts-react-official';
import React, { useEffect } from 'react';

if (typeof Highcharts === 'object') {
    HighchartsExporting(Highcharts);
}

export interface ColumnChartData {
    category: string;
    value: number;
}

interface ColumnChartProps {
    name: string;
    data: ColumnChartData[];
    title: string;
    suffixValue?: string;
    isLoading?: boolean;
}

const ColumnChart: React.FunctionComponent<ColumnChartProps> = ({ data, name, title, suffixValue = '', isLoading }) => {
    const ref = React.useRef<HighchartsReactRefObject>(null);
    const options = React.useMemo<HighchartsReactProps['options']>(() => {
        return {
            chart: {
                type: 'column',
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
                    type: 'column',
                    name,
                    data: data.map((item) => item.value),
                },
            ],
            exporting: {
                enabled: true,
            },
        };
    }, [data, name, title, suffixValue]);

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

export default ColumnChart;
