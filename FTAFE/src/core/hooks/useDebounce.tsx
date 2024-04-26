import * as React from 'react';

interface useDebounceProps<T> {
    delay: number;
    value: T;
}

export function useDebounce<T>({ delay, value }: useDebounceProps<T>) {
    const [debouncedValue, setDebouncedValue] = React.useState<T>(value);

    React.useEffect(() => {
        let timeOutId = setTimeout(() => {
            setDebouncedValue(value);
        }, delay);

        return () => {
            clearTimeout(timeOutId);
        };
    }, [delay, value]);

    return { debouncedValue };
}
