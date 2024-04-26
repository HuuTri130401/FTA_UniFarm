import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import React from 'react';
interface ProfitProps {
    options: any;
}

const Profit: React.FunctionComponent<ProfitProps> = ({ options }) => {
    return (
        <div className="flex flex-col bg-white border rounded-sm shadow-lg dark:bg-slate-800 border-slate-200 dark:border-slate-700">
            <HighchartsReact highcharts={Highcharts} options={options} />
        </div>
    );
};

export default Profit;
