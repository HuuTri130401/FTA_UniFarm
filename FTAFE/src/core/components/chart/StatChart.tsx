import { Card, Statistic, StatisticProps } from 'antd';
import * as React from 'react';

interface StatChartProps extends StatisticProps {}

const StatChart: React.FunctionComponent<StatChartProps> = ({ ...rest }) => {
    return (
        <Card>
            <Statistic {...rest} />
        </Card>
    );
};

export default StatChart;
