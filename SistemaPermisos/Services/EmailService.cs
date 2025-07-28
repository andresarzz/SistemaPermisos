using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SistemaPermisos.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:SenderEmail"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]!), MailKit.Security.SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            string subject = "Restablecer su contraseña - Sistema de Permisos";
            string message = $"Por favor, restablezca su contraseña haciendo clic en este enlace: <a href='{resetLink}'>Restablecer Contraseña</a>";
            return await SendEmailAsync(toEmail, subject, message);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            string subject = "Bienvenido a MIPP+";
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
                            <h2>¡Bienvenido a MIPP+!</h2>
                        </div>
                        <div class='content'>
                            <p>Hola {userName},</p>
                            <p>Su cuenta ha sido creada exitosamente en nuestro sistema MIPP+.</p>
                            <p>Ahora puede iniciar sesión y comenzar a utilizar todas las funcionalidades disponibles para gestionar permisos, omisiones y reportes.</p>
                            <p>Si tiene alguna pregunta o necesita ayuda, no dude en contactar al administrador del sistema.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático de MIPP+, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendNotificationAsync(string toEmail, string subject, string message)
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
                            <h2>Notificación - MIPP+</h2>
                        </div>
                        <div class='content'>
                            <h3>{subject}</h3>
                            <p>{message}</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático de MIPP+, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> Send2FACodeAsync(string toEmail, string code)
        {
            string subject = "Código de Verificación - MIPP+";
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
                            <h2>Código de Verificación - MIPP+</h2>
                        </div>
                        <div class='content'>
                            <p>Su código de verificación para MIPP+ es:</p>
                            <div class='code'>{code}</div>
                            <p>Este código expirará en 5 minutos.</p>
                            <p>Si no solicitó este código, por favor ignore este correo y cambie su contraseña inmediatamente.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático de MIPP+, por favor no responda a este mensaje.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}
