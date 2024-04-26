// module.exports = {
// content: ['./pages/**/*.{js,ts,jsx,tsx}', './src/**/*.{js,ts,jsx,tsx}'],
//     theme: {
//         extend: {
//             colors: {
//                 primary: {
//                     DEFAULT: '#22B75A',
//                     50: '#A3EEBF',
//                     100: '#92EBB3',
//                     200: '#6FE49B',
//                     300: '#4DDE83',
//                     400: '#2AD76B',
//                     500: '#22B75A',
//                     600: '#198843',
//                     700: '#10582B',
//                     800: '#082914',
//                     900: '#000000',
//                 },
//                 tango: {
//                     DEFAULT: '#F37124',
//                     50: '#FFFEFE',
//                     100: '#FEEEE5',
//                     200: '#FBCFB5',
//                     300: '#F8B085',
//                     400: '#F69054',
//                     500: '#F37124',
//                     600: '#D8580C',
//                     700: '#A84409',
//                     800: '#773107',
//                     900: '#471D04',
//                 },
//             },
//         },
//     },
//     corePlugins: {
//         preflight: false,
//     },
//     plugins: [require('@tailwindcss/forms')],
// };

/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['./pages/**/*.{js,ts,jsx,tsx}', './src/**/*.{js,ts,jsx,tsx}'],
    darkMode: 'class',
    theme: {
        extend: {
            backgroundImage: {
                'gradient-to-r': 'linear-gradient(to right, var(--tw-gradient-stops))',
                'green-black-gradient': 'linear-gradient(90deg, rgba(0,255,51,1) 5%, rgba(0,255,176,1) 61%)',
            },
            screens: {
                sm: '575px',
                md: '768px',
                lg: '1025px',
                xl: '1202px',
                pc: '1440px',
            },
            fontFamily: {
                display: ['"CalSans-SemiBold"', 'sans-serif'],
                body: ['"DM Sans"', 'sans-serif'],
            },
            container: {
                center: true,
                width: '1410px',
                padding: '9px 0',
            },
            colors: {
                transparent: 'transparent',
                current: 'currentColor',
                white: '#ffffff',
                accent: '#8358FF',
                'accent-dark': '#7444FF',
                'accent-light': '#9E7CFF',
                'accent-lighter': '#B9A0FF',
                'light-base': '#F5F8FA',
                green: '#10b981',
                orange: '#FEB240',
                red: {
                    DEFAULT: '#EF4444',
                },
                dark_red: '#991b1b',
                blue: '#428AF8',
                jacarta: {
                    base: '#5A5D79',
                    50: '#F4F4F6',
                    100: '#E7E8EC',
                    150: '#EDF2F7',
                    200: '#C4C5CF',
                    300: '#A1A2B3',
                    400: '#7D7F96',
                    500: '#5A5D79',
                    600: '#363A5D',
                    700: '#131740',
                    800: '#101436',
                    900: '#0D102D',
                },

                // Design system
                primary: {
                    // 50: '#e0cdff',
                    100: '#ccffcc',
                    200: '#99ff99',
                    300: '#66ff66',
                    400: '#32ff52',
                    500: '#00ff00',
                    600: '#00cc00',
                    700: '#009900',
                    800: '#006600',
                    900: '#003300',
                    DEFAULT: '#009900',
                },
                accent: {
                    50: '#DBDBE4',
                    100: '#B6B6C8',
                    200: '#9697B0',
                    300: '#7B7C9C',
                    400: '#666788',
                    500: '#575874',
                    600: '#484960',
                    700: '#3C3D50',
                    800: '#323342',
                    900: '#2A2A37',
                },
                info: {
                    50: '#E1F1FD',
                    100: '#A7D5F9',
                    200: '#74BCF6',
                    300: '#48A7F3',
                    400: '#2295F0',
                    500: '#0F83DF',
                    600: '#0D6FBE',
                    700: '#0B5EA2',
                    800: '#09508A',
                    900: '#084475',
                },
                success: {
                    50: '#A9FFC6',
                    100: '#6BFF9D',
                    200: '#36FF7B',
                    300: '#09FF5E',
                    400: '#00E24E',
                    500: '#00C143',
                    600: '#00A038',
                    700: '#00852E',
                    800: '#006E26',
                    900: '#005B20',
                },
                warning: {
                    50: '#FFEFD9',
                    100: '#FFDBA6',
                    200: '#FFC979',
                    300: '#FFB951',
                    400: '#FFAB2D',
                    500: '#FF9E0D',
                    600: '#EC8D00',
                    700: '#D07C00',
                    800: '#B76D00',
                    900: '#A16000',
                },
                danger: {
                    50: '#FFE1E1',
                    100: '#FFAAAA',
                    200: '#FF7979',
                    300: '#FF4E4E',
                    400: '#FF2828',
                    450: '#DE3439',
                    500: '#FF0606',
                    600: '#E30000',
                    700: '#C50000',
                    800: '#AB0000',
                    900: '#950000',
                },
            },
            boxShadow: {
                none: 'none',
                sm: '0px 1px 2px 0px rgba(13, 16, 45, 0.1)',
                base: '0px 1px 2px -1px rgba(13, 16, 45, 0.1), 0px 2px 4px 0px rgba(13, 16, 45, 0.1)',
                md: '0px 2px 4px -2px rgba(13, 16, 45, 0.1), 0px 4px 6px -1px rgba(13, 16, 45, 0.1)',
                lg: '0px 4px 6px -4px rgba(13, 16, 45, 0.1), 0px 10px 15px -3px rgba(13, 16, 45, 0.1)',
                xl: '0px 8px 10px -6px rgba(13, 16, 45, 0.1), 0px 20px 25px -5px rgba(13, 16, 45, 0.1)',
                '2xl': '0px 25px 50px -12px rgba(13, 16, 45, 0.1), 0px 12px 24px 0px rgba(13, 16, 45, 0.1)',
                'accent-volume': '5px 5px 10px rgba(108, 106, 213, 0.25), inset 2px 2px 6px #A78DF0, inset -5px -5px 10px #6336E4',
                'white-volume': '5px 5px 10px rgba(108, 106, 212, 0.25), inset 2px 2px 6px #EEF1F9, inset -5px -5px 10px #DFE3EF',
                cv: '5px 5px 0px 0px rgba(99,102,241,1)',
            },
            fontSize: {
                xs: ['0.75rem', { lineHeight: 'normal' }],
                '2xs': ['0.8125rem', { lineHeight: 'normal' }],
                sm: ['0.875rem', { lineHeight: 'normal' }],
                base: ['1rem', { lineHeight: 'normal' }],
                lg: ['1.25rem', { lineHeight: 'normal' }],
                xl: ['1.5rem', { lineHeight: 'normal' }],
                '2xl': ['1.75rem', { lineHeight: 'normal' }],
                '3xl': ['2rem', { lineHeight: 'normal' }],
                '4xl': ['2.25rem', { lineHeight: 'normal' }],
                '5xl': ['2.5rem', { lineHeight: 'normal' }],
                '6xl': ['3.5rem', { lineHeight: 'normal' }],
                '7xl': ['4.25rem', { lineHeight: 'normal' }],
                // Design system
                'header-1': [
                    '52px',
                    ,
                    {
                        lineHeight: '65px',
                        fontWeight: '500',
                    },
                ],
                'header-2': [
                    '42px',
                    {
                        lineHeight: '53px',
                        fontWeight: '500',
                    },
                ],
                'header-3': [
                    '34px',
                    {
                        lineHeight: '42px',
                        fontWeight: '500',
                    },
                ],
                'header-4': [
                    '27px',
                    {
                        lineHeight: '33px',
                        fontWeight: '500',
                    },
                ],
                'header-5': [
                    '22px',
                    {
                        lineHeight: '28px',
                        fontWeight: '500',
                    },
                ],
                'body-1-bolder': [
                    '18px',
                    {
                        lineHeight: '29px',
                        fontWeight: '700',
                    },
                ],
                'body-1': [
                    '18px',
                    {
                        lineHeight: '29px',
                        fontWeight: '500',
                    },
                ],
                'body-2-bolder': [
                    '16px',
                    {
                        lineHeight: '23px',
                        fontWeight: '700',
                    },
                ],
                'body-2': [
                    '16px',
                    {
                        lineHeight: '23px',
                        fontWeight: '500',
                    },
                ],
                'paragraph-1': [
                    '18px',
                    {
                        lineHeight: '29px',
                        fontWeight: '500',
                    },
                ],
                'paragraph-2': [
                    '16px',
                    {
                        lineHeight: '26px',
                        fontWeight: '500',
                    },
                ],
                'caption-bolder': [
                    '13px',
                    {
                        lineHeight: '21px',
                        fontWeight: '600',
                    },
                ],
                caption: [
                    '13px',
                    {
                        lineHeight: '21px',
                        fontWeight: '500',
                    },
                ],
                'button-large': [
                    '18px',
                    {
                        lineHeight: '18px',
                        fontWeight: '500',
                    },
                ],
                'button-medium': [
                    '16px',
                    {
                        lineHeight: '16px',
                        fontWeight: '500',
                    },
                ],
                'button-small': [
                    '13px',
                    {
                        lineHeight: '13px',
                        fontWeight: '600',
                    },
                ],
                table: [
                    '14px',
                    {
                        lineHeight: '23px',
                        fontWeight: '500',
                    },
                ],
                subtext: [
                    '13px',
                    {
                        lineHeight: '21px',
                        fontWeight: '500',
                    },
                ],
                'supported-text': [
                    '11px',
                    {
                        lineHeight: '18px',
                        fontWeight: '500',
                    },
                ],
            },
            borderRadius: {
                none: '0',
                sm: '0.125rem',
                DEFAULT: '0.25rem',
                DEFAULT: '4px',
                md: '0.375rem',
                lg: '0.5rem',
                full: '9999px',
                large: '12px',
            },

            borderRadius: {
                '2lg': '0.625rem',
            },
            transitionProperty: {
                height: 'height',
                width: 'width',
            },
            animation: {
                fly: 'fly 6s cubic-bezier(0.75, 0.02, 0.31, 0.87) infinite',
                heartBeat: 'heartBeat 1s cubic-bezier(0.75, 0.02, 0.31, 0.87) infinite',
                progress: 'progress 5s linear',
            },
            keyframes: {
                fly: {
                    '0%, 100%': { transform: 'translateY(5%)' },
                    '50%': { transform: 'translateY(0)' },
                },
                heartBeat: {
                    '0%, 40%, 80%, 100%': { transform: 'scale(1.1)' },
                    '20%, 60%': { transform: 'scale(.8)' },
                },
                progress: {
                    '0%': { width: '0%' },
                    '100%': { width: '100%' },
                },
            },
        },
        namedGroups: ['dropdown'],
    },
    // variants: {
    // 	display: ['children', 'children-not'],
    // },

    corePlugins: {
        preflight: false,
    },
    plugins: [require('@tailwindcss/forms'), require('@tailwindcss/typography')],
};
