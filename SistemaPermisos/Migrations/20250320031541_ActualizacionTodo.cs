using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPermisos.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegreso",
                table: "Permisos");

            migrationBuilder.RenameColumn(
                name: "FechaSalida",
                table: "Permisos",
                newName: "Fecha");

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Puesto",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadLecciones",
                table: "Permisos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Condicion",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraDesde",
                table: "Permisos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraHasta",
                table: "Permisos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraSalida",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JornadaCompleta",
                table: "Permisos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MediaJornada",
                table: "Permisos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesResolucion",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Puesto",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolucion",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoConvocatoria",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMotivo",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoRebajo",
                table: "Permisos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoriaPersonal",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instancia",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesResolucion",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Puesto",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolucion",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "OmisionesMarca",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Cedula", "FechaRegistro", "Password", "Puesto", "UltimaActualizacion" },
                values: new object[] { null, new DateTime(2025, 3, 19, 21, 15, 40, 672, DateTimeKind.Local).AddTicks(9157), "$2a$11$DF9hLtS3s5ECR8y.7NUz1udVWKVSQRaY/hr1SE8uiGl4GR4X8Ajx6", null, new DateTime(2025, 3, 19, 21, 15, 40, 672, DateTimeKind.Local).AddTicks(9171) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Cedula", "FechaRegistro", "Password", "Puesto", "UltimaActualizacion" },
                values: new object[] { null, new DateTime(2025, 3, 19, 21, 15, 40, 803, DateTimeKind.Local).AddTicks(6072), "$2a$11$QmBv.613fktvR/r0.Fp4MeTpj7iE52UONo41KP63ODv/a6NOkPeoq", null, new DateTime(2025, 3, 19, 21, 15, 40, 803, DateTimeKind.Local).AddTicks(6088) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Puesto",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CantidadLecciones",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Condicion",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "HoraDesde",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "HoraHasta",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "HoraSalida",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "JornadaCompleta",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "MediaJornada",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "ObservacionesResolucion",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Puesto",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Resolucion",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "TipoConvocatoria",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "TipoMotivo",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "TipoRebajo",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "CategoriaPersonal",
                table: "OmisionesMarca");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "OmisionesMarca");

            migrationBuilder.DropColumn(
                name: "Instancia",
                table: "OmisionesMarca");

            migrationBuilder.DropColumn(
                name: "ObservacionesResolucion",
                table: "OmisionesMarca");

            migrationBuilder.DropColumn(
                name: "Puesto",
                table: "OmisionesMarca");

            migrationBuilder.DropColumn(
                name: "Resolucion",
                table: "OmisionesMarca");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "OmisionesMarca");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Permisos",
                newName: "FechaSalida");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegreso",
                table: "Permisos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaRegistro", "Password", "UltimaActualizacion" },
                values: new object[] { new DateTime(2025, 3, 18, 21, 53, 21, 367, DateTimeKind.Local).AddTicks(1136), "$2a$11$p.qAmUsgX/bptfWmOLS4zO0N73DYb6zTO5USfE3hoMY16riDLxl/W", new DateTime(2025, 3, 18, 21, 53, 21, 367, DateTimeKind.Local).AddTicks(1153) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaRegistro", "Password", "UltimaActualizacion" },
                values: new object[] { new DateTime(2025, 3, 18, 21, 53, 21, 495, DateTimeKind.Local).AddTicks(8378), "$2a$11$VufOGrj1aYRUAOjqbYaDpuu2cJI4G8t7Zk/duAbtD.TNejBNZUdgi", new DateTime(2025, 3, 18, 21, 53, 21, 495, DateTimeKind.Local).AddTicks(8393) });
        }
    }
}
