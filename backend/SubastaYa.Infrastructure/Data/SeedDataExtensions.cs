using System;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Data
{
    public static class SeedDataExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // 1. Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Tecnología", UrlIcono = "https://images.unsplash.com/photo-1519389950473-47ba0277781c" },
                new Categoria { Id = 2, Nombre = "Coleccionables", UrlIcono = "https://images.unsplash.com/photo-1563245372-f21724e3856d" },
                new Categoria { Id = 3, Nombre = "Indumentaria", UrlIcono = "https://images.unsplash.com/photo-1523381210434-271e8be1f52b" },
                new Categoria { Id = 4, Nombre = "Vehículos", UrlIcono = "https://images.unsplash.com/photo-1503376780353-7e6692767b70" }
            );

            // 2. Usuarios
            var baseDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Nombre = "Vendedor Test", Email = "vendedor@test.com", FechaRegistro = baseDate },
                new Usuario { Id = 2, Nombre = "Comprador 1", Email = "comprador1@test.com", FechaRegistro = baseDate },
                new Usuario { Id = 3, Nombre = "Comprador 2", Email = "comprador2@test.com", FechaRegistro = baseDate },
                new Usuario { Id = 4, Nombre = "Sin Fondos", Email = "sinfondos@test.com", FechaRegistro = baseDate }
            );

            // 3. Billeteras
            modelBuilder.Entity<Billetera>().HasData(
                new Billetera
                {
                    Id = 1,
                    UsuarioId = 1,
                    SaldoTotal = 0.00m,
                    SaldoRetenido = 0.00m,
                    SaldoDisponible = 0.00m
                },
                new Billetera
                {
                    Id = 2,
                    UsuarioId = 2,
                    SaldoTotal = 150000.00m,
                    SaldoRetenido = 45000.00m,
                    SaldoDisponible = 105000.00m
                },
                new Billetera
                {
                    Id = 3,
                    UsuarioId = 3,
                    SaldoTotal = 200000.00m,
                    SaldoRetenido = 0.00m,
                    SaldoDisponible = 200000.00m
                },
                new Billetera
                {
                    Id = 4,
                    UsuarioId = 4,
                    SaldoTotal = 500.00m,
                    SaldoRetenido = 0.00m,
                    SaldoDisponible = 500.00m
                }
            );

            // 4. Subastas (5 Casos de Prueba)
            modelBuilder.Entity<Subasta>().HasData(
                // 1. Activa estándar: Cierra en 20-30 min con líder en $45.000
                new Subasta
                {
                    Id = 1,
                    VendedorId = 1,
                    CategoriaId = 1,
                    Titulo = "PlayStation 5 Digital Edition",
                    Descripcion = "Consola de última generación en perfecto estado con joystick DualSense",
                    UrlImagen = "https://images.unsplash.com/photo-1606813907291-d86efa9b94db",
                    PrecioBase = 30000.00m,
                    IncrementoMinimo = 5000.00m,
                    FechaInicio = new DateTime(2026, 9, 1, 19, 0, 0, DateTimeKind.Utc),
                    FechaFin = new DateTime(2026, 9, 1, 23, 45, 0, DateTimeKind.Utc),
                    Estado = "ACTIVA"
                },
                // 2. Activa crítica: Cierra en menos de 2 min (para probar alerta visual y anti-sniping)
                new Subasta
                {
                    Id = 2,
                    VendedorId = 1,
                    CategoriaId = 2,
                    Titulo = "Carta Pokémon Charizard 1st Edition",
                    Descripcion = "Carta coleccionable en estado Near Mint, protegida en acrílico",
                    UrlImagen = "https://images.unsplash.com/photo-1613770920235-94578b87192f",
                    PrecioBase = 10000.00m,
                    IncrementoMinimo = 1000.00m,
                    FechaInicio = new DateTime(2026, 9, 1, 19, 0, 0, DateTimeKind.Utc),
                    FechaFin = new DateTime(2026, 9, 1, 23, 15, 0, DateTimeKind.Utc),
                    Estado = "ACTIVA"
                },
                // 3. Próxima: Inicio programado a +24 hs (pujas bloqueadas)
                new Subasta
                {
                    Id = 3,
                    VendedorId = 1,
                    CategoriaId = 4,
                    Titulo = "Toyota Corolla 2022 2.0 SEG CVT",
                    Descripcion = "Vehículo sedán con 25.000 km, único dueño, todos los services oficiales",
                    UrlImagen = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb",
                    PrecioBase = 15000000.00m,
                    IncrementoMinimo = 100000.00m,
                    FechaInicio = new DateTime(2026, 9, 2, 20, 0, 0, DateTimeKind.Utc),
                    FechaFin = new DateTime(2026, 9, 3, 20, 0, 0, DateTimeKind.Utc),
                    Estado = "PROGRAMADA"
                },
                // 4. Vencida con ganador: Fecha fin pasada + puja ganadora
                new Subasta
                {
                    Id = 4,
                    VendedorId = 1,
                    CategoriaId = 1,
                    Titulo = "MacBook Pro M2 14 pulgadas 512GB",
                    Descripcion = "Laptop profesional 16GB RAM, batería con 40 ciclos de carga",
                    UrlImagen = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8",
                    PrecioBase = 80000.00m,
                    IncrementoMinimo = 5000.00m,
                    FechaInicio = new DateTime(2026, 8, 30, 10, 0, 0, DateTimeKind.Utc),
                    FechaFin = new DateTime(2026, 9, 1, 18, 0, 0, DateTimeKind.Utc),
                    Estado = "ACTIVA"
                },
                // 5. Vencida desierta: Fecha fin pasada sin pujas
                new Subasta
                {
                    Id = 5,
                    VendedorId = 1,
                    CategoriaId = 3,
                    Titulo = "Campera de Cuero Vintage Hombre",
                    Descripcion = "Campera de cuero genuino talle L, importada",
                    UrlImagen = "https://images.unsplash.com/photo-1551028719-00167b16eac5",
                    PrecioBase = 25000.00m,
                    IncrementoMinimo = 2000.00m,
                    FechaInicio = new DateTime(2026, 8, 30, 10, 0, 0, DateTimeKind.Utc),
                    FechaFin = new DateTime(2026, 9, 1, 17, 0, 0, DateTimeKind.Utc),
                    Estado = "ACTIVA"
                }
            );

            // 5. Pujas (Historial de 2 ofertas en la subasta 1 + 1 oferta en la subasta 4)
            modelBuilder.Entity<Puja>().HasData(
                new Puja
                {
                    Id = 1,
                    SubastaId = 1,
                    CompradorId = 3, // comprador2
                    Monto = 35000.00m,
                    Fecha = new DateTime(2026, 9, 1, 19, 30, 0, DateTimeKind.Utc)
                },
                new Puja
                {
                    Id = 2,
                    SubastaId = 1,
                    CompradorId = 2, // comprador1 (líder)
                    Monto = 45000.00m,
                    Fecha = new DateTime(2026, 9, 1, 19, 45, 0, DateTimeKind.Utc)
                },
                new Puja
                {
                    Id = 3,
                    SubastaId = 4,
                    CompradorId = 2, // comprador1 (ganador en subasta 4)
                    Monto = 90000.00m,
                    Fecha = new DateTime(2026, 8, 31, 15, 0, 0, DateTimeKind.Utc)
                }
            );

            // 6. Transacciones Ledger (Movimientos contables que respaldan los saldos)
            modelBuilder.Entity<TransaccionLedger>().HasData(
                // Depósito inicial comprador 1 ($150.000)
                new TransaccionLedger
                {
                    Id = 1,
                    BilleteraId = 2,
                    SubastaId = null,
                    Tipo = "DEPOSITO",
                    Monto = 150000.00m,
                    Fecha = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                // Retención comprador 1 por puja líder en subasta 1 ($45.000)
                new TransaccionLedger
                {
                    Id = 2,
                    BilleteraId = 2,
                    SubastaId = 1,
                    Tipo = "RETENCION",
                    Monto = 45000.00m,
                    Fecha = new DateTime(2026, 9, 1, 19, 45, 0, DateTimeKind.Utc)
                },
                // Depósito inicial comprador 2 ($200.000)
                new TransaccionLedger
                {
                    Id = 3,
                    BilleteraId = 3,
                    SubastaId = null,
                    Tipo = "DEPOSITO",
                    Monto = 200000.00m,
                    Fecha = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                // Retención temporal comprador 2 en subasta 1 ($35.000)
                new TransaccionLedger
                {
                    Id = 4,
                    BilleteraId = 3,
                    SubastaId = 1,
                    Tipo = "RETENCION",
                    Monto = 35000.00m,
                    Fecha = new DateTime(2026, 9, 1, 19, 30, 0, DateTimeKind.Utc)
                },
                // Liberación de saldo a comprador 2 al ser superado por comprador 1 ($35.000)
                new TransaccionLedger
                {
                    Id = 5,
                    BilleteraId = 3,
                    SubastaId = 1,
                    Tipo = "LIBERACION",
                    Monto = 35000.00m,
                    Fecha = new DateTime(2026, 9, 1, 19, 45, 0, DateTimeKind.Utc)
                },
                // Depósito inicial sinfondos ($500)
                new TransaccionLedger
                {
                    Id = 6,
                    BilleteraId = 4,
                    SubastaId = null,
                    Tipo = "DEPOSITO",
                    Monto = 500.00m,
                    Fecha = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
