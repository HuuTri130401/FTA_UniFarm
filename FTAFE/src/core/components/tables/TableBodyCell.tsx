import clsx from 'clsx';
import * as React from 'react';

interface TableBodyCellProps extends React.HTMLAttributes<HTMLTableCellElement> {
    label: string | React.ReactElement;
}

export const TableBodyCell: React.FC<TableBodyCellProps> = ({ label, className, ...reset }) => {
    return (
        <div className={clsx(`text-sm whitespace-normal`, className)} {...reset}>
            {label}
        </div>
    );
};
