using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPermisos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Departamento",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Usuarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfil",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaActualizacion",
                table: "Usuarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "RutaJustificacion",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RutaComprobante",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Departamento", "Direccion", "FechaNacimiento", "FechaRegistro", "FotoPerfil", "Password", "Telefono", "UltimaActualizacion" },
                values: new object[] { null, null, null, new DateTime(2025, 3, 18, 21, 53, 21, 367, DateTimeKind.Local).AddTicks(1136), null, "$2a$11$p.qAmUsgX/bptfWmOLS4zO0N73DYb6zTO5USfE3hoMY16riDLxl/W", null, new DateTime(2025, 3, 18, 21, 53, 21, 367, DateTimeKind.Local).AddTicks(1153) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Departamento", "Direccion", "FechaNacimiento", "FechaRegistro", "FotoPerfil", "Password", "Telefono", "UltimaActualizacion" },
                values: new object[] { null, null, null, new DateTime(2025, 3, 18, 21, 53, 21, 495, DateTimeKind.Local).AddTicks(8378), null, "$2a$11$VufOGrj1aYRUAOjqbYaDpuu2cJI4G8t7Zk/duAbtD.TNejBNZUdgi", null, new DateTime(2025, 3, 18, 21, 53, 21, 495, DateTimeKind.Local).AddTicks(8393) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Departamento",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimaActualizacion",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "RutaJustificacion",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RutaComprobante",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$6ckt0yrRO9rWHIQkTD2rXO19DCSxsKDj9XNA37RtxIkHtnWoL3nY6");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$b5FFOliiRNQHCo/a9fiOjODxY.REGQb8feJhu4QoOUxMN9vCoejCi");
        }
    }
}
