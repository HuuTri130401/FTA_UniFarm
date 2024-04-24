import { ChevronDownIcon, ChevronUpIcon } from '@heroicons/react/24/solid';
import * as React from 'react';

import { SortOrder, useTableUtil } from '../../contexts';

interface TableHeaderCellProps extends React.DetailedHTMLProps<React.ThHTMLAttributes<any>, any> {
    label: string;
    sortKey?: string;
}

export const TableHeaderCell: React.FC<TableHeaderCellProps> = ({ label, sortKey, className, ...rest }) => {
    const { sortField, sortOrder, handleOnChangeOrderFiled } = useTableUtil();

    return (
        <div
            className={`  ${className}`}
            {...rest}
            onClick={() => {
                if (sortKey) handleOnChangeOrderFiled(sortKey);
            }}
        >
            <div className="relative inline-block">
                {label}
                {Boolean(sortKey) && (
                    <div className="absolute top-0 left-full">
                        {sortField === sortKey &&
                            (sortOrder === SortOrder.ASC ? (
                                <ChevronUpIcon className="inline-block w-4 h-4 ml-1" />
                            ) : (
                                <ChevronDownIcon className="inline-block w-4 h-4 ml-1" />
                            ))}
                    </div>
                )}
            </div>
        </div>
    );
};
