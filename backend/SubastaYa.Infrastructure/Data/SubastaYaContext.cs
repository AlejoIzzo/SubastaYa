using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Data
{
    public class SubastaYaContext : DbContext
    {
        public SubastaYaContext(DbContextOptions<SubastaYaContext> options) : base(options)
        {
        }

        // Representación de las tablas en la base de datos
        public DbSet<Subasta> Subastas { get; set; }
        public DbSet<Billetera> Billeteras { get; set; }
        public DbSet<Puja> Pujas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Acá luego podemos inyectar el Seed Data (los usuarios y subastas de prueba)
            // que exige la consigna de forma obligatoria.
        }
    }
}