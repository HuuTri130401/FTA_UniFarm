import { useQueryGetTopFarmHub } from '@hooks/api/admin.hook';
import { Loader } from 'lucide-react';
import React from 'react';

interface TopExpertProps {}

const TopExpert: React.FunctionComponent<TopExpertProps> = () => {
    const { isLoading, data } = useQueryGetTopFarmHub();

    const farmHubList: any[] = data || [];

    return (
        <div className="bg-white border rounded-sm shadow-lg  border-slate-200 ">
            <header className="px-5 py-4 border-b border-slate-100 ">
                <h2 className="text-lg font-bold text-slate-800 ">Bảng xếp hạng nông trại</h2>
            </header>
            <div className="p-3">
                <div className="overflow-x-auto">
                    <table className="w-full table-auto">
                        <thead className="text-xs font-semibold uppercase text-slate-400  bg-slate-50  ">
                            <tr>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-left">Mã nông trại</div>
                                </th>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-left">Tên nông trại</div>
                                </th>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-left">Chủ sở hữu</div>
                                </th>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-left">Địa chỉ</div>
                                </th>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-center">Tổng Doanh thu (VND)</div>
                                </th>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-center">Tổng đơn hàng thành công</div>
                                </th>
                                <th className="p-2 whitespace-nowrap">
                                    <div className="font-semibold text-center">Tổng đơn hàng đã hủy</div>
                                </th>
                            </tr>
                        </thead>

                        <tbody className="text-sm divide-y divide-slate-100 ">
                            {isLoading ? (
                                <Loader />
                            ) : (
                                farmHubList.map((i: any) => {
                                    return (
                                        <tr key={i.id}>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="text-left">{i.code}</div>
                                            </td>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="flex items-center">
                                                    <div className="w-10 h-10 mr-2 shrink-0 sm:mr-3">
                                                        <img className="rounded-full" src={i.image} width="40" height="40" alt={i.name} />
                                                    </div>
                                                    <div className="font-medium text-slate-800 ">{i.name}</div>
                                                </div>
                                            </td>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="text-left">{i.ownerName}</div>
                                            </td>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="font-medium text-left text-green">{i.address}</div>
                                            </td>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="text-lg text-center">{i.totalRevenue}</div>
                                            </td>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="text-lg text-center">{i.totalOrderSuccess}</div>
                                            </td>
                                            <td className="p-2 whitespace-nowrap">
                                                <div className="text-lg text-center">{i.totalOrderCancel}</div>
                                            </td>
                                        </tr>
                                    );
                                })
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default TopExpert;
