using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPermisos.Migrations
{
    public partial class CreateAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cedula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Puesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FotoPerfil = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegistroId = table.Column<int>(type: "int", nullable: true),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionIP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActividad = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEntidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatosAntiguos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatosNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tabla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OmisionesMarca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoOmision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaOmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Justificacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cedula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Instancia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriaPersonal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Resolucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservacionesResolucion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AprobadoPorId = table.Column<int>(type: "int", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComentariosAprobador = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OmisionesMarca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OmisionesMarca_Usuarios_AprobadoPorId",
                        column: x => x.AprobadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OmisionesMarca_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usado = table.Column<bool>(type: "bit", nullable: false),
                    Utilizado = table.Column<bool>(type: "bit", nullable: false),
                    FechaUso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoPermiso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraDesde = table.Column<TimeSpan>(type: "time", nullable: true),
                    HoraHasta = table.Column<TimeSpan>(type: "time", nullable: true),
                    JornadaCompleta = table.Column<bool>(type: "bit", nullable: false),
                    MediaJornada = table.Column<bool>(type: "bit", nullable: false),
                    CantidadLecciones = table.Column<int>(type: "int", nullable: true),
                    Cedula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Puesto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Condicion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoMotivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoConvocatoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HoraSalida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RutaComprobante = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RutaJustificacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Justificado = table.Column<bool>(type: "bit", nullable: false),
                    Resolucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ObservacionesResolucion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoRebajo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AprobadoPorId = table.Column<int>(type: "int", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComentariosAprobador = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permisos_Usuarios_AprobadoPorId",
                        column: x => x.AprobadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Permisos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReporteDano",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Equipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoDano = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaReporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComentariosResolucion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RutaImagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UrlFoto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CostoEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResueltoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AsignadoAUsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteDano", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReporteDano_Usuarios_AsignadoAUsuarioId",
                        column: x => x.AsignadoAUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReporteDano_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TwoFactorAuth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usado = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorAuth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwoFactorAuth_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TwoFactorAuth_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Permiso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Fecha",
                table: "AuditLogs",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UsuarioId",
                table: "AuditLogs",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UsuarioId1",
                table: "AuditLogs",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_OmisionesMarca_AprobadoPorId",
                table: "OmisionesMarca",
                column: "AprobadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_OmisionesMarca_UsuarioId",
                table: "OmisionesMarca",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_Token",
                table: "PasswordResets",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UsuarioId",
                table: "PasswordResets",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UsuarioId1",
                table: "PasswordResets",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_AprobadoPorId",
                table: "Permisos",
                column: "AprobadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_UsuarioId",
                table: "Permisos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteDano_AsignadoAUsuarioId",
                table: "ReporteDano",
                column: "AsignadoAUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReporteDano_UsuarioId",
                table: "ReporteDano",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuth_UsuarioId",
                table: "TwoFactorAuth",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuth_UsuarioId1",
                table: "TwoFactorAuth",
                column: "UsuarioId1",
                unique: true,
                filter: "[UsuarioId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UsuarioId_Permiso",
                table: "UserPermissions",
                columns: new[] { "UsuarioId", "Permiso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "OmisionesMarca");

            migrationBuilder.DropTable(
                name: "PasswordResets");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "ReporteDano");

            migrationBuilder.DropTable(
                name: "TwoFactorAuth");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
