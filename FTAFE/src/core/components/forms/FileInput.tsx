import { FileAddOutlined, LoadingOutlined, PlusOutlined } from '@ant-design/icons';
import { useMutation } from '@tanstack/react-query';
import { Button, Upload, UploadProps } from 'antd';
import { RcFile, UploadChangeParam, UploadFile } from 'antd/lib/upload';
import { clsx } from 'clsx';
import * as React from 'react';
import { toast } from 'react-toastify';

import { fileApi } from '../../api/file.api';
import { constant } from '../../constant';
import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type FileInputType = UploadProps & FieldWrapperProps;

interface ImageInputProps extends FileInputType {
    imageUrl: string;
    handleSetImageUrl: React.Dispatch<React.SetStateAction<string>>;
    previewImageClassName?: string;
    acceptExtension?: string[];
    maxFileSize?: number;
    mode: 'image' | 'file';
}

export const FileInput: React.FC<ImageInputProps> = ({
    name,
    label,
    handleSetImageUrl,
    imageUrl,
    previewImageClassName = '',
    disabled,
    isHiddenError,
    required,
    acceptExtension = ['image/png', 'image/jpeg', 'image/jpg'],
    maxFileSize = 2, // 2MB
    mode = 'image',
    ...rest
}) => {
    const [loading, setLoading] = React.useState(false);

    const uploadImageMutation = useMutation(
        async (file: RcFile) => {
            const res = await toast.promise(fileApi.v1PostUpload(file), {
                pending: 'Đang tải tệp',
            });
            return res;
        },
        {
            onSuccess: (data) => {
                // setValue(name, data);
                handleSetImageUrl(data);
            },
            onError: (error) => {
                toast.error('Tải tệp thất bại');
            },
        }
    );

    const beforeUpload = (file: RcFile) => {
        const isJpgOrPng = acceptExtension.includes(file.type);
        if (!isJpgOrPng) {
            toast.error(`Bạn chỉ có thể tải lên tệp ${acceptExtension.map((item) => item.replace('image/', '').toUpperCase()).join(', ')}!`);
            return false;
        }
        const isLt2M = file.size / 1024 / 1024 < maxFileSize;
        if (!isLt2M) {
            toast.error(`Tệp phải nhỏ hơn ${maxFileSize}MB!`);
            return false;
        }
        return isJpgOrPng && isLt2M; 
    };

    const handleChange: UploadProps['onChange'] = async (info: UploadChangeParam<UploadFile>) => {
        if (info.file.status === 'uploading') {
            setLoading(true);
            return;
        }
        if (info.file.status === 'done') {
            setLoading(false);
            uploadImageMutation.mutate(info.file.originFileObj as RcFile);
        }
    };
    const dummyRequest = async ({ file, onSuccess }: any) => {
        setTimeout(() => {
            onSuccess('ok');
        }, 0);
    };

    return (
        <>
            <FieldWrapper name={name} label={label} isHiddenError={isHiddenError} required={required}>
                <div className={previewImageClassName}>
                    <Upload
                        {...rest}
                        showUploadList={mode !== 'image'}
                        beforeUpload={beforeUpload}
                        onChange={handleChange}
                        customRequest={dummyRequest}
                    >
                        {mode === 'image' ? (
                            <>
                                {imageUrl ? (
                                    <img src={imageUrl} alt={name} className={clsx([previewImageClassName])} />
                                ) : (
                                    <div className="relative w-full h-full">
                                        <img src={constant.DEFAULT_IMAGE_URL} alt="deafult" className={clsx([previewImageClassName])} />
                                        <div className="absolute flex items-center justify-center w-8 h-8 bg-white rounded-md bottom-2 right-2">
                                            {loading ? <LoadingOutlined /> : <PlusOutlined />}
                                        </div>
                                    </div>
                                )}
                            </>
                        ) : (
                            <div>
                                <Button loading={loading} icon={<FileAddOutlined />}>
                                    Chọn tệp
                                </Button>
                            </div>
                        )}
                    </Upload>
                </div>
            </FieldWrapper>
        </>
    );
};
