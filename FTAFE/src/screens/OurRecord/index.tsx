import React from 'react';
import { Slide } from 'react-awesome-reveal';

interface OurRecordProps {}
const statistics = [
    {
        key: 'Inexperienced Interview',
        value: '50%',
    },
    {
        key: 'CV Unsuitable',
        value: '60%',
    },
    {
        key: 'Unconfident',
        value: '70%',
    },
];
const OurRecord: React.FunctionComponent<OurRecordProps> = () => {
    const [imageInsideDiv, setImageInsideDiv] = React.useState(true);

    return (
        <Slide direction="right" triggerOnce>
            <div className="relative">
                <div
                    className={`flex flex-col md:flex-row justify-between max-w-screen-xl mx-auto py-20 md:py-24 ${
                        !imageInsideDiv && `md:items-center`
                    }`}
                >
                    <div className="w-full max-w-md mx-auto md:max-w-none md:mx-0 md:w-5/12 flex-shrink-0 h-80 md:h-auto relative">
                        <img
                            src="https://img.freepik.com/free-vector/people-analyzing-growth-charts-illustrated_23-2148865274.jpg?w=2000"
                            alt="outrecord"
                            className="rounded bg-contain bg-no-repeat bg-center h-full"
                        />
                    </div>
                    <div className="w-full max-w-md mx-auto md:max-w-none md:mx-0 md:w-7/12 mt-16 md:mt-0 md:ml-12 lg:ml-16 md:order-last">
                        <div className="lg:py-8 text-center md:text-left">
                            <h4 className="font-bold text-primary-900 text-center md:text-left text-lg">Theo thống kê</h4>
                            <h2 className="mt-4 font-black text-3xl sm:text-4xl lg:text-5xl text-center md:text-left leading-tight">
                                Why graduates are unemployed?
                            </h2>
                            <p className="mt-4 text-center md:text-left text-sm md:text-base lg:text-lg font-medium leading-relaxed text-secondary-100">
                                Students and recent graduates, often lacking work experience, face challenges in crafting compelling resumes and
                                feeling unprepared for interviews, which can make job hunting difficult.
                            </p>
                            <div className="flex flex-col items-center sm:block text-center md:text-left mt-4">
                                {statistics.map((statistic, index) => (
                                    <div className="text-left sm:inline-block sm:mr-12 last:mr-0 mt-4" key={index}>
                                        <div className="font-bold text-lg sm:text-xl lg:text-2xl text-secondary-500 tracking-wide">
                                            {statistic.value}
                                        </div>
                                        <div className="font-medium text-primary-800 text-caption">{statistic.key}</div>
                                    </div>
                                ))}
                            </div>
                            {/* <PrimaryButton as="a" href={primaryButtonUrl}>
                            {primaryButtonText}
                        </PrimaryButton> */}
                        </div>
                    </div>
                </div>
            </div>
        </Slide>
    );
};

export default OurRecord;
