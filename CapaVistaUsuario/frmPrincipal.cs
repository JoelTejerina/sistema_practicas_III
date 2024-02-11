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
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            lblSesion.Text = "Sesion de: " + UserCache.Usuario;

            // Crear y configurar el Label
            Label lblBitacora = new Label();
            lblBitacora.Size = new Size(300, 100);
            lblBitacora.Text = "Fomulario principal";
            lblBitacora.TextAlign = ContentAlignment.MiddleCenter;
            lblBitacora.Font = new Font(lblBitacora.Font.FontFamily, 16, FontStyle.Bold);

            int x = (panelPrincipal.Width - lblBitacora.Width) / 2;
            int y = (panelPrincipal.Height - lblBitacora.Height) / 2;

            lblBitacora.Location = new Point(x, y);

            panelPrincipal.Controls.Add(lblBitacora);
        }

        private void cambioDePasswordToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Loguin.frmCambioPassword fAux = new Loguin.frmCambioPassword();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult resultado = MessageBox.Show("Esta seguro de CERRAR el sistema?",
                                "FINALIZAR SISTEMA",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Asterisk,
                                MessageBoxDefaultButton.Button2);

            if (resultado == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void proveedoresToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmProveedores fAux = new frmProveedores();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void stockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmStock fAux = new frmStock();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void personalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonal fAux = new frmPersonal();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void facturaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFacturacion fAux = new frmFacturacion();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMenu fAux = new frmMenu();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void productoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProducto fAux = new frmProducto();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBitacora fAux = new frmBitacora();
            fAux.MdiParent = this;
            fAux.Show();
        }
    }
}
