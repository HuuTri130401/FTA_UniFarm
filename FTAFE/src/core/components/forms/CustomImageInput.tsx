import { LoadingOutlined, PlusOutlined } from '@ant-design/icons';
import { Upload } from 'antd';
import type { UploadChangeParam } from 'antd/es/upload';
import type { RcFile, UploadFile, UploadProps } from 'antd/es/upload/interface';
import React, { useState } from 'react';
import { useFormContext } from 'react-hook-form';
import { toast } from 'react-toastify';

const getBase64 = (img: RcFile, callback: (url: string) => void) => {
    const reader = new FileReader();
    reader.addEventListener('load', () => callback(reader.result as string));
    reader.readAsDataURL(img);
};

interface CustomImageUploadProps extends UploadProps {
    label: string;
    name: string;
    acceptExtension?: string[];
    maxFileSize?: number;
    path?: string;
}

const CustomImageUpload: React.FC<CustomImageUploadProps> = ({
    name,
    label,
    disabled,
    acceptExtension = ['image/png', 'image/jpeg', 'image/jpg'],
    maxFileSize = 2, // 2MB
    path = 'images',
    ...rest
}) => {
    const [loading, setLoading] = useState(false);
    const [imageUrl, setImageUrl] = useState<string>();
    const { setValue, clearErrors } = useFormContext();
    const handleChange: UploadProps['onChange'] = (info: UploadChangeParam<UploadFile>) => {
        clearErrors();
        if (info.file.status === 'uploading') {
            setLoading(true);
            return;
        }
        if (info.file.status === 'done') {
            getBase64(info.file.originFileObj as RcFile, (url) => {
                setLoading(false);
                setImageUrl(url);
            });
            setValue(name, info.file.originFileObj);
        }
    };
    const beforeUpload = (file: RcFile) => {
        const isJpgOrPng = acceptExtension.includes(file.type);
        if (!isJpgOrPng) {
            toast.error(
                `You can only upload with file extension ${acceptExtension.map((item) => item.replace('image/', '').toUpperCase()).join(', ')}!`
            );
            return false;
        }
        const isLt2M = file.size / 1024 / 1024 < maxFileSize;
        if (!isLt2M) {
            toast.error(`The file must smaller than ${maxFileSize}MB!`);
            return false;
        }
        return isJpgOrPng && isLt2M;
    };

    const uploadButton = (
        <div>
            {loading ? <LoadingOutlined /> : <PlusOutlined />}
            <div style={{ marginTop: 8 }}>Upload</div>
        </div>
    );

    return (
        <div className="flex flex-col space-y-2">
            <label htmlFor="cover-photo" className="block text-sm font-medium text-gray-700 sm:mt-px sm:pt-2">
                {label}
            </label>
            <Upload
                maxCount={1}
                name={name}
                listType="picture-card"
                className="avatar-uploader"
                showUploadList={false}
                beforeUpload={beforeUpload}
                onChange={handleChange}
                customRequest={({ onSuccess }) => setTimeout(() => { 
                    if (onSuccess) {
                        onSuccess('ok', undefined);
                    }
                    else {
                        console.error('Error: onSuccess is not defined');
                    }
                }
                , 0)}
            >
                {imageUrl ? <img src={imageUrl} alt="avatar" style={{ width: '100%' }} /> : uploadButton}
            </Upload>
        </div>
    );
};

export default CustomImageUpload;
