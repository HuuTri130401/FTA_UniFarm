export interface BaseModel {
    createdAt: string;
    updatedAt: string | null;
    isDeleted: boolean;
}

export interface ResponseList<T> {
    data: T[];
    total: number;
}

export enum SortOrder {
    ASC = 'ASC',
    DESC = 'DESC',
}

export interface PagingProps {
    pageIndex: number;
    pageSize: number;
    orderBy: string;
    order: SortOrder;
    // isShowInactive: boolean;
}

export const defaultPagingProps: PagingProps = {
    pageIndex: 0,
    pageSize: 10,
    orderBy: 'createdAt',
    order: SortOrder.DESC,
};

export interface IOption {
    label: string;
    value: any;
    origin: any;
}

export interface SelectOption {
    label: string;
    value: any;
}

export const enablerOptions: SelectOption[] = [
    {
        label: 'Show',
        value: true,
    },
    {
        label: 'Hide',
        value: false,
    },
];
export const enablerOptions2: SelectOption[] = [
    {
        label: 'Enable',
        value: true,
    },
    {
        label: 'Disable',
        value: false,
    },
];
export const enablerFilterOptions: SelectOption[] = [
    {
        label: 'All',
        value: '',
    },
    ...enablerOptions,
];

export const enablerFilterOptions2: SelectOption[] = [
    {
        label: 'All',
        value: '',
    },
    ...enablerOptions2,
];
