export function lowercaseFirstLetter(str: string) {
    return str.charAt(0).toLowerCase() + str.slice(1);
}
export function uppercaseFirstLetter(str: string) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

export function capitalizeString(str: string) {
    return str
        .split(' ')
        .map((item) => item.charAt(0).toUpperCase() + item.slice(1).toLowerCase())
        .join(' ');
}

export function splitString(str: string, separator: string) {
    try {
        return str.split(separator);
    } catch (error) {
        return [];
    }
}

export function concatString(value: any, separator: string) {
    try {
        return value.join(separator);
    } catch (error) {
        return '';
    }
}

export const formatMoney = (data: number | string, isRound = false) => {
    if (isRound) {
        data = Math.round(Number(data) / 1) * 1;
    }

    return Number(data).toLocaleString('it-IT', {
        style: 'currency',
        currency: 'USD',
    });
};

export const formatMoneyVND = (data: number | string, isRound = false) => {
    if (isRound) {
        data = Math.round(Number(data) / 1) * 1;
    }

    return Number(data).toLocaleString('it-IT', {
        style: 'currency',
        currency: 'VND',
    });
};

export const formatDistance = (data: number | string) => {
    data = Number(data);
    if (data < 1000) {
        return `${data} m`;
    }
    return `${(data / 1000).toFixed(2)} km`;
};
export const formatTime = (data: number) => {
    if (data < 60) {
        return `${data} giây`;
    }

    if (data < 3600) {
        return `${Math.floor(data / 60)} phút`;
    }

    if (data < 86400) {
        return `${Math.floor(data / 3600)} giờ`;
    }
};

export const formatAddress = (data: string) => {
    data = data
        .replace(/Thành phố/g, 'TP')
        .replace(/,\s*Việt\sNam$|,\s*$/g, '')
        .replace(/\d{5,6}$/g, '')
        .replace(/^\w,\s*/g, '')
        .trim()
        .replace(/,$/, '')
        .trim();

    return data;
};

export const translator = (str: string) => {
    str = str.trim().toLowerCase();
    const dictionary: Record<string, string> = {
        pay_subscription: 'Thanh toán gói cước',
        active: 'Hoạt Động',
        inactive: 'Vô Hiệu Hóa',
        initial: 'Khởi Tạo',
        pending: 'Đang Xử Lý',
        started: 'Bắt Đầu',
        success: 'Thành Công',
        cancel: 'Hủy Bỏ',
        available: 'Sẳn sàng',
        fail: 'Thất Bại',
        admin: 'quản lý',
        user: 'người dùng',
        transfer: 'Chuyển Tiền',
        deposit: 'Nạp Tiền',
        'half paid': 'Đặt Cọc',
        half_paid: 'Đặt Cọc',
        full_paid: 'Hoàn Tất Thanh Toán',
        not_verify: 'Chưa Xác Thật',
        verify: 'Đã Xác Thật',
        refund: 'Hoàn Tiền',
        waiting: 'Chờ Xem Xét',
        rejected: 'Bị Từ Chối',
        percent: 'Giảm theo phần trăm',
        amount: 'Giảm theo Price trị',
        receive: 'Nhận Tiền',
        status_1: 'Hoàn Thành',
        auto: 'Tự Động',
        manual: 'Thủ Công',
        all: 'Tất Cả',
        beginner: 'Mới Bắt Đầu',
        intermediate: 'Trung Bình',
        advanced: 'Nâng Cao',
        enable: 'Bật',
        disable: 'Tắt',
    };
    if (!dictionary[str]) {
        // console.log(`Miss key for ${str}`);
    }

    return dictionary[str.trim().toLowerCase()];
};

export const convertTimeToDate = (time: string) => {
    const date = new Date(time);
    return date.getTime();
};

export const convertStringToDate = (date: string | number) => {
    const time = new Date(Number(date));

    return time;
};

export const splitFirstCharacter = (str: string) => {
    const arr = str.split(' ');
    const result = arr.map((item) => item.charAt(0));
    return result.join('');
};

export const convertTextToAvatar = (text: string) => {
    return `https://ui-avatars.com/api/?name=${text}&background=6366f1&color=fff&size=24`;
};
