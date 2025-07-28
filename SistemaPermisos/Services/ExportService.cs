using OfficeOpenXml;
using SistemaPermisos.Models;
using System.Collections.Generic;
using System.IO;

namespace SistemaPermisos.Services
{
    public class ExportService : IExportService
    {
        public byte[] ExportUsersToExcel(List<Usuario> users)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Usuarios");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "Apellidos";
                worksheet.Cells[1, 4].Value = "Nombre de Usuario";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "Rol";
                worksheet.Cells[1, 7].Value = "Cédula";
                worksheet.Cells[1, 8].Value = "Puesto";
                worksheet.Cells[1, 9].Value = "Teléfono";
                worksheet.Cells[1, 10].Value = "Departamento";
                worksheet.Cells[1, 11].Value = "Dirección";
                worksheet.Cells[1, 12].Value = "Fecha Nacimiento";
                worksheet.Cells[1, 13].Value = "Activo";
                worksheet.Cells[1, 14].Value = "Fecha Registro";
                worksheet.Cells[1, 15].Value = "Último Acceso";

                for (int i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    worksheet.Cells[i + 2, 1].Value = user.Id;
                    worksheet.Cells[i + 2, 2].Value = user.Nombre;
                    worksheet.Cells[i + 2, 3].Value = user.Apellidos;
                    worksheet.Cells[i + 2, 4].Value = user.NombreUsuario;
                    worksheet.Cells[i + 2, 5].Value = user.Email;
                    worksheet.Cells[i + 2, 6].Value = user.Rol;
                    worksheet.Cells[i + 2, 7].Value = user.Cedula;
                    worksheet.Cells[i + 2, 8].Value = user.Puesto;
                    worksheet.Cells[i + 2, 9].Value = user.Telefono;
                    worksheet.Cells[i + 2, 10].Value = user.Departamento;
                    worksheet.Cells[i + 2, 11].Value = user.Direccion;
                    worksheet.Cells[i + 2, 12].Value = user.FechaNacimiento?.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 13].Value = user.IsActive ? "Sí" : "No";
                    worksheet.Cells[i + 2, 14].Value = user.FechaRegistro.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 15].Value = user.UltimoAcceso?.ToString("yyyy-MM-dd HH:mm");
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportPermisosToExcel(List<Permiso> permisos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Permisos");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Tipo Permiso";
                worksheet.Cells[1, 4].Value = "Fecha Inicio";
                worksheet.Cells[1, 5].Value = "Fecha Fin";
                worksheet.Cells[1, 6].Value = "Motivo";
                worksheet.Cells[1, 7].Value = "Estado";
                worksheet.Cells[1, 8].Value = "Fecha Solicitud";
                worksheet.Cells[1, 9].Value = "Aprobado Por";
                worksheet.Cells[1, 10].Value = "Fecha Resolución";
                worksheet.Cells[1, 11].Value = "Comentarios Aprobador";

                for (int i = 0; i < permisos.Count; i++)
                {
                    var permiso = permisos[i];
                    worksheet.Cells[i + 2, 1].Value = permiso.Id;
                    worksheet.Cells[i + 2, 2].Value = permiso.Usuario?.NombreUsuario;
                    worksheet.Cells[i + 2, 3].Value = permiso.TipoPermiso;
                    worksheet.Cells[i + 2, 4].Value = permiso.FechaInicio.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = permiso.FechaFin.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 6].Value = permiso.Motivo;
                    worksheet.Cells[i + 2, 7].Value = permiso.Estado;
                    worksheet.Cells[i + 2, 8].Value = permiso.FechaSolicitud.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 9].Value = permiso.AprobadoPor?.NombreUsuario;
                    worksheet.Cells[i + 2, 10].Value = permiso.FechaResolucion?.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 11].Value = permiso.ComentariosAprobador;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportOmisionesToExcel(List<OmisionMarca> omisiones)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Omisiones de Marca");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Tipo Omisión";
                worksheet.Cells[1, 4].Value = "Fecha Omisión";
                worksheet.Cells[1, 5].Value = "Hora Omisión";
                worksheet.Cells[1, 6].Value = "Motivo";
                worksheet.Cells[1, 7].Value = "Estado";
                worksheet.Cells[1, 8].Value = "Fecha Solicitud";
                worksheet.Cells[1, 9].Value = "Aprobado Por";
                worksheet.Cells[1, 10].Value = "Fecha Resolución";
                worksheet.Cells[1, 11].Value = "Comentarios Aprobador";

                for (int i = 0; i < omisiones.Count; i++)
                {
                    var omision = omisiones[i];
                    worksheet.Cells[i + 2, 1].Value = omision.Id;
                    worksheet.Cells[i + 2, 2].Value = omision.Usuario?.NombreUsuario;
                    worksheet.Cells[i + 2, 3].Value = omision.TipoOmision;
                    worksheet.Cells[i + 2, 4].Value = omision.FechaOmision.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 5].Value = omision.HoraOmision.ToString(@"hh\:mm");
                    worksheet.Cells[i + 2, 6].Value = omision.Motivo;
                    worksheet.Cells[i + 2, 7].Value = omision.Estado;
                    worksheet.Cells[i + 2, 8].Value = omision.FechaSolicitud.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 9].Value = omision.AprobadoPor?.NombreUsuario;
                    worksheet.Cells[i + 2, 10].Value = omision.FechaResolucion?.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 11].Value = omision.ComentariosAprobador;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportReportesToExcel(List<ReporteDano> reportes)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reportes de Daño");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Tipo Daño";
                worksheet.Cells[1, 4].Value = "Descripción";
                worksheet.Cells[1, 5].Value = "Fecha Reporte";
                worksheet.Cells[1, 6].Value = "Equipo";
                worksheet.Cells[1, 7].Value = "Ubicación";
                worksheet.Cells[1, 8].Value = "Estado";
                worksheet.Cells[1, 9].Value = "Resuelto Por";
                worksheet.Cells[1, 10].Value = "Fecha Resolución";
                worksheet.Cells[1, 11].Value = "Observaciones Resolución";

                for (int i = 0; i < reportes.Count; i++)
                {
                    var reporte = reportes[i];
                    worksheet.Cells[i + 2, 1].Value = reporte.Id;
                    worksheet.Cells[i + 2, 2].Value = reporte.Usuario?.NombreUsuario;
                    worksheet.Cells[i + 2, 3].Value = reporte.TipoDano;
                    worksheet.Cells[i + 2, 4].Value = reporte.Descripcion;
                    worksheet.Cells[i + 2, 5].Value = reporte.FechaReporte.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 6].Value = reporte.Equipo;
                    worksheet.Cells[i + 2, 7].Value = reporte.Ubicacion;
                    worksheet.Cells[i + 2, 8].Value = reporte.Estado;
                    worksheet.Cells[i + 2, 9].Value = reporte.ResueltoPor?.NombreUsuario;
                    worksheet.Cells[i + 2, 10].Value = reporte.FechaResolucion?.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[i + 2, 11].Value = reporte.ObservacionesResolucion;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportAuditLogsToExcel(List<AuditLog> auditLogs)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Registro de Auditoría");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Acción";
                worksheet.Cells[1, 4].Value = "Detalles";
                worksheet.Cells[1, 5].Value = "Fecha y Hora";
                worksheet.Cells[1, 6].Value = "Dirección IP";
                worksheet.Cells[1, 7].Value = "User Agent";

                for (int i = 0; i < auditLogs.Count; i++)
                {
                    var log = auditLogs[i];
                    worksheet.Cells[i + 2, 1].Value = log.Id;
                    worksheet.Cells[i + 2, 2].Value = log.Usuario?.NombreUsuario;
                    worksheet.Cells[i + 2, 3].Value = log.Accion;
                    worksheet.Cells[i + 2, 4].Value = log.Detalles;
                    worksheet.Cells[i + 2, 5].Value = log.FechaHora.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[i + 2, 6].Value = log.IpAddress;
                    worksheet.Cells[i + 2, 7].Value = log.UserAgent;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
    }
}
