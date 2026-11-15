using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaVistaUsuario
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CV_ExcepcionBitacora.ConfigurarManejadoresGlobales();
           // Application.Run(new CapaVistaUsuario.frmLoguin());

            // Ingreso -> sistema. Si el usuario elige "Cerrar sesion" se vuelve a la pantalla de ingreso.
            while (true)
            {
                CapaVistaUsuario.frmLoguin frm = new CapaVistaUsuario.frmLoguin();
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK)
                {
                    break;
                }

                frmPrincipal principal = new frmPrincipal();
                Application.Run(principal);
                if (!principal.CerrarSesionSolicitada)
                {
                    break;
                }
                CapaComun.UserCache.Limpiar();
            }

        }
    }
}
