import { useFileUploadMutation } from '@hooks/api/file.hook';
import _clsx from 'clsx';
import * as React from 'react';

import { http } from '../../api';
import { FormErrorMessage } from './FormErrorMessage';

interface ImageInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    name: string;
    label: string;
    imageUrl: string;
    handleSetImageUrl: React.Dispatch<React.SetStateAction<string>>;
    previewImageClassName?: string;
    uploadButtonLabel?: string;
    uploadNoteLabel?: string;
    subLabel?: string;
    isPreview?: boolean;
}

export const callUploadImage = async (input: File) => {
    const formData = new FormData();
    formData.append('file', input);

    const res = await http.post<string>(`/utils/aws/image`, formData);
    return res.data;
};

export const ImageInput: React.FC<ImageInputProps> = ({
    name,
    label,
    handleSetImageUrl,
    imageUrl,
    previewImageClassName = '',
    uploadButtonLabel = 'Upload a file',
    uploadNoteLabel = 'PNG, JPG, GIF up to 10MB',
    subLabel = 'or drag and drop',
    disabled,
    isPreview = true,
    ...rest
}) => {
    const { mutateFileUploadMutationAsync } = useFileUploadMutation();

    const _handleOnChange = async ({ currentTarget }: React.ChangeEvent<HTMLInputElement>) => {
        handleSetImageUrl('');
        if (currentTarget && currentTarget.files) {
            const file = currentTarget.files[0] as File;
            if (file) {
                const url = await mutateFileUploadMutationAsync(file);

                handleSetImageUrl(url);
            }
        }
    };

    return (
        <div className="space-y-2">
            <label htmlFor="cover-photo" className="block text-sm font-medium text-gray-700 sm:mt-px sm:pt-2">
                {label}
            </label>
            <div className="mt-1 sm:mt-0 sm:col-span-2">
                <div
                    className={_clsx('flex justify-center w-full px-6 pt-5 pb-6  border-2 border-gray-300 border-dashed rounded-md', {
                        'bg-gray-200': disabled,
                        'bg-white ': !disabled,
                    })}
                >
                    <div className="space-y-2 text-center">
                        <div className={`${previewImageClassName} mx-auto `}>
                            {isPreview && imageUrl ? (
                                <img className="object-cover w-full h-full" src={imageUrl} alt={name} />
                            ) : (
                                <label htmlFor={name}>
                                    <svg
                                        className="w-12 h-12 mx-auto text-gray-400"
                                        stroke="currentColor"
                                        fill="none"
                                        viewBox="0 0 48 48"
                                        aria-hidden="true"
                                    >
                                        <path
                                            d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                                            strokeWidth={2}
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                        />
                                    </svg>
                                </label>
                            )}
                        </div>
                        {!disabled && (
                            <>
                                <div className="flex text-sm text-gray-600">
                                    <label
                                        htmlFor={name}
                                        className="relative font-medium bg-white rounded-md cursor-pointer text-primary-600 hover:text-primary-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-primary-500"
                                    >
                                        <span>{uploadButtonLabel}</span>
                                        <input id={name} {...rest} onChange={_handleOnChange} name={name} type="file" className="sr-only" />
                                    </label>
                                    <p className="pl-1">{subLabel}</p>
                                </div>
                                <p className="text-xs text-gray-500">{uploadNoteLabel}</p>
                            </>
                        )}
                    </div>
                </div>
            </div>

            <FormErrorMessage className="text-sm text-red" name={name} label={label} />
        </div>
    );
};
