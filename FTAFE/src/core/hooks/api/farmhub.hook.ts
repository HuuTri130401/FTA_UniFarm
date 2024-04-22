import { useTableUtil } from '@context/tableUtilContext';
import { FarmHubAPI } from '@core/api/farmhub.api';
import { MenuAPI } from '@core/api/menu.api';
import { CreateFarmHubForm, UpdateFarmHubForm } from '@models/farmhub';
import { Menu } from '@models/menu';
import { ProductItemSelling } from '@models/product';
import { ProductItemInMenu } from '@models/product-item';
import { useMutation, useQuery } from '@tanstack/react-query';

export const useQueryFarmHub = () => {
    const { setTotalItem } = useTableUtil();
    return useQuery({
        queryKey: ['farm-hubs'],
        queryFn: async () => {
            const res = await FarmHubAPI.getFarmHubs();
            setTotalItem(res?.payload.length);
            return res;
        },
    });
};
export const useQueryFarmHubById = (id: string) => {
    return useQuery({
        queryKey: ['farm-hub', id],
        queryFn: async () => {
            const res = await FarmHubAPI.getFarmHubById(id);

            return res;
        },
    });
};

export const useUpdateFarmHubMutation = (id: string) => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (data: UpdateFarmHubForm) => {
        const res = FarmHubAPI.updateFarmHub(id, data);
        return res;
    });
    return {
        mutationUpdateFarmHub: mutate,
        mutationUpdateFarmHubAsync: mutateAsync,
        ...rest,
    };
};

export const useDeleteFarmHubMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (id: string) => {
        const res = FarmHubAPI.deleteFarmHub(id);
        return res;
    });
    return {
        mutationDeleteFarmHub: mutate,
        mutationDeleteFarmHubAsync: mutateAsync,
        ...rest,
    };
};

export const useCreateFarmHubMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (data: CreateFarmHubForm) => {
        const res = FarmHubAPI.createFarmHub(data);
        return res;
    });
    return {
        mutationCreateFarmHub: mutate,
        mutationCreateFarmHubAsync: mutateAsync,
        ...rest,
    };
};

export const useQueryGetFarmHubMenu = (id: string) => {
    return useQuery({
        queryKey: ['farm-hub-menu'],
        queryFn: async () => {
            const res = await MenuAPI.getByFarmHubId(id);
            return res;
        },
    });
};

export const useQueryGetAllMenus = () => {
    return useQuery({
        queryKey: ['menus'],
        queryFn: async () => {
            const res = await MenuAPI.getAllMenus();
            return res;
        },
    });
};

export const useQueryGetMenuById = (id: string) => {
    return useQuery({
        queryKey: ['menu', id],
        queryFn: async () => {
            const res = await MenuAPI.getById(id);
            return res.payload as Menu;
        },
    });
};

export const useQueryGetProductItemByMenuId = (menuId: string) => {
    return useQuery({
        queryKey: ['product-items'],
        queryFn: async () => {
            const res = await MenuAPI.getproductItemByMenuId(menuId);
            return res.payload as ProductItemInMenu[];
        },
    });
};

export const useDeleteProductItemInMenuMutation = () => {
    const { mutate, mutateAsync, ...rest } = useMutation(async (productId: string) => {
        const res = MenuAPI.deleteProductItem(productId);
        return res;
    });
    return {
        mutationDeleteProductItemInMenu: mutate,
        mutationDeleteProductItemInMenuAsync: mutateAsync,
        ...rest,
    };
};
export const useQueryGetProductItemSelling = (id: string) => {
    return useQuery({
        queryKey: ['product-item-selling'],
        queryFn: async () => {
            const res = await FarmHubAPI.getProductItemSelling(id);
            return res.payload as ProductItemSelling[];
        },
    });
};
