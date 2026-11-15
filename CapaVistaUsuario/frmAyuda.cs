using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapaComun;

namespace CapaVistaUsuario
{
    /// <summary>Ayuda en linea: lista de temas a la izquierda, texto a la derecha y un buscador que filtra los temas.</summary>
    public class frmAyuda : Form
    {
        private readonly TextBox txtBuscar = new TextBox();
        private readonly ListBox lstTemas = new ListBox();
        private readonly RichTextBox rtbTexto = new RichTextBox();
        private readonly Label lblBuscar = new Label();
        private readonly Button btnCerrar = new Button();

        public frmAyuda()
        {
            Name = "frmAyuda";
            Text = Idioma.Texto("Ayuda_Titulo");
            ClientSize = new Size(860, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;

            lblBuscar.Text = Idioma.Texto("Btn_Buscar");
            lblBuscar.Location = new Point(12, 14);
            lblBuscar.AutoSize = true;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Location = new Point(70, 10);
            txtBuscar.Width = 200;
            txtBuscar.TextChanged += (s, e) => CargarTemas();

            lstTemas.Name = "lstTemas";
            lstTemas.Location = new Point(12, 42);
            lstTemas.Size = new Size(258, 430);
            lstTemas.SelectedIndexChanged += (s, e) => MostrarTema();

            rtbTexto.Name = "rtbTexto";
            rtbTexto.Location = new Point(282, 12);
            rtbTexto.Size = new Size(566, 460);
            rtbTexto.ReadOnly = true;
            rtbTexto.BackColor = Color.White;
            rtbTexto.Font = new Font("Segoe UI", 10f);
            rtbTexto.BorderStyle = BorderStyle.FixedSingle;

            btnCerrar.Name = "btnCerrar";
            btnCerrar.Text = Idioma.Texto("Salir");
            btnCerrar.Location = new Point(748, 482);
            btnCerrar.Size = new Size(100, 28);
            btnCerrar.Click += (s, e) => Close();
            CancelButton = btnCerrar;

            Controls.AddRange(new Control[] { lblBuscar, txtBuscar, lstTemas, rtbTexto, btnCerrar });
            CargarTemas();
        }

        private void CargarTemas()
        {
            string filtro = (txtBuscar.Text ?? "").Trim();
            TemaAyuda[] temas = CV_Ayuda.Temas
                .Where(t => filtro.Length == 0
                    || t.Titulo.IndexOf(filtro, StringComparison.CurrentCultureIgnoreCase) >= 0
                    || t.Texto.IndexOf(filtro, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .ToArray();
            lstTemas.BeginUpdate();
            lstTemas.Items.Clear();
            foreach (TemaAyuda t in temas)
            {
                lstTemas.Items.Add(t);
            }
            lstTemas.EndUpdate();
            if (lstTemas.Items.Count > 0)
            {
                lstTemas.SelectedIndex = 0;
            }
            else
            {
                rtbTexto.Clear();
            }
        }

        private void MostrarTema()
        {
            TemaAyuda t = lstTemas.SelectedItem as TemaAyuda;
            if (t == null)
            {
                return;
            }
            rtbTexto.Clear();
            rtbTexto.SelectionFont = new Font("Segoe UI", 13f, FontStyle.Bold);
            rtbTexto.AppendText(t.Titulo + "\r\n\r\n");
            rtbTexto.SelectionFont = new Font("Segoe UI", 10f);
            rtbTexto.AppendText(t.Texto);
            rtbTexto.SelectionStart = 0;
        }
    }
}
