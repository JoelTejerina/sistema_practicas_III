using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaComun;
using CapaVistaUsuario.Administrador;

namespace CapaVistaUsuario
{
    class CV_Utiles
    {
        public static void BloquearControles(Form FRM)
        {
            foreach (Control c in FRM.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Enabled = false;
                }
                if (c is ComboBox)
                {
                    ((ComboBox)c).Enabled = false;
                }
                if (c is GroupBox | c is Panel)
                {
                    BC2(c);
                }
            }
        }

        private static void BC2(Control x)
        {
            foreach (Control h in x.Controls)
            {
                if (h is TextBox)
                {
                    ((TextBox)h).Enabled = false;
                }
                if (h is ComboBox)
                {
                    ((ComboBox)h).Enabled = false;
                }
            }
        }


        public static void DesbloquearControles(Form FRM)
        {
            foreach (Control c in FRM.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Enabled = true;
                }
                if (c is ComboBox)
                {
                    ((ComboBox)c).Enabled = true;
                }
                if (c is GroupBox | c is Panel)
                {
                    DC2(c);
                }
            }
        }

        private static void DC2(Control x)
        {
            foreach (Control h in x.Controls)
            {
                if (h is TextBox)
                {
                    ((TextBox)h).Enabled = true;
                }
                if (h is ComboBox)
                {
                    ((ComboBox)h).Enabled = true;
                }
            }
        }

        public static void LimpiarControles(Form FRM)
        {
            foreach (Control c in FRM.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Text = null;
                }
                if (c is ComboBox)
                {
                    ((ComboBox)c).Text = null;
                }
                if (c is GroupBox | c is Panel)
                {
                    LC2(c);
                }
            }
        }

        private static void LC2(Control x)
        {
            foreach (Control h in x.Controls)
            {
                if (h is TextBox)
                {
                    ((TextBox)h).Text = null;
                }
                if (h is ComboBox)
                {
                    ((ComboBox)h).Text = null;
                }
            }
        }

        public static void InicializarLabelPanel(string texto, Panel nombrePanel)
        {
            // Crear y configurar el Label
            Label lblBitacora = new Label();
            lblBitacora.Size = new Size(300, 100);
            lblBitacora.Text = texto;
            lblBitacora.TextAlign = ContentAlignment.MiddleCenter;
            lblBitacora.Font = new Font(lblBitacora.Font.FontFamily, 16, FontStyle.Bold);
            lblBitacora.ForeColor = Color.White;

            int x = (nombrePanel.Width - lblBitacora.Width) / 2;
            int y = (nombrePanel.Height - lblBitacora.Height) / 2;

            lblBitacora.Location = new Point(x, y);

            nombrePanel.Controls.Add(lblBitacora);
        }

    }
}
