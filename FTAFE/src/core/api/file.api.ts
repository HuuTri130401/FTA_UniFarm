import _get from 'lodash.get';

import { http } from './http';

export interface IV1ReplaceFile {
    currentUrl: string;
    newUrl: string;
}

export const fileApi = {
    v1PostUpload: async (file: File | Blob) => {
        const formData = new FormData();
        formData.append('image', file);
        const res = await http.post(`/s3/image`, formData);
        return _get(res, 'data') as string;
    },
};
