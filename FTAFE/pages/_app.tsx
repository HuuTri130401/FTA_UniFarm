import '../styles/globals.css';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
// import { ThemeProvider } from 'next-themes';
import type { AppProps } from 'next/app';
import Script from 'next/script';
import { Provider } from 'react-redux';
import { ToastContainer } from 'react-toastify';

import { AutoLoginWrapper, DynamicLayout, ProgressLoadingBar } from '../src/core/components';
import { store } from '../src/core/store';

const queryClient = new QueryClient({});

import 'antd/dist/antd.css';
import 'react-toastify/dist/ReactToastify.css';
import 'swiper/css';
import 'swiper/css/navigation';
require('antd/dist/antd.less');

import GetCurrentUserWrapper from '@components/wrappers/GetCurrentUserWrapper';
import { ThemeProvider } from 'next-themes';

function MyApp({ Component, pageProps }: AppProps) {
    return (
        <>
            <Script type="text/javascript" src="/static/js/google.script.js" />
            <Provider store={store}>
                <AutoLoginWrapper>
                    <QueryClientProvider client={queryClient}>
                        <GetCurrentUserWrapper>
                            <ThemeProvider enableSystem={true} attribute="class">
                                <ToastContainer autoClose={1500} />
                                <ProgressLoadingBar />
                                <DynamicLayout>
                                    <Component {...pageProps} />
                                </DynamicLayout>
                                <ReactQueryDevtools initialIsOpen={false} position="bottom-right" />
                            </ThemeProvider>
                        </GetCurrentUserWrapper>
                    </QueryClientProvider>
                </AutoLoginWrapper>
            </Provider>
        </>
    );
}

export default MyApp;
