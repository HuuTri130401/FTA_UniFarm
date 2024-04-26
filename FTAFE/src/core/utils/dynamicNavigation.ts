/* -------------------------------------------------------------------------- */
/*                      is children page active checking                      */
/* -------------------------------------------------------------------------- */

export function isChildrenPageActive(path: string, match: string) {
    if (path && match) {
        if (path === match) {
            return true;
        }
        return false;
    }
    return false;
}

/* -------------------------------------------------------------------------- */
/*                       is parent page active checking                       */
/* -------------------------------------------------------------------------- */

export function isParentPageActive(pages: Array<{ path: string }>, path: string) {
    if (pages) {
        return pages.some((page) => page.path === path);
    }
    return false;
}
