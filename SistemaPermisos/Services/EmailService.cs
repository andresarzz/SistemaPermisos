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
        private readonly bool _enableSsl;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            _fromEmail = _configuration["EmailSettings:FromEmail"];
            _fromName = _configuration["EmailSettings:FromName"];
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
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
                    client.EnableSsl = _enableSsl;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    await client.SendMailAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Registrar el error
                System.Diagnostics.Debug.WriteLine($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetLink)
        {
            string subject = "Restablecimiento de Contraseña";
            string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #f8f9fa; padding: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; }}
                        .footer {{ background-color: #f8f9fa; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Restablecimiento de Contraseña</h2>
                        </div>
                        <div class='content'>
                            <p>Hemos recibido una solicitud para restablecer la contraseña de su cuenta.</p>
                            <p>Para continuar con el proceso, haga clic en el siguiente enlace:</p>
                            <p><a href='{resetLink}' class='button'>Restablecer Contraseña</a></p>
                            <p>Si no solicitó restablecer su contraseña, puede ignorar este correo.</p>
                            <p>El enlace expirará en 24 horas.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendWelcomeEmailAsync(string to, string userName)
        {
            string subject = "Bienvenido al Sistema de Permisos";
            string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #f8f9fa; padding: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .footer {{ background-color: #f8f9fa; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>¡Bienvenido al Sistema de Permisos!</h2>
                        </div>
                        <div class='content'>
                            <p>Hola {userName},</p>
                            <p>Su cuenta ha sido creada exitosamente en nuestro sistema.</p>
                            <p>Ahora puede iniciar sesión y comenzar a utilizar todas las funcionalidades disponibles.</p>
                            <p>Si tiene alguna pregunta o necesita ayuda, no dude en contactar al administrador del sistema.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> SendNotificationAsync(string to, string subject, string message)
        {
            string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #f8f9fa; padding: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .footer {{ background-color: #f8f9fa; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>{subject}</h2>
                        </div>
                        <div class='content'>
                            <p>{message}</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }

        public async Task<bool> Send2FACodeAsync(string to, string code)
        {
            string subject = "Código de Verificación";
            string body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #f8f9fa; padding: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .code {{ font-size: 24px; font-weight: bold; text-align: center; padding: 10px; background-color: #f8f9fa; margin: 20px 0; }}
                        .footer {{ background-color: #f8f9fa; padding: 10px; text-align: center; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Código de Verificación</h2>
                        </div>
                        <div class='content'>
                            <p>Su código de verificación es:</p>
                            <div class='code'>{code}</div>
                            <p>Este código expirará en 5 minutos.</p>
                            <p>Si no solicitó este código, por favor ignore este correo y cambie su contraseña inmediatamente.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }
    }
}
