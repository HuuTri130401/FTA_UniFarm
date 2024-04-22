import { FarmHubAPI } from '@core/api/farmhub.api';
import { UpdateFarmHubForm } from '@models/farmhub';
import { FarmHub } from '@models/user';
import { useMutation } from '@tanstack/react-query';
import { Button, Form, Input, Select } from 'antd';
import React from 'react';
import { toast } from 'react-toastify';

interface FarmHubEditProps {
    farmHub: FarmHub;
}

const FarmHubEdit: React.FC<FarmHubEditProps> = ({ farmHub }) => {
    const updateFarmHubMutation = useMutation({
        mutationKey: ['farm-hub', farmHub?.id],
        mutationFn: async (data: UpdateFarmHubForm) => {
            const res = await FarmHubAPI.updateFarmHub(farmHub.id, data);
            return res;
        },
    });

    const onFinish = (data: FarmHub) => {
        const updatedData = { ...data, updatedAt: new Date().toISOString() };

        updateFarmHubMutation.mutate(updatedData, {
            onSuccess: () => {
                toast.success('Update FarmHub Success');
            },
        });

        console.log('Updated FarmHub:', updatedData);
    };

    return (
        <div className="w-full">
            <Form labelCol={{ span: 6 }} wrapperCol={{ span: 12 }} onFinish={onFinish} initialValues={farmHub}>
                <Form.Item label="Name" name="name" rules={[{ required: true, message: 'Please enter the name' }]}>
                    <Input />
                </Form.Item>

                <Form.Item label="Mã" name="code" rules={[{ required: true, message: 'Please enter the code' }]}>
                    <Input />
                </Form.Item>

                <Form.Item label="Description" name="description">
                    <Input.TextArea />
                </Form.Item>

                <Form.Item label="Address" name="address">
                    <Input />
                </Form.Item>

                <Form.Item label="Status" name="status" rules={[{ required: true, message: 'Please select the status' }]}>
                    <Select>
                        <Select.Option value="Active">Active</Select.Option>
                        <Select.Option value="Inactive">Inactive</Select.Option>
                    </Select>
                </Form.Item>

                <Form.Item wrapperCol={{ offset: 6, span: 12 }}>
                    <Button type="primary" htmlType="submit">
                        Save
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};

export default FarmHubEdit;
