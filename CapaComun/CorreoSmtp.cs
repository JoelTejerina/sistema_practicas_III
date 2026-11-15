using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace CapaComun
{
    /// <summary>
    /// Envio de mails por SMTP (recuperacion de contraseña). Se configura en App.config (appSettings:
    /// SmtpHost, SmtpPort, SmtpUsuario, SmtpClave, SmtpRemitente, SmtpSsl) o con variables de entorno
    /// SISTEMA_SMTP_HOST / _PORT / _USUARIO / _CLAVE / _REMITENTE / _SSL, que tienen prioridad.
    /// </summary>
    public static class CorreoSmtp
    {
        private static string Valor(string clave, string variableEntorno)
        {
            string entorno = Environment.GetEnvironmentVariable(variableEntorno);
            if (!string.IsNullOrWhiteSpace(entorno))
            {
                return entorno.Trim();
            }
            string config = ConfigurationManager.AppSettings[clave];
            return config == null ? "" : config.Trim();
        }

        public static bool Configurado
        {
            get { return Valor("SmtpHost", "SISTEMA_SMTP_HOST").Length > 0; }
        }

        public static void Enviar(string para, string asunto, string cuerpo)
        {
            string host = Valor("SmtpHost", "SISTEMA_SMTP_HOST");
            if (host.Length == 0)
            {
                throw new InvalidOperationException("El envio de correo no esta configurado.");
            }

            int puerto;
            if (!int.TryParse(Valor("SmtpPuerto", "SISTEMA_SMTP_PORT"), out puerto)) puerto = 587;
            bool ssl;
            string textoSsl = Valor("SmtpSsl", "SISTEMA_SMTP_SSL");
            if (!bool.TryParse(textoSsl, out ssl)) ssl = textoSsl.Length == 0 && puerto != 25 && puerto != 2525;
            string usuario = Valor("SmtpUsuario", "SISTEMA_SMTP_USUARIO");
            string clave = Valor("SmtpClave", "SISTEMA_SMTP_CLAVE");
            string remitente = Valor("SmtpRemitente", "SISTEMA_SMTP_REMITENTE");
            if (remitente.Length == 0) remitente = usuario.Length > 0 ? usuario : "no-responder@sistema.local";

            using (SmtpClient cliente = new SmtpClient(host, puerto))
            {
                cliente.EnableSsl = ssl;
                cliente.Timeout = 15000;
                if (usuario.Length > 0)
                {
                    cliente.Credentials = new NetworkCredential(usuario, clave);
                }
                using (MailMessage mensaje = new MailMessage(remitente, para, asunto, cuerpo))
                {
                    cliente.Send(mensaje);
                }
            }
        }
    }
}
