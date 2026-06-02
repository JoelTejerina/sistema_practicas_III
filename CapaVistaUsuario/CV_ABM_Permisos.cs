using System.Windows.Forms;
using CapaComun;

namespace CapaVistaUsuario
{
    public static class CV_ABM_Permisos
    {
        public static void AplicarModoLectura(Form formulario, string permisoModulo)
        {
            if (!PermisosSistema.SoloLectura(permisoModulo))
            {
                return;
            }

            foreach (Control c in formulario.Controls)
            {
                if (c is Panel || c is GroupBox)
                {
                    OcultarBotonesEdicion(c);
                }
            }
        }

        private static void OcultarBotonesEdicion(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is Button)
                {
                    string nombre = c.Name;
                    if (nombre == "btnAgregar" || nombre == "btnModificar" ||
                        nombre == "btnGuardar" || nombre == "btnGuardaCambios" ||
                        nombre == "btnEliminar" || nombre == "btnCobrar")
                    {
                        c.Visible = false;
                        c.Enabled = false;
                    }
                }
            }
        }
    }
}
