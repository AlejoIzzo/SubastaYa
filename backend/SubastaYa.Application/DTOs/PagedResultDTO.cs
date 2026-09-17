using System;
using System.Collections.Generic;

namespace SubastaYa.Application.DTOs
{
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PaginaActual { get; set; }
        public int TamanioPagina { get; set; }
        public int TotalPaginas => TamanioPagina > 0 ? (int)Math.Ceiling((double)TotalItems / TamanioPagina) : 0;
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    }
}
