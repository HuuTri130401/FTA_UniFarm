import { PlusOutlined } from '@ant-design/icons';
import type { InputRef } from 'antd';
import { Input, Tag, Tooltip } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

interface TagInputProps extends FieldWrapperProps {
    value?: string[];
    onChange?: (value: string[]) => void;
}
export const TagInput: React.FC<TagInputProps> = ({ name = '', label = '', isHiddenError, required, value = [], onChange = () => {}, ...rest }) => {
    const [tags, setTags] = useState<string[]>([...value]);
    const [inputVisible, setInputVisible] = useState(false);
    const [inputValue, setInputValue] = useState('');
    const [editInputIndex, setEditInputIndex] = useState(-1);
    const [editInputValue, setEditInputValue] = useState('');
    const inputRef = useRef<InputRef>(null);
    const editInputRef = useRef<InputRef>(null);
    const { control } = useFormContext();
    useEffect(() => {
        if (inputVisible) {
            inputRef.current?.focus();
        }
    }, [inputVisible]);

    useEffect(() => {
        editInputRef.current?.focus();
    }, [inputValue]);

    const handleClose = (removedTag: string) => {
        const newTags = tags.filter((tag) => tag !== removedTag);
        setTags(newTags);
    };

    const showInput = () => {
        setInputVisible(true);
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInputValue(e.target.value);
    };

    const handleInputConfirm = () => {
        if (inputValue && tags.indexOf(inputValue) === -1) {
            setTags([...tags, inputValue]);
        }
        setInputVisible(false);
        setInputValue('');
    };

    const handleEditInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEditInputValue(e.target.value);
    };

    const handleEditInputConfirm = () => {
        const newTags = [...tags];
        newTags[editInputIndex] = editInputValue;
        setTags(newTags);
        setEditInputIndex(-1);
        setInputValue('');
    };
    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            <div className="flex flex-wrap gap-4">
                <Controller
                    control={control}
                    name={name}
                    render={({ field }) => (
                        <>
                            {tags.map((tag, index) => {
                                if (editInputIndex === index) {
                                    return (
                                        <Input
                                            {...field}
                                            {...rest}
                                            ref={editInputRef}
                                            key={tag}
                                            size="small"
                                            className="tag-input"
                                            value={editInputValue}
                                            onChange={handleEditInputChange}
                                            onBlur={handleEditInputConfirm}
                                            onPressEnter={handleEditInputConfirm}
                                        />
                                    );
                                }

                                const isLongTag = tag.length > 20;

                                const tagElem = (
                                    <Tag
                                        className="edit-tag"
                                        key={tag}
                                        closable={true}
                                        onClose={() => handleClose(tag)}
                                        style={{
                                            display: 'flex',
                                            justifyContent: 'center',
                                            alignItems: 'center',
                                            minWidth: 100,
                                            cursor: 'pointer',
                                        }}
                                    >
                                        <span
                                            onDoubleClick={(e) => {
                                                setEditInputIndex(index);
                                                setEditInputValue(tag);
                                                e.preventDefault();
                                            }}
                                        >
                                            {isLongTag ? `${tag.slice(0, 20)}...` : tag}
                                        </span>
                                    </Tag>
                                );
                                return isLongTag ? (
                                    <Tooltip title={tag} key={tag}>
                                        {tagElem}
                                    </Tooltip>
                                ) : (
                                    tagElem
                                );
                            })}
                        </>
                    )}
                />
            </div>
            {inputVisible && (
                <Input
                    ref={inputRef}
                    type="text"
                    size="small"
                    className="tag-input"
                    value={inputValue}
                    onChange={handleInputChange}
                    onBlur={handleInputConfirm}
                    onPressEnter={handleInputConfirm}
                />
            )}
            {!inputVisible && (
                <Tag
                    className="site-tag-plus"
                    onClick={showInput}
                    style={{
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        width: 100,
                        cursor: 'pointer',
                    }}
                >
                    <PlusOutlined /> Add Skills
                </Tag>
            )}
        </FieldWrapper>
    );
};
