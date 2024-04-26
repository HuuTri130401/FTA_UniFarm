import { config } from './config';

export const routes = {
    auth: {
        login: () => '/auth/login',
        register: () => '/auth/register',
        verify: () => '/auth/verify',
        loginSocial: () => `${config.SERVER_URL}/auth/google/login`,
    },

    user: {
        detail: () => '/user',
        profile: {
            detail: () => `/user/profile`,
            update: () => `/user/profile/edit`,
        },
        transaction: {
            list: () => `/user/transaction`,
            deposit: () => `/user/transaction/deposit`,
            success: (amount: number) => `/user/transaction/success?amount=${amount}`,
            fail: () => `/user/transaction/fail`,
        },
        notification: {
            list: () => `/user/notification`,
        },
    },

    admin: {
        admin: () => '#',
        home: () => '/admin',
        profile: () => '/admin/profile',
        businessDay: () => '/admin/business-day',
        apartment: () => '/admin/apartment',
        area: () => '/admin/area',
        user: {
            customer: {
                list: () => `/admin/user/customer`,
                detail: (id: string) => `/admin/user/customer/${id}`,
                create: () => `/admin/user/customer/create`,
                update: (id: string) => `/admin/user/customer/${id}/edit`,
            },
            farm_hub: {
                list: () => `/admin/user/farmhub`,
                detail: (id: string) => `/admin/user/farmhub/${id}`,
                create: () => `/admin/user/farmhub/create`,
                update: (id: string) => `/admin/user/farmhub/${id}/edit`,
            },
            station: {
                list: () => `/admin/user/station`,
                detail: (id: string) => `/admin/user/station/${id}`,
                create: () => `/admin/user/station/create`,
            },
            collected_hub_staff: {
                list: () => `/admin/user/collected-hub-staff`,
                detail: (id: string) => `/admin/user/collected-hub-staff/${id}`,
                create: () => `/admin/user/collected-hub-staff/create`,
                update: (id: string) => `/admin/user/collected-hub-staff/${id}/edit`,
            },
            staff: {
                list: () => `/admin/user/staffs`,
            },
        },
        category: {
            list: () => '/admin/categories',
            create: () => '/admin/categories/create',
            edit: () => '/admin/categories/edit',
        },
        product: {
            list: () => '/admin/product',
            detail: (id: string) => `/admin/product/${id}`,
            listItem: (productId: string) => `/admin/product/${productId}/items`,
        },
        order: {
            list: () => '/admin/order',
            create: () => '/admin/order/create',
            edit: () => '/admin/order/edit',
        },
        menu: {
            list: () => '/admin/menu',
            create: () => '/admin/menu/create',
            edit: () => '/admin/menu/edit',
        },
        payment: {
            home: () => '/admin/payment/history',
            transaction: () => '/admin/transaction',
        },
    },
    farmhub: {
        home1: () => '#',
        home: () => '/farmhub',
        product: {
            list: () => '/farmhub/product',
            detail: (id: string) => `/farmhub/product/${id}`,
        },
        category: {
            list: () => '/farmhub/categories',
            create: () => '/farmhub/categories/create',
            edit: () => '/farmhub/categories/edit',
        },
        businessDay: {
            list: () => '/farmhub/business-day',
            create: () => '/farmhub/business-day/create',
            edit: () => '/farmhub/business-day/edit',
            batchDetail: (id: string) => `/farmhub/business-day/batch/${id}`,
        },
        order: {
            list: () => '/farmhub/order',
            create: () => '/farmhub/order/create',
            edit: () => '/farmhub/order/edit',
        },
        menu: {
            list: () => '/farmhub/menu',
            create: () => '/farmhub/menu/create',
            edit: () => '/farmhub/menu/edit',
            detail: (id: string) => `/farmhub/menu/${id}`,
        },
        batch: {
            list: () => '/farmhub/batch',
            create: () => '/farmhub/batch/create',
            edit: () => '/farmhub/batch/edit',
            detail: (id: string) => `/farmhub/batch/${id}`,
        },
        transaction: {
            list: () => '/farmhub/transaction',
        },
        payment: {
            list: () => '/farmhub/payment',
        },
    },

    staff: {
        home1: () => '#',
        home: () => '/staff',
        profile: () => '/staff/profile',
        businessDay: {
            list: () => '/staff/business-day',
            batchList: (id: string) => `/staff/business-day/${id}/batch`,
        },
        order: {
            list: () => '/staff/order',
            inBatch: (id: string) => `/staff/batch/${id}/order`,
        },
        transfer: {
            list: () => '/staff/transfers',
            orderList: (id: string) => `/staff/transfers/${id}`,
            station: () => '/staff/station',
            create: () => `/staff/transfers/create/`,
        },
    },
    expert: {
        interview: {
            detail: (id: string) => `/expert/interview/${id}`,
            cv: (id: string) => `/expert/interview/${id}/cv`,
        },
    },
    job: {
        detail: (id: string) => `/job/${id}`,
        list: () => '/job',
    },

    home: () => '/',
};
