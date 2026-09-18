using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarFechasYUrlsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=200&auto=format&fit=crop&q=80");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1563245372-f21724e3856d?w=200&auto=format&fit=crop&q=80");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1523381210434-271e8be1f52b?w=200&auto=format&fit=crop&q=80");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=200&auto=format&fit=crop&q=80");

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=600&auto=format&fit=crop&q=80" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 10, 31, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1613770920235-94578b87192f?w=600&auto=format&fit=crop&q=80" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 12, 15, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 1, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?w=600&auto=format&fit=crop&q=80" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600&auto=format&fit=crop&q=80" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=600&auto=format&fit=crop&q=80" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1519389950473-47ba0277781c");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1563245372-f21724e3856d");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1523381210434-271e8be1f52b");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "UrlIcono",
                value: "https://images.unsplash.com/photo-1503376780353-7e6692767b70");

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 9, 1, 23, 45, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 19, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1606813907291-d86efa9b94db" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 9, 1, 23, 15, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 19, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1613770920235-94578b87192f" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 9, 3, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 2, 20, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaInicio", "UrlImagen" },
                values: new object[] { new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), "https://images.unsplash.com/photo-1551028719-00167b16eac5" });
        }
    }
}
