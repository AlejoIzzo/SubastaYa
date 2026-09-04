using System.Collections.Generic;

namespace SubastaYa.Application.DTOs
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? UrlIcono { get; set; }
    }
}
