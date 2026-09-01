using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Domain.Entities
{
    public class Subasta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Usuario_Id")]
        public int VendedorId { get; set; }

        [Required]
        [ForeignKey("Categoria_Id")]
        public int CategoriaId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string UrlImagen { get; set; } = string.Empty;

        // Se usa decimal(18,2) para evitar errores de redondeo en cálculos financieros
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioBase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal IncrementoMinimo { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "PROGRAMADA"; // PROGRAMADA, ACTIVA, FINALIZADA, DESIERTA

        // [Timestamp] mapea el campo Version para el Optimistic Locking automático en EF Core
        [Timestamp]
        public byte[] Version { get; set; }

        // navigators
        public ICollection<Puja> Pujas { get; set; } = new List<Puja>();
        public ICollection<TransaccionLedger> TransaccionesLedger { get; set; } = new List<TransaccionLedger>();
        public Usuario Vendedor { get; set; } = null!;
        public Categoria Categoria { get; set; } = null!;
    }
}