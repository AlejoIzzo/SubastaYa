using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaYa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorregirRetencionSeedYConstraintsBilletera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "SaldoRetenido", "SaldoTotal" },
                values: new object[] { 135000.00m, 240000.00m });

            migrationBuilder.UpdateData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Fecha", "Monto" },
                values: new object[] { new DateTime(2026, 8, 30, 10, 0, 0, 0, DateTimeKind.Utc), 240000.00m });

            migrationBuilder.InsertData(
                table: "TransaccionLedger",
                columns: new[] { "Id", "BilleteraId", "Fecha", "Monto", "SubastaId", "Tipo" },
                values: new object[] { 7, 2, new DateTime(2026, 8, 31, 15, 0, 0, 0, DateTimeKind.Utc), 90000.00m, 4, "RETENCION" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddCheckConstraint(
                name: "CK_Billeteras_SaldoDisponible_NoNegativo",
                table: "Billeteras",
                sql: "[SaldoDisponible] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Billeteras_SaldoRetenido_NoNegativo",
                table: "Billeteras",
                sql: "[SaldoRetenido] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Billeteras_SaldoTotal_NoNegativo",
                table: "Billeteras",
                sql: "[SaldoTotal] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Billeteras_SaldoDisponible_NoNegativo",
                table: "Billeteras");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Billeteras_SaldoRetenido_NoNegativo",
                table: "Billeteras");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Billeteras_SaldoTotal_NoNegativo",
                table: "Billeteras");

            migrationBuilder.DeleteData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "Billeteras",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "SaldoRetenido", "SaldoTotal" },
                values: new object[] { 45000.00m, 150000.00m });

            migrationBuilder.UpdateData(
                table: "TransaccionLedger",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Fecha", "Monto" },
                values: new object[] { new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), 150000.00m });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaRegistro",
                value: new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
