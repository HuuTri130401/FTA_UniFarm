import { Progress } from 'antd';
import React from 'react';

interface SkillWeightProgressProps {
    weight: number;
    total: number;
    showInfo?: boolean;
}

const SkillWeightProgress: React.FunctionComponent<SkillWeightProgressProps> = ({ weight, total, showInfo = true }) => {
    const [percent, setPercent] = React.useState(0);
    const [color, setColor] = React.useState('');

    React.useEffect(() => {
        if (percent < 25) {
            return setColor('#64E291');
        }

        if (percent < 50) {
            return setColor('#E6E56C');
        }

        if (percent < 75) {
            return setColor('#FEC771');
        }

        setColor('#EB7070');
    }, [percent]);

    React.useEffect(() => {
        setPercent((weight / total) * 100);
    }, [weight, total]);

    return (
        <div className="flex flex-col items-center justify-center gap-0">
            {showInfo && (
                <p className="m-0 text-xs font-semibold text-gray-900">
                    {weight}/{total}
                </p>
            )}

            <Progress showInfo={false} percent={percent} strokeColor={color} />
        </div>
    );
};

export default SkillWeightProgress;
