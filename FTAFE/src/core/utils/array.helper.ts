export const addAllOptionSelection = (arr: { label: string; value: any; origin: any }[]) => {
    return [{ label: 'Tất cả', value: '', origin: '' }, ...arr];
};
