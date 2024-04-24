import { Empty, Table } from 'antd';
import { ColumnsType } from 'antd/lib/table';

interface TableBuilderDescriptionProps<T extends object> {
    data: T[];
    columns: ColumnsType<T>;
    isLoading: boolean;
    rowKey: string;
    onClickRow?: (record: T) => void;
}

export const TableBuilderDescription = <T extends object>({ columns, data, isLoading, rowKey, onClickRow }: TableBuilderDescriptionProps<T>) => {
    return (
        <Table
            bordered
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
                showSizeChanger: true,
                pageSizeOptions: ['10', '20', '50', '100'],
                showTotal: (total, range) => `${range[0]}-${range[1]} of ${total} items`,
            }}
        />
    );
};
