import { Menu, Transition } from '@headlessui/react';
import { ChevronDownIcon } from '@heroicons/react/24/outline';
import * as React from 'react';

interface TableActionCellProps extends React.HTMLAttributes<HTMLTableCellElement> {
    label: string;
    actions: { label: React.ReactNode | string; onClick: () => void }[];
}

export const TableActionCell: React.FC<TableActionCellProps> = ({ actions, label, className, ...reset }) => {
    return (
        <div className={`text-sm whitespace-nowrap first:pl-6 first:pr-3 last:pr-6 last:pl-3  ${className}`} {...reset}>
            <Menu as="div" className="relative inline-block text-left">
                <div>
                    <Menu.Button className="inline-flex justify-center w-full px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 shadow-sm hover:bg-gray-200 focus:outline-none ">
                        {label}
                        <ChevronDownIcon className="w-5 h-5 ml-2 -mr-1" aria-hidden="true" />
                    </Menu.Button>
                </div>

                <Transition
                    as={React.Fragment}
                    enter="transition ease-out duration-100"
                    enterFrom="transform opacity-0 scale-95"
                    enterTo="transform opacity-100 scale-100"
                    leave="transition ease-in duration-75"
                    leaveFrom="transform opacity-100 scale-100"
                    leaveTo="transform opacity-0 scale-95"
                >
                    <Menu.Items className="absolute right-0 z-10 w-56 mt-2 origin-top-right bg-white rounded-md shadow-lg bottom-full ring-1 ring-black ring-opacity-5 focus:outline-none">
                        <div className="py-1">
                            {actions.map((item) => (
                                <Menu.Item key={item.label?.toString()}>
                                    {({}) => (
                                        <button
                                            onClick={item.onClick}
                                            className="block w-full px-4 py-2 text-sm text-left text-gray-700 duration-300 hover:bg-gray-200 hover:text-gray-900"
                                        >
                                            {item.label}
                                        </button>
                                    )}
                                </Menu.Item>
                            ))}
                        </div>
                    </Menu.Items>
                </Transition>
            </Menu>
        </div>
    );
};
