using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;

namespace CapaVistaUsuario
{
    /// <summary>
    /// Registra excepciones en la bitácora y opcionalmente las muestra al usuario.
    /// </summary>
    public static class CV_ExcepcionBitacora
    {
        public static void ConfigurarManejadoresGlobales()
        {
            Application.ThreadException += (sender, e) =>
            {
                Registrar(e.Exception, "Application.ThreadException");
                MostrarError(e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    Registrar(ex, "AppDomain.UnhandledException");
                }
            };
        }

        public static void Registrar(Exception ex, string origen)
        {
            if (ex == null)
            {
                return;
            }

            try
            {
                string detalle = ex.Message;
                if (ex.InnerException != null)
                {
                    detalle += " | " + ex.InnerException.Message;
                }
                if (detalle.Length > 450)
                {
                    detalle = detalle.Substring(0, 450) + "...";
                }
                new CL_clsBitacora("Excepción", detalle, origen ?? "Sistema");
            }
            catch
            {
                // No interrumpir el flujo si la bitácora falla
            }
        }

        public static void RegistrarYMostrar(Exception ex, string origen, string titulo = null)
        {
            Registrar(ex, origen);
            MostrarError(ex, titulo);
        }

        private static void MostrarError(Exception ex, string titulo = null)
        {
            string mensaje = Idioma.Texto("Msg_GuardadoError") + "\n" + ex.Message;
            MessageBox.Show(mensaje, titulo ?? Idioma.Texto("Msg_ErrorTitulo"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
