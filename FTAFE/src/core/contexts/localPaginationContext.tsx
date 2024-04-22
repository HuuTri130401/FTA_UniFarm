import * as React from 'react';

export interface LocalPagingProps {
    data: Array<any>;
    originData: Array<any>;
    pageLength: number;
    pageSize: number;
    pageNumber: number;
    setOriginData: React.Dispatch<React.SetStateAction<any[]>>;
    handleOnChangePage: (next: number) => void;
    setPageNumber: React.Dispatch<React.SetStateAction<number>>;
    setPageSize: React.Dispatch<React.SetStateAction<number>>;
}

export const LocalPagingContext = React.createContext<LocalPagingProps>({
    data: [],
    originData: [],
    pageLength: 0,
    pageSize: 0,
    pageNumber: 0,
    handleOnChangePage: () => {},
    setOriginData: () => {},
    setPageNumber: () => {},
    setPageSize: () => {},
});

interface LocalPaginationProviderProps {
    values: Array<any>;
    children: React.ReactNode;
}

export const LocalPagingProvider: React.FC<LocalPaginationProviderProps> = ({ children, values }) => {
    const [pageSize, setPageSize] = React.useState(12);
    const [pageNumber, setPageNumber] = React.useState(2);
    const [originData, setOriginData] = React.useState<Array<any>>([]);
    const [data, setData] = React.useState<Array<any>>([]);
    const [pageLength, setPageLength] = React.useState(0);

    React.useEffect(() => {
        setOriginData(values);
    }, [values]);

    React.useEffect(() => {
        setPageLength(Math.ceil(originData.length / pageSize));
        setData(originData.slice(pageNumber * pageSize, pageNumber * pageSize + pageSize));
    }, [originData, pageSize, pageNumber]);

    const handleOnChangePage = (next: number) => {
        if (next <= pageLength && next >= 0) {
            setPageNumber(next);
        } else {
        }
    };

    return (
        <LocalPagingContext.Provider
            value={{ setPageSize, originData, data, handleOnChangePage, pageLength, pageNumber, pageSize, setOriginData, setPageNumber }}
        >
            {children}
        </LocalPagingContext.Provider>
    );
};

export function useLocalPagination<T>() {
    const methods = React.useContext(LocalPagingContext);

    return {
        ...methods,
        originData: methods.originData as T[],
        data: methods.data as T[],
    };
}
