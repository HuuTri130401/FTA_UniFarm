import { useQueryGetSettlement } from '@hooks/api/businessDay.hook';
import { useQueryGetProductItemSelling } from '@hooks/api/farmhub.hook';
import { useStoreUser } from '@store/index';
import { Spin } from 'antd';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import moment from 'moment';
import React from 'react';

interface ProductItemSellingProps {
    id: string;
}

const ProductItemSelling: React.FC<ProductItemSellingProps> = ({ id }) => {
    const user = useStoreUser();
    const { data: productItemSelling, isLoading } = useQueryGetProductItemSelling(id);
    const { data: settlement, isLoading: isLoadingSettlement } = useQueryGetSettlement(id, user.farmHub?.id as string);

    const options = {
        chart: {
            type: 'column',
        },
        title: {
            text: 'Thống kê các sản phẩm bán được',
        },
        xAxis: {
            categories: productItemSelling?.map((item) => item.title),
        },
        yAxis: {
            title: {
                text: 'Tổng số lượng sản phẩm',
            },
        },
        series: [
            {
                name: 'Tổng số lượng sản phẩm',
                data: productItemSelling?.map((item) => item.quantity),
            },
            {
                name: 'Tổng số lượng bán được',
                data: productItemSelling?.map((item) => item.sold),
            },
        ],
    };

    return (
        <div className="px-10 py-20 min-h-min">
            {isLoading ? (
                <Spin size="large" />
            ) : (
                <div className="flex items-center justify-center gap-20">
                    <div className="w-1/2">
                        {isLoadingSettlement ? (
                            <Spin size="large" />
                        ) : (
                            <div className="w-full shrink-0 grow-0 basis-auto">
                                <h5 className="mb-5 text-xl font-bold text-center dark:text-white">Thống kê doanh thu</h5>
                                <div className="flex flex-wrap">
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex items-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M2.25 18.75a60.07 60.07 0 0115.797 2.101c.727.198 1.453-.342 1.453-1.096V18.75M3.75 4.5v.75A.75.75 0 013 6h-.75m0 0v-.375c0-.621.504-1.125 1.125-1.125H20.25M2.25 6v9m18-10.5v.75c0 .414.336.75.75.75h.75m-1.5-1.5h.375c.621 0 1.125.504 1.125 1.125v9.75c0 .621-.504 1.125-1.125 1.125h-.375m1.5-1.5H21a.75.75 0 00-.75.75v.75m0 0H3.75m0 0h-.375a1.125 1.125 0 01-1.125-1.125V15m1.5 1.5v-.75A.75.75 0 003 15h-.75M15 10.5a3 3 0 11-6 0 3 3 0 016 0zm3 0h.008v.008H18V10.5zm-12 0h.008v.008H6V10.5z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Tổng doanh thu</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.totalSales.toLocaleString('en') + ' VNĐ'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex items-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M2.25 18.75a60.07 60.07 0 0115.797 2.101c.727.198 1.453-.342 1.453-1.096V18.75M3.75 4.5v.75A.75.75 0 013 6h-.75m0 0v-.375c0-.621.504-1.125 1.125-1.125H20.25M2.25 6v9m18-10.5v.75c0 .414.336.75.75.75h.75m-1.5-1.5h.375c.621 0 1.125.504 1.125 1.125v9.75c0 .621-.504 1.125-1.125 1.125h-.375m1.5-1.5H21a.75.75 0 00-.75.75v.75m0 0H3.75m0 0h-.375a1.125 1.125 0 01-1.125-1.125V15m1.5 1.5v-.75A.75.75 0 003 15h-.75M15 10.5a3 3 0 11-6 0 3 3 0 016 0zm3 0h.008v.008H18V10.5zm-12 0h.008v.008H6V10.5z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Tổng lợi nhuận</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.profit.toLocaleString('en') + ' VNĐ'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex align-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M12 7.5h1.5m-1.5 3h1.5m-7.5 3h7.5m-7.5 3h7.5m3-9h3.375c.621 0 1.125.504 1.125 1.125V18a2.25 2.25 0 01-2.25 2.25M16.5 7.5V18a2.25 2.25 0 002.25 2.25M16.5 7.5V4.875c0-.621-.504-1.125-1.125-1.125H4.125C3.504 3.75 3 4.254 3 4.875V18a2.25 2.25 0 002.25 2.25h13.5M6 7.5h3v3H6v-3z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Tổng đơn hàng</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.numOfOrder.toLocaleString('en') + ' đơn hàng'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex align-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M2.25 18.75a60.07 60.07 0 0115.797 2.101c.727.198 1.453-.342 1.453-1.096V18.75M3.75 4.5v.75A.75.75 0 013 6h-.75m0 0v-.375c0-.621.504-1.125 1.125-1.125H20.25M2.25 6v9m18-10.5v.75c0 .414.336.75.75.75h.75m-1.5-1.5h.375c.621 0 1.125.504 1.125 1.125v9.75c0 .621-.504 1.125-1.125 1.125h-.375m1.5-1.5H21a.75.75 0 00-.75.75v.75m0 0H3.75m0 0h-.375a1.125 1.125 0 01-1.125-1.125V15m1.5 1.5v-.75A.75.75 0 003 15h-.75M15 10.5a3 3 0 11-6 0 3 3 0 016 0zm3 0h.008v.008H18V10.5zm-12 0h.008v.008H6V10.5z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Phí giao hàng</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.deliveryFeeOfOrder.toLocaleString('en') + ' VNĐ'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex align-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M2.25 18.75a60.07 60.07 0 0115.797 2.101c.727.198 1.453-.342 1.453-1.096V18.75M3.75 4.5v.75A.75.75 0 013 6h-.75m0 0v-.375c0-.621.504-1.125 1.125-1.125H20.25M2.25 6v9m18-10.5v.75c0 .414.336.75.75.75h.75m-1.5-1.5h.375c.621 0 1.125.504 1.125 1.125v9.75c0 .621-.504 1.125-1.125 1.125h-.375m1.5-1.5H21a.75.75 0 00-.75.75v.75m0 0H3.75m0 0h-.375a1.125 1.125 0 01-1.125-1.125V15m1.5 1.5v-.75A.75.75 0 003 15h-.75M15 10.5a3 3 0 11-6 0 3 3 0 016 0zm3 0h.008v.008H18V10.5zm-12 0h.008v.008H6V10.5z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Phí hoa hồng</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">{settlement?.commissionFee + ' VND'}</p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex align-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M2.25 18.75a60.07 60.07 0 0115.797 2.101c.727.198 1.453-.342 1.453-1.096V18.75M3.75 4.5v.75A.75.75 0 013 6h-.75m0 0v-.375c0-.621.504-1.125 1.125-1.125H20.25M2.25 6v9m18-10.5v.75c0 .414.336.75.75.75h.75m-1.5-1.5h.375c.621 0 1.125.504 1.125 1.125v9.75c0 .621-.504 1.125-1.125 1.125h-.375m1.5-1.5H21a.75.75 0 00-.75.75v.75m0 0H3.75m0 0h-.375a1.125 1.125 0 01-1.125-1.125V15m1.5 1.5v-.75A.75.75 0 003 15h-.75M15 10.5a3 3 0 11-6 0 3 3 0 016 0zm3 0h.008v.008H18V10.5zm-12 0h.008v.008H6V10.5z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Phí hàng ngày</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.dailyFee.toLocaleString('en') + ' VNĐ'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex align-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        fill="none"
                                                        viewBox="0 0 24 24"
                                                        strokeWidth={2}
                                                        stroke="currentColor"
                                                        className="w-6 h-6"
                                                    >
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M15.362 5.214A8.252 8.252 0 0112 21 8.25 8.25 0 016.038 7.048 8.287 8.287 0 009 9.6a8.983 8.983 0 013.361-6.867 8.21 8.21 0 003 2.48z"
                                                        />
                                                        <path
                                                            strokeLinecap="round"
                                                            strokeLinejoin="round"
                                                            d="M12 18a3.75 3.75 0 00.495-7.467 5.99 5.99 0 00-1.925 3.546 5.974 5.974 0 01-2.133-1A3.75 3.75 0 0012 18z"
                                                        />
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Trạng thái thanh toán</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.paymentStatus === 'Pending'
                                                        ? 'Chờ thanh toán'
                                                        : settlement?.paymentStatus === 'Paid'
                                                        ? 'Đã thanh toán'
                                                        : 'Chưa thanh toán'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="w-full mb-12 shrink-0 grow-0 basis-auto md:w-6/12 md:px-3 lg:px-3">
                                        <div className="flex align-start">
                                            <div className="shrink-0">
                                                <div className="inline-block p-4 rounded-md bg-primary-100 text-primary">
                                                    <svg
                                                        fill="#009900"
                                                        version="1.1"
                                                        id="Capa_1"
                                                        xmlns="http://www.w3.org/2000/svg"
                                                        xmlnsXlink="http://www.w3.org/1999/xlink"
                                                        viewBox="0 0 610.398 610.398"
                                                        xmlSpace="preserve"
                                                        className="w-6 h-6"
                                                    >
                                                        <g id="SVGRepo_bgCarrier" strokeWidth={0} />
                                                        <g id="SVGRepo_tracerCarrier" strokeLinecap="round" strokeLinejoin="round" />
                                                        <g id="SVGRepo_iconCarrier">
                                                            <g>
                                                                <g>
                                                                    <path d="M159.567,0h-15.329c-1.956,0-3.811,0.411-5.608,0.995c-8.979,2.912-15.616,12.498-15.616,23.997v10.552v27.009v14.052 c0,2.611,0.435,5.078,1.066,7.44c2.702,10.146,10.653,17.552,20.158,17.552h15.329c11.724,0,21.224-11.188,21.224-24.992V62.553 V35.544V24.992C180.791,11.188,171.291,0,159.567,0z" />
                                                                    <path d="M461.288,0h-15.329c-11.724,0-21.224,11.188-21.224,24.992v10.552v27.009v14.052c0,13.804,9.5,24.992,21.224,24.992 h15.329c11.724,0,21.224-11.188,21.224-24.992V62.553V35.544V24.992C482.507,11.188,473.007,0,461.288,0z" />
                                                                    <path d="M539.586,62.553h-37.954v14.052c0,24.327-18.102,44.117-40.349,44.117h-15.329c-22.247,0-40.349-19.79-40.349-44.117 V62.553H199.916v14.052c0,24.327-18.102,44.117-40.349,44.117h-15.329c-22.248,0-40.349-19.79-40.349-44.117V62.553H70.818 c-21.066,0-38.15,16.017-38.15,35.764v476.318c0,19.784,17.083,35.764,38.15,35.764h468.763c21.085,0,38.149-15.984,38.149-35.764 V98.322C577.735,78.575,560.671,62.553,539.586,62.553z M527.757,557.9l-446.502-0.172V173.717h446.502V557.9z" />
                                                                    <path d="M353.017,266.258h117.428c10.193,0,18.437-10.179,18.437-22.759s-8.248-22.759-18.437-22.759H353.017 c-10.193,0-18.437,10.179-18.437,22.759C334.58,256.074,342.823,266.258,353.017,266.258z" />
                                                                    <path d="M353.017,348.467h117.428c10.193,0,18.437-10.179,18.437-22.759c0-12.579-8.248-22.758-18.437-22.758H353.017 c-10.193,0-18.437,10.179-18.437,22.758C334.58,338.288,342.823,348.467,353.017,348.467z" />
                                                                    <path d="M353.017,430.676h117.428c10.193,0,18.437-10.18,18.437-22.759s-8.248-22.759-18.437-22.759H353.017 c-10.193,0-18.437,10.18-18.437,22.759S342.823,430.676,353.017,430.676z" />
                                                                    <path d="M353.017,512.89h117.428c10.193,0,18.437-10.18,18.437-22.759c0-12.58-8.248-22.759-18.437-22.759H353.017 c-10.193,0-18.437,10.179-18.437,22.759C334.58,502.71,342.823,512.89,353.017,512.89z" />
                                                                    <path d="M145.032,266.258H262.46c10.193,0,18.436-10.179,18.436-22.759s-8.248-22.759-18.436-22.759H145.032 c-10.194,0-18.437,10.179-18.437,22.759C126.596,256.074,134.838,266.258,145.032,266.258z" />
                                                                    <path d="M145.032,348.467H262.46c10.193,0,18.436-10.179,18.436-22.759c0-12.579-8.248-22.758-18.436-22.758H145.032 c-10.194,0-18.437,10.179-18.437,22.758C126.596,338.288,134.838,348.467,145.032,348.467z" />
                                                                    <path d="M145.032,430.676H262.46c10.193,0,18.436-10.18,18.436-22.759s-8.248-22.759-18.436-22.759H145.032 c-10.194,0-18.437,10.18-18.437,22.759S134.838,430.676,145.032,430.676z" />
                                                                    <path d="M145.032,512.89H262.46c10.193,0,18.436-10.18,18.436-22.759c0-12.58-8.248-22.759-18.436-22.759H145.032 c-10.194,0-18.437,10.179-18.437,22.759C126.596,502.71,134.838,512.89,145.032,512.89z" />
                                                                </g>
                                                            </g>
                                                        </g>
                                                    </svg>
                                                </div>
                                            </div>
                                            <div className="ml-3 grow">
                                                <p className="mb-2 font-bold dark:text-white">Ngày thanh toán</p>
                                                <p className="text-base text-neutral-600 dark:text-neutral-200">
                                                    {settlement?.paymentDate
                                                        ? moment(settlement?.paymentDate).format('DD/MM/YYYY')
                                                        : 'Chưa thanh toán'}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                    <div className="w-1/2">
                        <HighchartsReact highcharts={Highcharts} options={options} />
                    </div>
                </div>
            )}
        </div>
    );
};

export default ProductItemSelling;
