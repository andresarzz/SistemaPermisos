using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["Email:SmtpServer"];
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            _smtpUsername = _configuration["Email:SmtpUsername"];
            _smtpPassword = _configuration["Email:SmtpPassword"];
            _fromEmail = _configuration["Email:FromEmail"];
            _fromName = _configuration["Email:FromName"];
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                message.To.Add(new MailAddress(to));

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;
                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string to, string name)
        {
            string subject = "Bienvenido al Sistema de Gestión de Permisos";
            string body = $@"
                <html>
                <body>
                    <h2>Bienvenido, {name}!</h2>
                    <p>Su cuenta ha sido creada exitosamente en el Sistema de Gestión de Permisos.</p>
                    <p>Ahora puede iniciar sesión y comenzar a utilizar el sistema.</p>
                    <p>Saludos cordiales,<br>El equipo de administración</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string to, string name, string resetLink)
        {
            string subject = "Restablecimiento de Contraseña";
            string body = $@"
                <html>
                <body>
                    <h2>Hola, {name}</h2>
                    <p>Hemos recibido una solicitud para restablecer su contraseña.</p>
                    <p>Para continuar con el proceso, haga clic en el siguiente enlace:</p>
                    <p><a href='{resetLink}'>Restablecer mi contraseña</a></p>
                    <p>Este enlace expirará en 24 horas.</p>
                    <p>Si usted no solicitó este cambio, puede ignorar este correo.</p>
                    <p>Saludos cordiales,<br>El equipo de administración</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task Send2FACodeAsync(string to, string name, string code)
        {
            string subject = "Código de Verificación";
            string body = $@"
                <html>
                <body>
                    <h2>Hola, {name}</h2>
                    <p>Su código de verificación es:</p>
                    <h1 style='text-align: center; font-size: 32px; letter-spacing: 5px;'>{code}</h1>
                    <p>Este código expirará en 5 minutos.</p>
                    <p>Si usted no solicitó este código, por favor contacte al administrador.</p>
                    <p>Saludos cordiales,<br>El equipo de administración</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPermissionApprovedEmailAsync(string to, string name, int permisoId)
        {
            string subject = "Solicitud de Permiso Aprobada";
            string body = $@"
                <html>
                <body>
                    <h2>Hola, {name}</h2>
                    <p>Nos complace informarle que su solicitud de permiso (ID: {permisoId}) ha sido aprobada.</p>
                    <p>Puede ver los detalles de la aprobación en el sistema.</p>
                    <p>Saludos cordiales,<br>El equipo de administración</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPermissionRejectedEmailAsync(string to, string name, int permisoId, string reason)
        {
            string subject = "Solicitud de Permiso Rechazada";
            string body = $@"
                <html>
                <body>
                    <h2>Hola, {name}</h2>
                    <p>Lamentamos informarle que su solicitud de permiso (ID: {permisoId}) ha sido rechazada.</p>
                    <p><strong>Motivo:</strong> {reason}</p>
                    <p>Si tiene alguna pregunta, por favor contacte al administrador.</p>
                    <p>Saludos cordiales,<br>El equipo de administración</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }
    }
}

