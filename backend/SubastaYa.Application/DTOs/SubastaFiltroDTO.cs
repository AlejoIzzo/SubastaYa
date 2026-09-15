using System;

namespace SubastaYa.Application.DTOs
{
    public class SubastaFiltroDTO
    {
        public string? Estado { get; set; } // ACTIVA, PROGRAMADA, FINALIZADA
        public int? CategoriaId { get; set; }
        public string? Busqueda { get; set; } // Búsqueda por título o descripción
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public string? Orden { get; set; } // "tiempo_asc", "tiempo_desc", "puja_desc", "puja_asc", "recientes"
        public int? VendedorId { get; set; } // Para filtrar "Mis Publicaciones" del vendedor
        public int? Pagina { get; set; } // Número de página (1 por defecto si se especifica paginación)
        public int? TamanioPagina { get; set; } // Cantidad de elementos por página (ej: 8 o 12)
    }
}
