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
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Subasta> Subastas { get; set; }
        public DbSet<Billetera> Billeteras { get; set; }
        public DbSet<Puja> Pujas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<AuditoriaLog> AuditoriaLogs { get; set; }
        public DbSet<TransaccionLedger> TransaccionLedger { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Acá luego podemos inyectar el Seed Data (los usuarios y subastas de prueba)
            // que exige la consigna de forma obligatoria.

            // --- on cascade config --- 
            
            // no permitir eliminar un vendedor (usuario) con subastas (evitar eliminar subastas al eliminar vendedor)
            modelBuilder.Entity<Subasta>()
                .HasOne(s => s.Vendedor)
                .WithMany(u => u.Subastas)
                .HasForeignKey(s => s.VendedorId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar categorias con subastas
            modelBuilder.Entity<Subasta>()
                .HasOne(s => s.Categoria)
                .WithMany(c => c.Subastas)
                .HasForeignKey(s => s.CategoriaId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar subastas con pujas
            modelBuilder.Entity<Puja>()
                .HasOne(p => p.Subasta)
                .WithMany(s => s.Pujas)
                .HasForeignKey(p => p.SubastaId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar usuario (comprador) con pujas
            modelBuilder.Entity<Puja>()
                .HasOne(p => p.Comprador)
                .WithMany(u => u.Pujas)
                .HasForeignKey(p => p.CompradorId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar usuarios con billetera
            modelBuilder.Entity<Billetera>()
                .HasOne(b => b.Usuario)
                .WithOne(u => u.Billetera)
                .HasForeignKey<Billetera>(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar billeteras con transacciones hechas 
            modelBuilder.Entity<TransaccionLedger>()
                .HasOne(t => t.Billetera)
                .WithMany(b => b.TransaccionesLedger)
                .HasForeignKey(t => t.BilleteraId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar subastas con transacciones registradas
            modelBuilder.Entity<TransaccionLedger>()
                .HasOne(t => t.Subasta)
                .WithMany(s => s.TransaccionesLedger)
                .HasForeignKey(t => t.SubastaId)
                .OnDelete(DeleteBehavior.NoAction);

            // no permitir eliminar usuarios con logs
            modelBuilder.Entity<AuditoriaLog>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.AuditoriaLogs)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);


            // --- valores por defecto ---
            modelBuilder.Entity<Usuario>()
                .Property(u => u.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<AuditoriaLog>()
                .Property(a => a.Fecha)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<TransaccionLedger>()
                .Property(t => t.Fecha)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Puja>()
                .Property(p => p.Fecha)
                .HasDefaultValueSql("GETUTCDATE()");

            // --- check constraints --- 
            modelBuilder.Entity<TransaccionLedger>()
                .ToTable("TransaccionLedger", table =>
                    table.HasCheckConstraint(
                        "CK_TransaccionLedger_Tipo",
                        "[Tipo] IN ('DEPOSITO', 'RETENCION', 'LIBERACION', 'PAGO', 'COBRO')"
                    ));

            modelBuilder.Entity<Subasta>()
                .ToTable("Subastas", table =>
                    table.HasCheckConstraint(
                        "CK_Subastas_Estado",
                        "[Estado] IN ('PROGRAMADA', 'ACTIVA', 'FINALIZADA', 'DESIERTA')"
                    ));

            // --- Carga de Datos Semilla Obligatorios (Seed Data) ---
            modelBuilder.Seed();
        }
    }
}