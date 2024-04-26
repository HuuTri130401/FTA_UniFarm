import React from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

interface RealtimeChartProps {}

const RealtimeChart: React.FunctionComponent<RealtimeChartProps> = () => {
    const options = {
        chart: {
            type: 'line',
            animation: Highcharts.SVGRenderer, // enables smooth real-time updates
            marginRight: 10,
            events: {
                load: function () {
                    // set up the updating of the chart every 5 minutes
                    // @ts-ignore
                    const series = this.series[0];
                    setInterval(function () {
                        const x = new Date().getTime(); // current time
                        const y = Math.random(); // random data point
                        series.addPoint([x, y], true, true);
                    }, 5 * 60 * 1000); // 5 minutes in milliseconds
                },
            },
        },
        title: {
            text: 'Biểu đồ đường thời gian thực',
        },
        xAxis: {
            type: 'datetime',
            tickPixelInterval: 150,
        },
        yAxis: {
            title: {
                text: 'Value',
            },
            plotLines: [
                {
                    value: 0,
                    width: 1,
                    color: '#808080',
                },
            ],
        },
        series: [
            {
                name: 'Random Data',
                data: (function () {
                    // generate an array of random data points for initialization
                    const data = [];
                    const time = new Date().getTime();
                    let i;

                    for (i = -19; i <= 0; i++) {
                        data.push({
                            x: time + i * 5 * 60 * 1000, // 5 minutes in milliseconds
                            y: Math.random(),
                        });
                    }
                    return data;
                })(),
            },
        ],
    };
    return (
        <div className="flex flex-col bg-white border rounded-sm shadow-lg dark:bg-slate-800 border-slate-200 dark:border-slate-700">
            <HighchartsReact highcharts={Highcharts} options={options} />
        </div>
    );
};

export default RealtimeChart;
