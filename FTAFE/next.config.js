/** @type {import('next').NextConfig} */
// const webpack = require('webpack');

// const withBundleAnalyzer = require('@next/bundle-analyzer')({
//     enabled: process.env.ANALYZE === 'true',
// });

// const { parsed: myEnv } = require('dotenv').config({
//     path: './config/.env',
// });

const withAntdLess = require('next-plugin-antd-less');

const nextConfig = {
    productionBrowserSourceMaps: false,
    env: {
        SERVER_URL: process.env.SERVER_URL,
    },
    reactStrictMode: false,
    swcMinify: true,
    experimental: { images: { allowFutureImage: true } },
    // webpack(config) {
    //     // all vars end up in the client -
    //     config.plugins.push(new webpack.EnvironmentPlugin(myEnv));
    //     return config;
    // },
    experimental: {
        externalDir:
            true |
            {
                enabled: true,
                silent: true,
            },
    },

    modifyVars: { '@primary-color': '#006600' },
};

module.exports = withAntdLess(nextConfig);
