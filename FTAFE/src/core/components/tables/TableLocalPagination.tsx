import { ChevronLeftIcon, ChevronRightIcon } from '@heroicons/react/24/solid';
import * as React from 'react';

import { useLocalPagination } from '../../contexts';

export interface PaginationProperty {
    pageNumber: number;
    pageSize: number;
}

interface TableLocalPaginationProps {}

export const TableLocalPagination: React.FC<TableLocalPaginationProps> = () => {
    const { pageLength, pageNumber, handleOnChangePage } = useLocalPagination();
    const [renderPage, setRenderPage] = React.useState<Array<number>>([]);

    React.useEffect(() => {
        let pages = Array.apply(null, Array(pageLength)).map((_, i) => i);
        if (pageLength > 10 && pageNumber > 5) {
            pages = [0, ...pages.slice(pageNumber < 5 ? 0 : pageNumber - 4, pageNumber), ...pages.slice(pageNumber, pageNumber + 5), pageLength - 1];
        } else if (pageLength > 10 && pageNumber <= 5) {
            pages = pages.slice(0, 10);
        }

        setRenderPage(pages);
    }, [pageLength, pageNumber]);

    return (
        <div className="flex items-center justify-between px-4 py-3 bg-white border-t border-gray-200 sm:px-6">
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-end">
                {pageLength > 1 && (
                    <div>
                        <nav className="relative z-0 inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                            <button
                                onClick={() => {
                                    if (pageNumber > 0) handleOnChangePage(pageNumber - 1);
                                }}
                                className="relative inline-flex items-center px-2 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-l-md hover:bg-gray-50"
                            >
                                <ChevronLeftIcon className="w-5 h-5" aria-hidden="true" />
                            </button>

                            {renderPage.map((item) => (
                                <button
                                    key={item}
                                    onClick={() => handleOnChangePage(item)}
                                    className={`relative inline-flex items-center px-4 py-2 text-sm font-medium text-gray-500 bg-white border hover:bg-gray-50  
			      ${pageNumber === item ? 'border border-red-500' : ''}
			      ${pageNumber + 1 === item ? ' border-l-yellow-500' : 'border-gray-300 '}`}
                                >
                                    {item + 1}
                                </button>
                            ))}
                            <button
                                onClick={() => {
                                    if (pageNumber < pageLength - 1) handleOnChangePage(pageNumber + 1);
                                }}
                                className="relative inline-flex items-center px-2 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-r-md hover:bg-gray-50"
                            >
                                <ChevronRightIcon className="w-5 h-5" aria-hidden="true" />
                            </button>
                        </nav>
                    </div>
                )}
            </div>
        </div>
    );
};
