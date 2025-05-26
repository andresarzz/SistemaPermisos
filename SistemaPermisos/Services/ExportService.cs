using OfficeOpenXml;
using SistemaPermisos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

namespace SistemaPermisos.Services
{
    public class ExportService : IExportService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public ExportService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            // Configurar EPPlus para uso no comercial
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        #region Excel Exports

        public async Task<byte[]> ExportUsersToExcelAsync(IEnumerable<Usuario> users)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Usuarios");

            // Encabezados
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Nombre";
            worksheet.Cells[1, 3].Value = "Correo";
            worksheet.Cells[1, 4].Value = "Rol";
            worksheet.Cells[1, 5].Value = "Cédula";
            worksheet.Cells[1, 6].Value = "Puesto";
            worksheet.Cells[1, 7].Value = "Teléfono";
            worksheet.Cells[1, 8].Value = "Departamento";
            worksheet.Cells[1, 9].Value = "Fecha Registro";
            worksheet.Cells[1, 10].Value = "Activo";

            // Estilo de encabezados
            using (var range = worksheet.Cells[1, 1, 1, 10])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Font.Color.SetColor(Color.Black);
            }

            // Datos
            int row = 2;
            foreach (var user in users)
            {
                worksheet.Cells[row, 1].Value = user.Id;
                worksheet.Cells[row, 2].Value = user.Nombre;
                worksheet.Cells[row, 3].Value = user.Correo;
                worksheet.Cells[row, 4].Value = user.Rol;
                worksheet.Cells[row, 5].Value = user.Cedula ?? "";
                worksheet.Cells[row, 6].Value = user.Puesto ?? "";
                worksheet.Cells[row, 7].Value = user.Telefono ?? "";
                worksheet.Cells[row, 8].Value = user.Departamento ?? "";
                worksheet.Cells[row, 9].Value = user.FechaRegistro.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 10].Value = user.Activo ? "Sí" : "No";

                // Colorear filas según estado
                if (!user.Activo)
                {
                    using (var range = worksheet.Cells[row, 1, row, 10])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                    }
                }

                row++;
            }

            // Autoajustar columnas
            worksheet.Cells.AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportPermisosToExcelAsync(IEnumerable<Permiso> permisos)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Permisos");

            // Encabezados
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Usuario";
            worksheet.Cells[1, 3].Value = "Tipo";
            worksheet.Cells[1, 4].Value = "Fecha Inicio";
            worksheet.Cells[1, 5].Value = "Fecha Fin";
            worksheet.Cells[1, 6].Value = "Motivo";
            worksheet.Cells[1, 7].Value = "Estado";
            worksheet.Cells[1, 8].Value = "Fecha Solicitud";

            // Estilo de encabezados
            using (var range = worksheet.Cells[1, 1, 1, 8])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Font.Color.SetColor(Color.Black);
            }

            // Datos
            int row = 2;
            foreach (var permiso in permisos)
            {
                worksheet.Cells[row, 1].Value = permiso.Id;
                worksheet.Cells[row, 2].Value = permiso.Usuario?.Nombre ?? "N/A";
                worksheet.Cells[row, 3].Value = permiso.TipoPermiso;
                worksheet.Cells[row, 4].Value = permiso.FechaInicio.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 5].Value = permiso.FechaFin.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 6].Value = permiso.Motivo;
                worksheet.Cells[row, 7].Value = permiso.Estado;
                worksheet.Cells[row, 8].Value = permiso.FechaSolicitud.ToString("dd/MM/yyyy HH:mm");

                // Colorear filas según estado
                Color backgroundColor = Color.White;
                switch (permiso.Estado)
                {
                    case "Pendiente":
                        backgroundColor = Color.LightYellow;
                        break;
                    case "Aprobado":
                        backgroundColor = Color.LightGreen;
                        break;
                    case "Rechazado":
                        backgroundColor = Color.LightPink;
                        break;
                }

                using (var range = worksheet.Cells[row, 1, row, 8])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                }

                row++;
            }

            // Autoajustar columnas
            worksheet.Cells.AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportOmisionesToExcelAsync(IEnumerable<OmisionMarca> omisiones)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Omisiones de Marca");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Fecha Omisión";
                worksheet.Cells[1, 4].Value = "Tipo Omisión";
                worksheet.Cells[1, 5].Value = "Motivo";
                worksheet.Cells[1, 6].Value = "Estado";
                worksheet.Cells[1, 7].Value = "Fecha Registro";

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.Font.Color.SetColor(Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var omision in omisiones)
                {
                    worksheet.Cells[row, 1].Value = omision.Id;
                    worksheet.Cells[row, 2].Value = omision.Usuario?.Nombre ?? "N/A";
                    worksheet.Cells[row, 3].Value = omision.FechaOmision.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = omision.TipoOmision;
                    worksheet.Cells[row, 5].Value = omision.Motivo;
                    worksheet.Cells[row, 6].Value = omision.Estado;
                    worksheet.Cells[row, 7].Value = omision.FechaRegistro.ToString("dd/MM/yyyy");

                    // Colorear filas según estado
                    Color backgroundColor = Color.White;
                    switch (omision.Estado)
                    {
                        case "Pendiente":
                            backgroundColor = Color.LightYellow;
                            break;
                        case "Aprobado":
                            backgroundColor = Color.LightGreen;
                            break;
                        case "Rechazado":
                            backgroundColor = Color.LightPink;
                            break;
                    }

                    using (var range = worksheet.Cells[row, 1, row, 7])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    }

                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
        }

        public async Task<byte[]> ExportAuditLogToExcelAsync(IEnumerable<AuditLog> auditLogs)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Auditoría");

            // Encabezados
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Usuario";
            worksheet.Cells[1, 3].Value = "Acción";
            worksheet.Cells[1, 4].Value = "Entidad";
            worksheet.Cells[1, 5].Value = "Registro ID";
            worksheet.Cells[1, 6].Value = "Fecha";
            worksheet.Cells[1, 7].Value = "IP";

            // Estilo de encabezados
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Font.Color.SetColor(Color.Black);
            }

            // Datos
            int row = 2;
            foreach (var log in auditLogs)
            {
                worksheet.Cells[row, 1].Value = log.Id;
                worksheet.Cells[row, 2].Value = log.Usuario?.Nombre ?? "Sistema";
                worksheet.Cells[row, 3].Value = log.Accion;
                worksheet.Cells[row, 4].Value = log.Entidad;
                worksheet.Cells[row, 5].Value = log.RegistroId?.ToString() ?? "";
                worksheet.Cells[row, 6].Value = log.Fecha.ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cells[row, 7].Value = log.DireccionIP ?? "";

                // Colorear filas según acción
                Color backgroundColor = Color.White;
                switch (log.Accion.ToLower())
                {
                    case "crear":
                        backgroundColor = Color.LightGreen;
                        break;
                    case "actualizar":
                    case "editar":
                        backgroundColor = Color.LightBlue;
                        break;
                    case "eliminar":
                        backgroundColor = Color.LightCoral;
                        break;
                    case "iniciar sesión":
                    case "cerrar sesión":
                        backgroundColor = Color.LightYellow;
                        break;
                }

                using (var range = worksheet.Cells[row, 1, row, 7])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                }

                row++;
            }

            // Autoajustar columnas
            worksheet.Cells.AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> ExportReportesToExcelAsync(IEnumerable<ReporteDano> reportes)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reportes de Daños");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Equipo";
                worksheet.Cells[1, 4].Value = "Ubicación";
                worksheet.Cells[1, 5].Value = "Descripción";
                worksheet.Cells[1, 6].Value = "Estado";
                worksheet.Cells[1, 7].Value = "Fecha Reporte";

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.Font.Color.SetColor(Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var reporte in reportes)
                {
                    worksheet.Cells[row, 1].Value = reporte.Id;
                    worksheet.Cells[row, 2].Value = reporte.Usuario?.Nombre ?? "N/A";
                    worksheet.Cells[row, 3].Value = reporte.Equipo;
                    worksheet.Cells[row, 4].Value = reporte.Ubicacion;
                    worksheet.Cells[row, 5].Value = reporte.Descripcion;
                    worksheet.Cells[row, 6].Value = reporte.Estado;
                    worksheet.Cells[row, 7].Value = reporte.FechaReporte.ToString("dd/MM/yyyy");

                    // Colorear filas según estado
                    Color backgroundColor = Color.White;
                    switch (reporte.Estado)
                    {
                        case "Pendiente":
                            backgroundColor = Color.LightYellow;
                            break;
                        case "En Proceso":
                            backgroundColor = Color.LightBlue;
                            break;
                        case "Resuelto":
                            backgroundColor = Color.LightGreen;
                            break;
                        case "Cancelado":
                            backgroundColor = Color.LightCoral;
                            break;
                    }

                    using (var range = worksheet.Cells[row, 1, row, 7])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(backgroundColor);
                    }

                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
        }

        #endregion

        #region PDF Exports (Simplified - Only Excel for now)

        public async Task<byte[]> ExportUsersToPdfAsync(IEnumerable<Usuario> users)
        {
            // Por simplicidad, exportamos como Excel por ahora
            return await ExportUsersToExcelAsync(users);
        }

        public async Task<byte[]> ExportPermisosToPdfAsync(IEnumerable<Permiso> permisos)
        {
            // Por simplicidad, exportamos como Excel por ahora
            return await ExportPermisosToExcelAsync(permisos);
        }

        public async Task<byte[]> ExportOmisionesToPdfAsync(IEnumerable<OmisionMarca> omisiones)
        {
            // Por simplicidad, exportamos como Excel por ahora
            return await ExportOmisionesToExcelAsync(omisiones);
        }

        public async Task<byte[]> ExportAuditLogToPdfAsync(IEnumerable<AuditLog> logs)
        {
            // Por simplicidad, exportamos como Excel por ahora
            return await ExportAuditLogToExcelAsync(logs);
        }

        public async Task<byte[]> ExportReportesToPdfAsync(IEnumerable<ReporteDano> reportes)
        {
            // Por simplicidad, exportamos como Excel por ahora
            return await ExportReportesToExcelAsync(reportes);
        }

        #endregion
    }
}
