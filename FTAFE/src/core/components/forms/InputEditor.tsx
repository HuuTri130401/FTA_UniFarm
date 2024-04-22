'use client';
import { fileApi } from '@core/api/file.api';
import { Editor, IAllProps } from '@tinymce/tinymce-react';
import * as React from 'react';
import { Controller, useFormContext } from 'react-hook-form';

import { FieldWrapper, FieldWrapperProps } from './FieldWrapper';

type InputEditorType = FieldWrapperProps & IAllProps;
interface InputEditorProps extends InputEditorType {
    defaultValue?: string;
    props?: any;
    height?: number;
}

export const InputEditor: React.FC<InputEditorProps> = ({
    height = 500,
    name = '',
    label = '',
    props,
    defaultValue,
    required,
    isHiddenError,
    ...rest
}) => {
    const { control } = useFormContext();

    return (
        <FieldWrapper name={name} label={label} required={required} isHiddenError={isHiddenError}>
            {/* <div className="cap"></div> */}
            <Controller
                control={control}
                name={name}
                render={({ field: { value, ...field } }) => (
                    <Editor
                        apiKey="jbvyqxeg0vehu56ao5i5sta7jn6hu5wtufubeaitv0movowl"
                        id="editor"
                        value={value}
                        onEditorChange={(content, editor) => field.onChange(editor.getContent().trim())}
                        init={{
                            height,
                            menubar: false,
                            images_upload_handler: (blobInfo, progress) =>
                                new Promise((resolve, reject) => {
                                    fileApi.v1PostUpload(blobInfo.blob()).then((res) => {
                                        resolve(res);
                                    });
                                }),

                            plugins: [
                                'advlist',
                                'autolink',
                                'lists',
                                'link',
                                'image',
                                'charmap',
                                'preview',
                                'anchor',
                                'searchreplace',
                                'visualblocks',
                                'code',
                                'fullscreen',
                                'insertdatetime',
                                'media',
                                'table',
                                'code',
                                'help',
                                'wordcount',
                                'image',
                            ],
                            toolbar:
                                'undo redo | blocks | ' +
                                'bold italic forecolor | alignleft aligncenter ' +
                                'alignright alignjustify | bullist numlist outdent indent | ' +
                                'removeformat | image | help',
                            // content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }',
                            content_css: '/styles/tailwind.css',
                            body_class: '!prose',
                        }}
                    />
                )}
            />
        </FieldWrapper>
    );
};
