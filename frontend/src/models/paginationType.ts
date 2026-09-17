export type PaginatedResultDTO<T> = {
    items: T[];
    totalItems: number;
    paginaActual: number;
    tamanioPagina: number;
    totalPaginas: number;
    tienePaginaAnterior: boolean;
    tienePaginaSiguiente: boolean;
}