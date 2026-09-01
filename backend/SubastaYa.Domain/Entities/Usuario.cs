using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubastaYa.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public DateTime FechaRegistro { get; set; }

        // navigators

        public Billetera Billetera { get; set; } = null!;
        public ICollection<Subasta> Subastas { get; set; } = new List<Subasta>();
        public ICollection<Puja> Pujas { get; set; } = new List<Puja>();

        public ICollection<AuditoriaLog> AuditoriaLogs { get; set; } = new List<AuditoriaLog>();
    }
}