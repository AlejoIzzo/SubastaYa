export function capitalize(string: string) {
    return string.charAt(0).toUpperCase() + string.slice(1).toLowerCase();
}

export function formatNumber(number: number) {
    return number.toLocaleString('es-ES', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2,
    });
}