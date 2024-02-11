using CapaLogicaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmBitacora : Form
    {
        private CL_clsBitacora capaBitacora;

        public frmBitacora()
        {
            InitializeComponent();
            capaBitacora = new CL_clsBitacora();
        }

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            // Crear y configurar el Label
            Label lblBitacora = new Label();
            lblBitacora.Size = new Size(300, 100);
            lblBitacora.Text = "Bitacora";
            lblBitacora.TextAlign = ContentAlignment.MiddleCenter;
            lblBitacora.Font = new Font(lblBitacora.Font.FontFamily, 16, FontStyle.Bold);

            int x = (panelBitacora.Width - lblBitacora.Width) / 2;
            int y = (panelBitacora.Height - lblBitacora.Height) / 2;

            lblBitacora.Location = new Point(x, y);

            panelBitacora.Controls.Add(lblBitacora);
        }

        private void btnFacturacion_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarFacturacion());
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarMenu());
        }

        private void btnPersonal_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarPersonal());
        }

        private void btnProducto_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarProducto());
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarProveedores());
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarStock());
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            MostrarDatosEnDataGridView(capaBitacora.MostrarLogin());
        }

        private void MostrarDatosEnDataGridView(DataTable tabla)
        {
            dgvBitacora.DataSource = tabla;
        }
    }
}
