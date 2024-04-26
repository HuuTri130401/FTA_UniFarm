import { fileApi } from '@core/api/file.api';
import { useMutation } from '@tanstack/react-query';

const useFileUploadMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (file: File | Blob) => {
        const res = await fileApi.v1PostUpload(file);
        return res;
    });

    return {
        mutateFileUploadMutation: mutate,
        mutateFileUploadMutationAsync: mutateAsync,
        ...rest,
    };
};

export { useFileUploadMutation };
