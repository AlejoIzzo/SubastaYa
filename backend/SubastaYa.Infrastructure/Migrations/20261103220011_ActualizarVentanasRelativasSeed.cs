using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarVentanasRelativasSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 11, 1, 12, 25, 0, 0, DateTimeKind.Utc), new DateTime(2026, 11, 1, 11, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 11, 1, 12, 1, 30, 0, DateTimeKind.Utc), new DateTime(2026, 11, 1, 11, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 11, 8, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 11, 2, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 11, 1, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 30, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 11, 1, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 30, 12, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 10, 31, 23, 59, 59, 0, DateTimeKind.Utc), new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 12, 15, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 1, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 1, 18, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Subastas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio" },
                values: new object[] { new DateTime(2026, 9, 1, 17, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 10, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
