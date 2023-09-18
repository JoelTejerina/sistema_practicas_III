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
            lblSesion.Text = "Sesion de: " + UserCache.Apellido + " " + UserCache.Nombres; 
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
        /*
         * El evento FormClosing se produce al momento de cerrar el formulario, en este evento
         * que se dispara a través de una instruccion o bien al clickear sobre la X de los ControlBox
         * del formulario, es donde colocamos lo que queremos que suceda al cerrar
         * */
        {
            //pregunto si desea cerrar, dando la opción de cancelar la operación
            DialogResult resultado = MessageBox.Show("Esta seguro de CERRAR el sistema?",
                                "FINALIZAR SISTEMA",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Asterisk,
                                MessageBoxDefaultButton.Button2);

            if (resultado == DialogResult.Cancel)
            {
                e.Cancel = true; //Cancela el cierre del formulario
            }
        }

        private void gestiónDePersonalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonas fAux = new frmPersonas();
            //Loguin.frmCambioPassword fAux = new Loguin.frmCambioPassword();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void ubicacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Administrador.frmUbicaciones fAux = new Administrador.frmUbicaciones();
            fAux.MdiParent = this;
            fAux.Show();
            //Hmmm que delicia
        }

        private void gestionDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUsuario fAux = new frmUsuario();
            fAux.MdiParent = this;
            fAux.Show();
        }
    }
}
