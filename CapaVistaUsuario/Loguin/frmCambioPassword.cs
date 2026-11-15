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
using CapaLogicaNegocio;
using CapaLogicaNegocio.Loguin;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Loguin
{
    public partial class frmCambioPassword : Form
    {
        /// <summary>true cuando se abre desde el ingreso por una contraseña temporal: el cambio es obligatorio.</summary>
        public bool Forzado { get; set; }

        public frmCambioPassword()
        {
            InitializeComponent();
        }
        private void frmCambioPassword_Load(object sender, EventArgs e)
        {
            txtPassActual.UseSystemPasswordChar = true;
            txtPassNueva.UseSystemPasswordChar = true;
            txtPassNueva1.UseSystemPasswordChar = true;

            lblUsuario.Text = UserCache.Usuario;

            if (Forzado)
            {
                Text = Idioma.Texto("CambioObligatorio");
                Shown += (origen, args) => MessageBox.Show(Idioma.Texto("Msg_CambioObligatorio"), Text);
            }

        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            CL_clsCambioPass CP = new CL_clsCambioPass();
            try
            {
                CP.CambioPass(UserCache.Usuario, txtPassActual.Text, txtPassNueva.Text, txtPassNueva1.Text);
                if (Forzado)
                {
                    MessageBox.Show(Idioma.Texto("Msg_CambioObligatorioOk"), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                MessageBox.Show("Cambio de contraseña exitoso!! \n" +
                    "EL SISTEMA SE CERRARA, VUELVA A INGRESAR CON LA NUEVA CONTRASEÑA. \n \n" +
                    "MAntenga la nueva contraseña en privado \n" +
                    "y no se la entregue a nadie.", "CAMBIO EXITOSO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                Application.Exit();

            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name, "ERROR EN CAMBIO");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
