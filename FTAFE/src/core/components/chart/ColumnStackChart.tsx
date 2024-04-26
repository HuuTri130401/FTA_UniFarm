import { Card, Skeleton } from 'antd';
import Highcharts from 'highcharts';
import HighchartsReact, { HighchartsReactProps } from 'highcharts-react-official';
import React from 'react';

export interface ColumnStackChartData {
    category: string;
    value: number[];
}

interface ColumnStackChartProps {
    name: string;
    stackNames: string[];
    data: ColumnStackChartData[];
    title: string;
    suffixValue?: string;
    colors?: string[];
}

const ColumnStackChart: React.FunctionComponent<ColumnStackChartProps> = ({ stackNames, data, name, title, suffixValue = '', colors }) => {
    const options = React.useMemo<HighchartsReactProps['options']>(() => {
        return {
            chart: {
                type: 'column',
            },
            title: {
                text: title,
            },

            plotOptions: {
                column: {
                    stacking: 'normal',
                    dataLabels: {
                        enabled: true,
                    },
                },
            },

            xAxis: {
                categories: data.map((item) => item.category),
            },

            series: stackNames.map((item, index) => ({
                type: 'column',
                name: item,
                data: data.map((item) => item.value[index]),
            })),
            colors: colors ? colors : Highcharts.getOptions().colors,
        };
    }, [stackNames, data, name, title, suffixValue, colors]);

    return <Card>{Boolean(data) ? <HighchartsReact highcharts={Highcharts} options={options} /> : <Skeleton />}</Card>;
};

export default ColumnStackChart;
