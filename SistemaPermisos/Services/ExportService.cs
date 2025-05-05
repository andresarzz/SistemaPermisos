using OfficeOpenXml;
using SistemaPermisos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SistemaPermisos.Services
{
    public class ExportService : IExportService
    {
        public ExportService()
        {
            // Corregir advertencia: LicenseContext está obsoleto
            // Cambiar de:
            // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // A:
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        public Task<byte[]> ExportUsersToExcelAsync(IEnumerable<Usuario> users)
        {
            // Agregar await para resolver advertencia CS1998
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
                worksheet.Cells[1, 7].Value = "Estado";
                worksheet.Cells[1, 8].Value = "Fecha Registro";

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
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
                    worksheet.Cells[row, 7].Value = user.Activo ? "Activo" : "Inactivo";
                    worksheet.Cells[row, 8].Value = user.FechaRegistro.ToString("dd/MM/yyyy");
                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                // Cambio de return await package.GetAsByteArrayAsync() a:
                return Task.FromResult(package.GetAsByteArray());
            }
        }

        public Task<byte[]> ExportUsersToPdfAsync(IEnumerable<Usuario> users)
        {
            // Agregar await para resolver advertencia CS1998
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Listado de Usuarios", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 3f, 2f, 2f, 2f, 2f });

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Nombre", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Correo", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Rol", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Cédula", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Estado", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Fecha Registro", headerFont)));

                // Estilo de encabezados
                foreach (PdfPCell cell in table.GetRow(0).GetCells())
                {
                    cell.BackgroundColor = new BaseColor(211, 211, 211);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5f;
                }

                // Datos
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var user in users)
                {
                    table.AddCell(new Phrase(user.Id.ToString(), cellFont));
                    table.AddCell(new Phrase(user.Nombre, cellFont));
                    table.AddCell(new Phrase(user.Correo, cellFont));
                    table.AddCell(new Phrase(user.Rol, cellFont));
                    table.AddCell(new Phrase(user.Cedula ?? "N/A", cellFont));
                    table.AddCell(new Phrase(user.Activo ? "Activo" : "Inactivo", cellFont));
                    table.AddCell(new Phrase(user.FechaRegistro.ToString("dd/MM/yyyy"), cellFont));
                }

                document.Add(table);

                // Pie de página
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", cellFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                // Cambio de return await Task.FromResult(memoryStream.ToArray()) a:
                return Task.FromResult(memoryStream.ToArray());
            }
        }

        public Task<byte[]> ExportPermisosToExcelAsync(IEnumerable<Permiso> permisos)
        {
            // Agregar await para resolver advertencia CS1998
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

                // Estilo de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
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
                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                // Cambio de return await package.GetAsByteArrayAsync() a:
                return Task.FromResult(package.GetAsByteArray());
            }
        }

        public Task<byte[]> ExportPermisosToPdfAsync(IEnumerable<Permiso> permisos)
        {
            // Agregar await para resolver advertencia CS1998
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Listado de Permisos", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 2f, 3f, 2f, 2f, 2f });

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Usuario", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Fecha", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Motivo", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Estado", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Justificado", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Fecha Solicitud", headerFont)));

                // Estilo de encabezados
                foreach (PdfPCell cell in table.GetRow(0).GetCells())
                {
                    cell.BackgroundColor = new BaseColor(211, 211, 211);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5f;
                }

                // Datos
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var permiso in permisos)
                {
                    table.AddCell(new Phrase(permiso.Id.ToString(), cellFont));
                    table.AddCell(new Phrase(permiso.Usuario?.Nombre ?? "N/A", cellFont));
                    table.AddCell(new Phrase(permiso.Fecha.ToString("dd/MM/yyyy"), cellFont));
                    table.AddCell(new Phrase(permiso.Motivo, cellFont));
                    table.AddCell(new Phrase(permiso.Estado, cellFont));
                    table.AddCell(new Phrase(permiso.Justificado ? "Sí" : "No", cellFont));
                    table.AddCell(new Phrase(permiso.FechaSolicitud.ToString("dd/MM/yyyy"), cellFont));
                }

                document.Add(table);

                // Pie de página
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", cellFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                // Cambio de return await Task.FromResult(memoryStream.ToArray()) a:
                return Task.FromResult(memoryStream.ToArray());
            }
        }

        public Task<byte[]> ExportOmisionesToExcelAsync(IEnumerable<OmisionMarca> omisiones)
        {
            // Corregir el error de sintaxis en el rango de celdas y agregar await
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Omisiones");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Usuario";
                worksheet.Cells[1, 3].Value = "Fecha Omisión";
                worksheet.Cells[1, 4].Value = "Tipo Omisión";
                worksheet.Cells[1, 5].Value = "Motivo";
                worksheet.Cells[1, 6].Value = "Estado";
                worksheet.Cells[1, 7].Value = "Fecha Registro";

                // Estilo de encabezados - Corregir sintaxis
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
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
                    row++;
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                // Cambio de return await package.GetAsByteArrayAsync() a:
                return Task.FromResult(package.GetAsByteArray());
            }
        }

        public Task<byte[]> ExportOmisionesToPdfAsync(IEnumerable<OmisionMarca> omisiones)
        {
            // Agregar await para resolver advertencia CS1998
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Listado de Omisiones de Marca", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 2f, 2f, 3f, 2f, 2f });

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Usuario", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Fecha Omisión", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Tipo Omisión", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Motivo", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Estado", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Fecha Registro", headerFont)));

                // Estilo de encabezados
                foreach (PdfPCell cell in table.GetRow(0).GetCells())
                {
                    cell.BackgroundColor = new BaseColor(211, 211, 211);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5f;
                }

                // Datos
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var omision in omisiones)
                {
                    table.AddCell(new Phrase(omision.Id.ToString(), cellFont));
                    table.AddCell(new Phrase(omision.Usuario?.Nombre ?? "N/A", cellFont));
                    table.AddCell(new Phrase(omision.FechaOmision.ToString("dd/MM/yyyy"), cellFont));
                    table.AddCell(new Phrase(omision.TipoOmision, cellFont));
                    table.AddCell(new Phrase(omision.Motivo, cellFont));
                    table.AddCell(new Phrase(omision.Estado, cellFont));
                    table.AddCell(new Phrase(omision.FechaRegistro.ToString("dd/MM/yyyy"), cellFont));
                }

                document.Add(table);

                // Pie de página
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", cellFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                // Cambio de return await Task.FromResult(memoryStream.ToArray()) a:
                return Task.FromResult(memoryStream.ToArray());
            }
        }

        public Task<byte[]> ExportAuditLogToExcelAsync(IEnumerable<AuditLog> auditLogs)
        {
            // Agregar await para resolver advertencia CS1998
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

                // Cambio de return await package.GetAsByteArrayAsync() a:
                return Task.FromResult(package.GetAsByteArray());
            }
        }

        public Task<byte[]> ExportAuditLogToPdfAsync(IEnumerable<AuditLog> auditLogs)
        {
            // Agregar await para resolver advertencia CS1998
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("Registro de Auditoría", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(7);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 2f, 2f, 2f, 1f, 2f, 2.5f });

                // Encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Usuario", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Acción", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Tabla", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Reg. ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("IP", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Fecha", headerFont)));

                // Estilo de encabezados
                foreach (PdfPCell cell in table.GetRow(0).GetCells())
                {
                    cell.BackgroundColor = new BaseColor(211, 211, 211);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5f;
                }

                // Datos
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var log in auditLogs)
                {
                    table.AddCell(new Phrase(log.Id.ToString(), cellFont));
                    table.AddCell(new Phrase(log.Usuario?.Nombre ?? "Sistema", cellFont));
                    table.AddCell(new Phrase(log.Accion, cellFont));
                    table.AddCell(new Phrase(log.Tabla, cellFont));
                    table.AddCell(new Phrase(log.RegistroId?.ToString() ?? "N/A", cellFont));
                    table.AddCell(new Phrase(log.DireccionIP, cellFont));
                    table.AddCell(new Phrase(log.Fecha.ToString("dd/MM/yyyy HH:mm:ss"), cellFont));
                }

                document.Add(table);

                // Pie de página
                Paragraph footer = new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", cellFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                footer.SpacingBefore = 10f;
                document.Add(footer);

                document.Close();
                writer.Close();

                // Cambio de return await Task.FromResult(memoryStream.ToArray()) a:
                return Task.FromResult(memoryStream.ToArray());
            }
        }
    }
}
