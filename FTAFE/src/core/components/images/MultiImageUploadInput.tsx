import { InboxOutlined } from '@ant-design/icons';
import { message, Modal } from 'antd';
import type { RcFile, UploadProps } from 'antd/es/upload';
import type { UploadFile } from 'antd/es/upload/interface';
import Dragger from 'antd/lib/upload/Dragger';
import React, { useState } from 'react';
import { useFormContext } from 'react-hook-form';
import { toast } from 'react-toastify';

interface ButtonUploadInputProps extends UploadProps {
    name: string;
    label: string;
    acceptExtension?: string[];
    maxFileSize?: number;
    // path?: string;
}

const getBase64 = (file: RcFile): Promise<string> =>
    new Promise((resolve, reject) => {
        try {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result as string);
            reader.onerror = (error) => reject(error);
        } catch (error) {
            console.error('Error:', error);
            reject(error);
        }
    });

export const MultiImageUploadInput: React.FC<ButtonUploadInputProps> = ({
    name,
    label,
    disabled,
    acceptExtension = ['image/png', 'image/jpeg', 'image/jpg'],
    maxFileSize = 2, // 2MB
    // path = 'images',
    ...rest
}) => {
    const [previewOpen, setPreviewOpen] = useState(false);
    const [previewImage, setPreviewImage] = useState('');
    const [previewTitle, setPreviewTitle] = useState('');

    const handleCancel = () => setPreviewOpen(false);

    const handlePreview = async (file: UploadFile) => {
        try {
            if (!file.url && !file.preview) {
                file.preview = await getBase64(file.originFileObj as RcFile).catch((error) => {
                    console.error('Error:', error);
                    return '';
                });
            }

            setPreviewImage(file.url || (file.preview as string));
            setPreviewOpen(true);
            setPreviewTitle(file.name || '');
        } catch (error) {
            console.error('Error:', error);
        }
    };
    const { setValue, clearErrors } = useFormContext();

    const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
        console.log('Dropped files', e.dataTransfer.files);
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

    return (
        <div className="flex flex-col space-y-2">
            <label htmlFor="cover-photo" className="block text-sm font-medium text-gray-700 sm:mt-px sm:pt-2">
                {label}
            </label>
            <Dragger
                beforeUpload={beforeUpload}
                onPreview={handlePreview}
                name={name}
                multiple
                customRequest={({ onSuccess }) =>
                    setTimeout(() => {
                        if (onSuccess) {
                            onSuccess('ok', undefined);
                        } else {
                            console.error('Error: onSuccess is not defined');
                        }
                    }, 0)
                }
                // log errors

                onChange={(info) => {
                    clearErrors();
                    const { status } = info.file;
                    if (status !== 'uploading') {
                    }
                    if (status === 'done') {
                    } else if (status === 'error') {
                        message.error(`${info.file.name} file upload failed.`);
                    }

                    const doneFiles = info.fileList.filter((file) => file.status === 'done');

                    const fileObjects = doneFiles.map((file) => file.originFileObj);

                    setValue(name, fileObjects);
                }}
                onDrop={handleDrop}
            >
                <p className="ant-upload-drag-icon">
                    <InboxOutlined />
                </p>
                <p className="ant-upload-text">Nhấp hoặc kéo tệp vào khu vực này để tải lên</p>
                <p className="ant-upload-hint">Hỗ trợ tải lên một lần hoặc hàng loạt. Nghiêm cấm tải lên dữ liệu công ty hoặc các tập tin mật khác</p>
            </Dragger>

            <Modal open={previewOpen} title={previewTitle} footer={null} onCancel={handleCancel}>
                <img alt="example" style={{ width: '100%' }} src={previewImage} />
            </Modal>
        </div>
    );
};
