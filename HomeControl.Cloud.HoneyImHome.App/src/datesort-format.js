export class DateSortValueConverter {
    toView(array, propertyName, direction) {
        let factor = direction === 'ascending' ? 1 : -1;
        return array.sort((a, b) => {
            return (Date.parse(a[propertyName]) - Date.parse(b[propertyName])) * factor;
    });
}
}