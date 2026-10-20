using System.Windows.Forms;

namespace CapaVistaUsuario
{
    class CV_Utiles
    {
        public static void BloquearControles(Form FRM)
        {
            Recorrer(FRM.Controls, Bloquear);
        }

        public static void DesbloquearControles(Form FRM)
        {
            Recorrer(FRM.Controls, Desbloquear);
        }

        public static void LimpiarControles(Form FRM)
        {
            Recorrer(FRM.Controls, Limpiar);
        }

        // Aplica la accion a los controles del formulario y a los de sus GroupBox / Panel.
        private static void Recorrer(Control.ControlCollection controles, System.Action<Control> accion)
        {
            foreach (Control c in controles)
            {
                accion(c);
                if (c is GroupBox | c is Panel)
                {
                    foreach (Control h in c.Controls)
                    {
                        accion(h);
                    }
                }
            }
        }

        private static void Bloquear(Control c)
        {
            if (c is TextBox || c is ComboBox || c is DateTimePicker)
            {
                c.Enabled = false;
            }
        }

        private static void Desbloquear(Control c)
        {
            if (c is TextBox || c is ComboBox || c is DateTimePicker)
            {
                c.Enabled = true;
            }
        }

        private static void Limpiar(Control c)
        {
            if (c is TextBox)
            {
                ((TextBox)c).Text = null;
            }
            if (c is ComboBox)
            {
                ((ComboBox)c).Text = null;
            }

            // Las fechas obligatorias con casilla (fecha de nacimiento) vuelven a quedar sin elegir.
            DateTimePicker fecha = c as DateTimePicker;
            if (fecha != null && fecha.ShowCheckBox)
            {
                fecha.Checked = false;
            }
        }
    }
}
