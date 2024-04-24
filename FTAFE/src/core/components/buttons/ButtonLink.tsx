import Link from 'next/link';
import * as React from 'react';

interface ButtonLinkProps {
    href: string;
    label: string;
    className?: string;
}

export const ButtonLink: React.FC<ButtonLinkProps> = ({
    href,
    label,
    className = 'px-5 py-3 text-white border-transparent rounded-md bg-primary-600 hover:bg-primary-700',
}) => {
    return (
        <Link href={href}>
            <a href={href} className={`inline-flex duration-300 duration items-center justify-center text-base font-medium border ${className} `}>
                {label}
            </a>
        </Link>
    );
};
