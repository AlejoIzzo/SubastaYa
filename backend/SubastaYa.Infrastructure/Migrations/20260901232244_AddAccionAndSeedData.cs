using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccionAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accion",
                table: "AuditoriaLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre", "UrlIcono" },
                values: new object[,]
                {
                    { 1, "Tecnología", "https://images.unsplash.com/photo-1519389950473-47ba0277781c" },
                    { 2, "Coleccionables", "https://images.unsplash.com/photo-1563245372-f21724e3856d" },
                    { 3, "Indumentaria", "https://images.unsplash.com/photo-1523381210434-271e8be1f52b" },
                    { 4, "Vehículos", "https://images.unsplash.com/photo-1503376780353-7e6692767b70" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "FechaRegistro", "Nombre" },
                values: new object[,]
                {
                    { 1, "vendedor@test.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vendedor Test" },
                    { 2, "comprador1@test.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comprador 1" },
                    { 3, "comprador2@test.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comprador 2" },
                    { 4, "sinfondos@test.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sin Fondos" }
                });

            migrationBuilder.InsertData(
                table: "Billeteras",
                columns: new[] { "Id", "SaldoDisponible", "SaldoRetenido", "SaldoTotal", "UsuarioId" },
                values: new object[,]
                {
                    { 1, 0.00m, 0.00m, 0.00m, 1 },
                    { 2, 105000.00m, 45000.00m, 150000.00m, 2 },
                    { 3, 200000.00m, 0.00m, 200000.00m, 3 },
                    { 4, 500.00m, 0.00m, 500.00m, 4 }
                });

            migrationBuilder.InsertData(
                table: "Subastas",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "FechaFin", "FechaInicio", "IncrementoMinimo", "PrecioBase", "Titulo", "UrlImagen", "VendedorId" },
                values: new object[,]
                {
                    { 1, 1, "Consola de última generación en perfecto estado con joystick DualSense", "ACTIVA", new DateTime(2026, 9, 1, 23, 45, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 19, 0, 0, 0, DateTimeKind.Utc), 5000.00m, 30000.00m, "PlayStation 5 Digital Edition", "https://images.unsplash.com/photo-1606813907291-d86efa9b94db", 1 },
                    { 2, 2, "Carta coleccionable en estado Near Mint, protegida en acrílico", "ACTIVA", new DateTime(2026, 9, 1, 23, 15, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 19, 0, 0, 0, DateTimeKind.Utc), 1000.00m, 10000.00m, "Carta Pokémon Charizard 1st Edition", "https://images.unsplash.com/photo-1613770920235-94578b87192f", 1 },
                    { 3, 4, "Vehículo sedán con 25.000 km, único dueño, todos los services oficiales", "PROGRAMADA", new DateTime(2026, 9, 3, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 20, 0, 0, 0, DateTimeKind.Utc), 100000.00m, 15000000.00m, "Toyota Corolla 2022 2.0 SEG CVT", "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb", 1 },
                    { 4, 1, "Laptop profesional 16GB RAM, batería con 40 ciclos de carga", "ACTIVA", new DateTime(2026, 9, 1, 18, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), 5000.00m, 80000.00m, "MacBook Pro M2 14 pulgadas 512GB", "https://images.unsplash.com/photo-1517336714731-489689fd1ca8", 1 },
                    { 5, 3, "Campera de cuero genuino talle L, importada", "ACTIVA", new DateTime(2026, 9, 1, 17, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), 2000.00m, 25000.00m, "Campera de Cuero Vintage Hombre", "https://images.unsplash.com/photo-1551028719-00167b16eac5", 1 }
                });

            migrationBuilder.InsertData(
                table: "Pujas",
                columns: new[] { "Id", "CompradorId", "Fecha", "Monto", "SubastaId" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 9, 1, 19, 30, 0, 0, DateTimeKind.Utc), 35000.00m, 1 },
                    { 2, 2, new DateTime(2026, 9, 1, 19, 45, 0, 0, DateTimeKind.Utc), 45000.00m, 1 },
                    { 3, 2, new DateTime(2026, 8, 31, 15, 0, 0, 0, DateTimeKind.Utc), 90000.00m, 4 }
                });

            migrationBuilder.InsertData(
                table: "TransaccionLedger",
                columns: new[] { "Id", "BilleteraId", "Fecha", "Monto", "SubastaId", "Tipo" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), 150000.00m, null, "DEPOSITO" },
                    { 2, 2, new DateTime(2026, 9, 1, 19, 45, 0, 0, DateTimeKind.Utc), 45000.00m, 1, "RETENCION" },
                    { 3, 3, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), 200000.00m, null, "DEPOSITO" },
                    { 4, 3, new DateTime(2026, 9, 1, 19, 30, 0, 0, DateTimeKind.Utc), 35000.00m, 1, "RETENCION" },
                    { 5, 3, new DateTime(2026, 9, 1, 19, 45, 0, 0, DateTimeKind.Utc), 35000.00m, 1, "LIBERACION" },
                    { 6, 4, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), 500.00m, null, "DEPOSITO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pujas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "Accion",
                table: "AuditoriaLogs");
        }
    }
}
