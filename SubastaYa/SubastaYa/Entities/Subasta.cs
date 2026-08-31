using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Entities
{
    public class Subasta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Vendedor_Id { get; set; }

        [Required]
        public int Categoria_Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Url_Imagen { get; set; } = string.Empty;

        // Se usa decimal(18,2) para evitar errores de redondeo en cálculos financieros
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio_Base { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Incremento_Minimo { get; set; }

        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "PROGRAMADA"; // PROGRAMADA, ACTIVA, FINALIZADA, DESIERTA

        // [Timestamp] mapea el campo Version para el Optimistic Locking automático en EF Core
        [Timestamp]
        public byte[] Version { get; set; }

        public ICollection<Puja> Pujas { get; set; } = new List<Puja>();
    }
}