import { Empty } from 'antd';
import Table, { ColumnsType } from 'antd/lib/table';
import { TableRowSelection } from 'antd/lib/table/interface';

import { useTableUtil } from '../../contexts';
interface TableBuilderProps<T extends object> {
    data: T[];
    columns: ColumnsType<T>;
    isLoading: boolean;
    rowKey: string;
    onClickRow?: (record: T) => void;
    rowSelection?: TableRowSelection<T>;
}

export const TableBuilder = <T extends object>({ data, columns, rowKey, isLoading, onClickRow, rowSelection }: TableBuilderProps<T>) => {
    const { totalItem, page, handleOnChangePage } = useTableUtil();

    return (
        <div>
            <Table
                rowSelection={rowSelection}
                rowKey={rowKey}
                dataSource={data}
                columns={columns}
                onRow={(record) => {
                    return {
                        onClick: () => onClickRow && onClickRow(record),
                    };
                }}
                locale={{
                    emptyText: <Empty />,
                    cancelSort: 'Hủy sắp xếp',
                    collapse: 'Thu gọn',
                    expand: 'Mở rộng',
                    filterCheckall: 'Chọn tất cả',
                    filterConfirm: 'Xác nhận',
                    filterEmptyText: 'Không có lọc',
                    filterReset: 'Bỏ lọc',
                    filterTitle: 'Bộ lọc',
                    filterSearchPlaceholder: 'Tìm kiếm',
                    selectAll: 'Chọn tất cả',
                    selectInvert: 'Chọn ngược lại',
                    selectionAll: 'Chọn tất cả',
                    selectNone: 'Không chọn',
                    sortTitle: 'Sắp xếp',
                    triggerAsc: 'Sắp xếp tăng dần',
                    triggerDesc: 'Sắp xếp giảm dần',
                }}
                loading={isLoading}
                pagination={{
                    pageSize: 10,
                    // showSizeChanger: true,
                    pageSizeOptions: ['10', '20', '50', '100'],
                    showTotal: (total, range) => `${range[0]}-${range[1]} trong ${total} bản ghi`,
                }}
                // onChange={(pagination) => handleOnChangePage((pagination.current || 0) - 1, pagination.pageSize || 10)}
            />
        </div>
    );
};
