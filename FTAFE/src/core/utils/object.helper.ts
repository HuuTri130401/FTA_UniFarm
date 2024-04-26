import color from 'color';
import _get from 'lodash.get';

import { lowercaseFirstLetter, uppercaseFirstLetter } from './string.helper';

export const getObjectWithDefault = <T>(context: any, defaultValues: Record<keyof T, any>) => {
    return Object.keys(defaultValues as any).reduce<Record<keyof T, any>>((pre, cur) => {
        pre[cur as keyof T] = _get(context, cur, defaultValues[cur as keyof T]);

        if (typeof pre[cur as keyof T] === 'string' && Array.isArray(defaultValues[cur as keyof T])) {
            pre[cur as keyof T] = [_get(context, cur, '')];
        }

        return pre;
    }, {} as T);
};

export const jsonToFormData = (json: any) => {
    const formData = new FormData();
    Object.keys(json).forEach((key) => {
        formData.append(key, json[key]);
    });
    return formData;
};

export const lowerCaseField = (json: Record<string, any>) => {
    const result: Record<string, any> = {};
    Object.keys(json).forEach((key) => {
        result[lowercaseFirstLetter(key)] = json[key];
    });
    return result;
};

export const upperCaseField = (json: Record<string, any>) => {
    const formData: Record<string, any> = {};
    Object.keys(json).forEach((key) => {
        formData[uppercaseFirstLetter(key)] = json[key];
    });
    return formData;
};

function hex(str: string) {
    for (var i = 0, hash = 0; i < str.length; hash = str.charCodeAt(i++) + ((hash << 5) - hash));

    var color = Math.floor(Math.abs(((Math.sin(hash) * 10000) % 1) * 16777216)).toString(16);

    return '#' + Array(6 - color.length + 1).join('0') + color;
}

export function colorspace(namespace: string, delimiter?: string) {
    var split = namespace.split(delimiter || ':');
    var base = hex(split[0]);

    if (!split.length) return base;

    for (var i = 0, l = split.length - 1; i < l; i++) {
        base = color(base)
            .mix(color(hex(split[i + 1])))
            .saturate(1)
            .hex();
    }

    return base;
}
