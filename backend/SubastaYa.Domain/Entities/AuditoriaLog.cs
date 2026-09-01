using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Domain.Entities
{
    public class AuditoriaLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Entidad { get; set; } = string.Empty;

        [Required]
        public int EntidadId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Accion { get; set; } = string.Empty;

        [ForeignKey("Usuario_Id")]
        public int? UsuarioId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
        public string? DetalleJson { get; set; }

        // navigators
        public Usuario? Usuario { get; set; }
    }
}