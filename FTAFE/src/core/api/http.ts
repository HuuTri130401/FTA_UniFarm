import axios, { AxiosError } from 'axios';
import _get from 'lodash.get';

import { config } from '../config';
import { constant } from '../constant';
import { store } from '../store';
import { apiActions } from '../store/api';
import { lowerCaseField } from '../utils/object.helper';

const http = axios.create({
    baseURL: config.SERVER_URL,
    // withCredentials: true,
});
http.interceptors.request.use(function (req) {
    store.dispatch(apiActions.initReq());
    const token = localStorage.getItem(constant.TOKEN_KEY) || '';

    if (token && req.headers) req.headers[constant.TOKEN_HEADER_KEY] = `Bearer ${token}`;
    // if (token && req.headers) req.headers[constant.TOKEN_HEADER_KEY] = `${token}`;

    return req;
});
http.interceptors.response.use(
    function (response) {
        store.dispatch(apiActions.resetState());
        if (response?.data?.message) store.dispatch(apiActions.updateSuccessMessage(response.data));

        return response;
    },
    function (error: AxiosError) {
        store.dispatch(apiActions.resetState());
        if (error.response?.status) store.dispatch(apiActions.updateErrorDetails(lowerCaseField(_get(error, 'response.data', {}))));

        return Promise.reject(error.response);
    }
);

export { http };
