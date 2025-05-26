using OfficeOpenXml;
using SistemaPermisos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;

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
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Usuarios");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "Correo";
                worksheet.Cells[1, 4].Value = "Rol";
                worksheet.Cells[1, 5].Value = "Cédula";
                worksheet.Cells[1, 6].Value = "Puesto";
                worksheet.Cells[1, 7].Value = "Departamento";
                worksheet.Cells[1, 8].Value = "Teléfono";
                worksheet.Cells[1, 9].Value = "Fecha Registro";
                worksheet.Cells[1, 10].Value = "Estado";

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = user.Id;
                    worksheet.Cells[row, 2].Value = user.Nombre;
                    worksheet.Cells[row, 3].Value = user.Correo;
                    worksheet.Cells[row, 4].Value = user.Rol;
                    worksheet.Cells[row, 5].Value = user.Cedula;
                    worksheet.Cells[row, 6].Value = user.Puesto;
                    worksheet.Cells[row, 7].Value = user.Departamento;
                    worksheet.Cells[row, 8].Value = user.Telefono;
                    worksheet.Cells[row, 9].Value = user.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 10].Value = user.Activo ? "Activo" : "Inactivo";

                    // Colorear filas según estado
                    if (!user.Activo)
                    {
                        using (var range = worksheet.Cells[row, 1, row, 10])
                        {
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                        }
                    }

                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
        }

        public async Task<byte[]> ExportPermisosToExcelAsync(IEnumerable<Permiso> permisos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Permisos");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Fecha";
                worksheet.Cells[1, 4].Value = "Motivo";
                worksheet.Cells[1, 5].Value = "Estado";
                worksheet.Cells[1, 6].Value = "Justificado";
                worksheet.Cells[1, 7].Value = "Fecha Solicitud";
                worksheet.Cells[1, 8].Value = "Observaciones";

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                // Datos
                int row = 2;
                foreach (var permiso in permisos)
                {
                    worksheet.Cells[row, 1].Value = permiso.Id;
                    worksheet.Cells[row, 2].Value = permiso.Usuario?.Nombre ?? "N/A";
                    worksheet.Cells[row, 3].Value = permiso.Fecha.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = permiso.Motivo;
                    worksheet.Cells[row, 5].Value = permiso.Estado;
                    worksheet.Cells[row, 6].Value = permiso.Justificado ? "Sí" : "No";
                    worksheet.Cells[row, 7].Value = permiso.FechaSolicitud.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 8].Value = permiso.Observaciones;

                    // Colorear filas según estado
                    System.Drawing.Color backgroundColor = System.Drawing.Color.White;
                    switch (permiso.Estado)
                    {
                        case "Pendiente":
                            backgroundColor = System.Drawing.Color.LightYellow;
                            break;
                        case "Aprobado":
                            backgroundColor = System.Drawing.Color.LightGreen;
                            break;
                        case "Rechazado":
                            backgroundColor = System.Drawing.Color.LightPink;
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

                return await package.GetAsByteArrayAsync();
            }
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
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
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
                    System.Drawing.Color backgroundColor = System.Drawing.Color.White;
                    switch (omision.Estado)
                    {
                        case "Pendiente":
                            backgroundColor = System.Drawing.Color.LightYellow;
                            break;
                        case "Aprobado":
                            backgroundColor = System.Drawing.Color.LightGreen;
                            break;
                        case "Rechazado":
                            backgroundColor = System.Drawing.Color.LightPink;
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
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Auditoría");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Acción";
                worksheet.Cells[1, 4].Value = "Tabla";
                worksheet.Cells[1, 5].Value = "Registro ID";
                worksheet.Cells[1, 6].Value = "Dirección IP";
                worksheet.Cells[1, 7].Value = "Fecha";

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Datos
                int row = 2;
                foreach (var log in auditLogs)
                {
                    worksheet.Cells[row, 1].Value = log.Id;
                    worksheet.Cells[row, 2].Value = log.Usuario?.Nombre ?? "Sistema";
                    worksheet.Cells[row, 3].Value = log.Accion;
                    worksheet.Cells[row, 4].Value = log.Tabla;
                    worksheet.Cells[row, 5].Value = log.RegistroId?.ToString() ?? "N/A";
                    worksheet.Cells[row, 6].Value = log.DireccionIP;
                    worksheet.Cells[row, 7].Value = log.Fecha.ToString("dd/MM/yyyy HH:mm:ss");
                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
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
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
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
                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
        }

        #endregion

        #region PDF Exports

        public async Task<byte[]> ExportUsersToPdfAsync(IEnumerable<Usuario> users)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                Paragraph title = new Paragraph("Listado de Usuarios", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 0.5f, 2f, 2f, 1f, 1f, 1.5f, 1f };
                table.SetWidths(widths);

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 51, 51);

                AddCellToTable(table, "ID", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Nombre", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Correo", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Rol", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Cédula", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Departamento", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Estado", headerFont, headerBackground, Element.ALIGN_CENTER);

                // Datos
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor activeColor = new BaseColor(240, 255, 240);
                BaseColor inactiveColor = new BaseColor(255, 240, 240);

                foreach (var user in users)
                {
                    BaseColor rowColor = user.Activo ? activeColor : inactiveColor;

                    AddCellToTable(table, user.Id.ToString(), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, user.Nombre, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, user.Correo, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, user.Rol, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, user.Cedula ?? "N/A", dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, user.Departamento ?? "N/A", dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, user.Activo ? "Activo" : "Inactivo", dataFont, rowColor, Element.ALIGN_CENTER);
                }

                document.Add(table);

                // Pie de página
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", footerFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportPermisosToPdfAsync(IEnumerable<Permiso> permisos)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                Paragraph title = new Paragraph("Listado de Permisos", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 0.5f, 2f, 1.5f, 2f, 1f, 1f, 1.5f };
                table.SetWidths(widths);

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 51, 51);

                AddCellToTable(table, "ID", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Usuario", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Fecha", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Motivo", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Estado", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Justificado", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Fecha Solicitud", headerFont, headerBackground, Element.ALIGN_CENTER);

                // Datos
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor pendingColor = new BaseColor(255, 255, 224);
                BaseColor approvedColor = new BaseColor(240, 255, 240);
                BaseColor rejectedColor = new BaseColor(255, 240, 240);

                foreach (var permiso in permisos)
                {
                    BaseColor rowColor;
                    switch (permiso.Estado)
                    {
                        case "Pendiente":
                            rowColor = pendingColor;
                            break;
                        case "Aprobado":
                            rowColor = approvedColor;
                            break;
                        case "Rechazado":
                            rowColor = rejectedColor;
                            break;
                        default:
                            rowColor = BaseColor.WHITE;
                            break;
                    }

                    AddCellToTable(table, permiso.Id.ToString(), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, permiso.Usuario?.Nombre ?? "N/A", dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, permiso.Fecha.ToString("dd/MM/yyyy"), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, permiso.Motivo, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, permiso.Estado, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, permiso.Justificado ? "Sí" : "No", dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, permiso.FechaSolicitud.ToString("dd/MM/yyyy"), dataFont, rowColor, Element.ALIGN_CENTER);
                }

                document.Add(table);

                // Pie de página
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", footerFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportOmisionesToPdfAsync(IEnumerable<OmisionMarca> omisiones)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                Paragraph title = new Paragraph("Listado de Omisiones de Marca", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 0.5f, 2f, 1.5f, 1f, 2f, 1f, 1.5f };
                table.SetWidths(widths);

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 51, 51);

                AddCellToTable(table, "ID", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Usuario", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Fecha", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Tipo", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Motivo", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Estado", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Fecha Registro", headerFont, headerBackground, Element.ALIGN_CENTER);

                // Datos
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor pendingColor = new BaseColor(255, 255, 224);
                BaseColor approvedColor = new BaseColor(240, 255, 240);
                BaseColor rejectedColor = new BaseColor(255, 240, 240);

                foreach (var omision in omisiones)
                {
                    BaseColor rowColor;
                    switch (omision.Estado)
                    {
                        case "Pendiente":
                            rowColor = pendingColor;
                            break;
                        case "Aprobado":
                            rowColor = approvedColor;
                            break;
                        case "Rechazado":
                            rowColor = rejectedColor;
                            break;
                        default:
                            rowColor = BaseColor.WHITE;
                            break;
                    }

                    AddCellToTable(table, omision.Id.ToString(), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, omision.Usuario?.Nombre ?? "N/A", dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, omision.FechaOmision.ToString("dd/MM/yyyy"), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, omision.TipoOmision, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, omision.Motivo, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, omision.Estado, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, omision.FechaRegistro.ToString("dd/MM/yyyy"), dataFont, rowColor, Element.ALIGN_CENTER);
                }

                document.Add(table);

                // Pie de página
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", footerFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportAuditLogToPdfAsync(IEnumerable<AuditLog> logs)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                Paragraph title = new Paragraph("Registro de Auditoría", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 0.5f, 1.5f, 1f, 1f, 0.8f, 2.2f, 1.5f };
                table.SetWidths(widths);

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 51, 51);

                AddCellToTable(table, "ID", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Usuario", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Acción", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Tabla", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Reg. ID", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "IP", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Fecha", headerFont, headerBackground, Element.ALIGN_CENTER);

                // Datos
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor createColor = new BaseColor(240, 255, 240);
                BaseColor updateColor = new BaseColor(224, 240, 255);
                BaseColor deleteColor = new BaseColor(255, 240, 240);
                BaseColor loginColor = new BaseColor(255, 255, 224);

                foreach (var log in logs)
                {
                    BaseColor rowColor;
                    switch (log.Accion.ToLower())
                    {
                        case "crear":
                            rowColor = createColor;
                            break;
                        case "actualizar":
                        case "editar":
                            rowColor = updateColor;
                            break;
                        case "eliminar":
                            rowColor = deleteColor;
                            break;
                        case "iniciar sesión":
                        case "cerrar sesión":
                            rowColor = loginColor;
                            break;
                        default:
                            rowColor = BaseColor.WHITE;
                            break;
                    }

                    AddCellToTable(table, log.Id.ToString(), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, log.Usuario?.Nombre ?? "Sistema", dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, log.Accion, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, log.Tabla, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, log.RegistroId?.ToString() ?? "N/A", dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, log.DireccionIP, dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, log.Fecha.ToString("dd/MM/yyyy HH:mm:ss"), dataFont, rowColor, Element.ALIGN_CENTER);
                }

                document.Add(table);

                // Pie de página
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", footerFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportReportesToPdfAsync(IEnumerable<ReporteDano> reportes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                Paragraph title = new Paragraph("Listado de Reportes de Daños", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 0.5f, 2f, 1.5f, 1.5f, 2f, 1f };
                table.SetWidths(widths);

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBackground = new BaseColor(51, 51, 51);

                AddCellToTable(table, "ID", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Usuario", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Equipo", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Ubicación", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Descripción", headerFont, headerBackground, Element.ALIGN_CENTER);
                AddCellToTable(table, "Estado", headerFont, headerBackground, Element.ALIGN_CENTER);

                // Datos
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
                BaseColor pendingColor = new BaseColor(255, 255, 224);
                BaseColor inProcessColor = new BaseColor(224, 240, 255);
                BaseColor resolvedColor = new BaseColor(240, 255, 240);
                BaseColor canceledColor = new BaseColor(255, 240, 240);

                foreach (var reporte in reportes)
                {
                    BaseColor rowColor;
                    switch (reporte.Estado)
                    {
                        case "Pendiente":
                            rowColor = pendingColor;
                            break;
                        case "En Proceso":
                            rowColor = inProcessColor;
                            break;
                        case "Resuelto":
                            rowColor = resolvedColor;
                            break;
                        case "Cancelado":
                            rowColor = canceledColor;
                            break;
                        default:
                            rowColor = BaseColor.WHITE;
                            break;
                    }

                    AddCellToTable(table, reporte.Id.ToString(), dataFont, rowColor, Element.ALIGN_CENTER);
                    AddCellToTable(table, reporte.Usuario?.Nombre ?? "N/A", dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, reporte.Equipo, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, reporte.Ubicacion, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, reporte.Descripcion, dataFont, rowColor, Element.ALIGN_LEFT);
                    AddCellToTable(table, reporte.Estado, dataFont, rowColor, Element.ALIGN_CENTER);
                }

                document.Add(table);

                // Pie de página
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", footerFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        #endregion

        #region Helper Methods

        private void AddCellToTable(PdfPTable table, string text, Font font, BaseColor backgroundColor, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            table.AddCell(cell);
        }

        // Método para convertir TimeSpan a string para mostrar en reportes
        private string TimeSpanToString(TimeSpan? timeSpan)
        {
            if (timeSpan.HasValue)
            {
                return timeSpan.Value.ToString(@"hh\:mm");
            }
            return "N/A";
        }

        #endregion
    }
}
