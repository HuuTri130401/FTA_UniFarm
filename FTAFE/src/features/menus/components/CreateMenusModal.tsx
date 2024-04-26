import { FormWrapper, TextInput } from '@components/forms';
import { MenusAPI } from '@core/api/menus.api';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button, Modal, ModalProps } from 'antd';
import { useForm } from 'react-hook-form';
import { toast } from 'react-toastify';

interface CreateMenusModalProps extends ModalProps {
    farmHubId: string; // Adding farmHubId to the props for API call
}

const defaultValues = {
    name: '',
    tag: '',
};

/* const CreateMenusModal: React.FC<CreateMenusModalProps> = ({ farmHubId, ...rest }) => {
    const methods = useForm({ defaultValues });
    const queryClient = useQueryClient();

    const createMenuMutation = useMutation(async (data: { name: string; tag: string }) => await MenusAPI.createMenuForFarmHub(farmHubId, data), {
        onSuccess: () => {
            methods.reset();
            toast.success('Menu created successfully!');
            queryClient.invalidateQueries(['menus']); // Assuming you have a query key for menus
            rest.afterClose && rest.afterClose();
        },
        onError: () => {
            toast.error('Failed to create menu');
        },
    });

    const onSubmit = (data: { name: string; tag: string }) => createMenuMutation.mutate(data);

    return (
        <FormWrapper methods={methods}>
            <Modal
                {...rest}
                title="Create Menu"
                width={600}
                footer={[
                    <Button key="close" type="default" onClick={rest.onCancel}>
                        Cancel
                    </Button>,
                    <Button key="submit" type="primary" onClick={methods.handleSubmit(onSubmit)}>
                        Save
                    </Button>,
                ]}
            >
                <form onSubmit={methods.handleSubmit(onSubmit)} className="flex flex-col gap-2">
                    <TextInput name="name" label="Menu Name" required />
                    <TextInput name="tag" label="Tag" required />
                </form>
            </Modal>
        </FormWrapper>
    );
};

export default CreateMenusModal;
 */
