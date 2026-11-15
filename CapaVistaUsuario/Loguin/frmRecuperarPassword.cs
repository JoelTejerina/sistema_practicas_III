using System;
using System.Drawing;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio.Loguin;

namespace CapaVistaUsuario.Loguin
{
    /// <summary>
    /// Recuperar la contraseña (CU-SEG-003): se pide el usuario y se le envia por mail una contraseña
    /// temporal que tendra que cambiar al ingresar. No se indica si el usuario existe o no.
    /// </summary>
    public class frmRecuperarPassword : Form
    {
        private readonly CN_RecuperarPassword recuperacion = new CN_RecuperarPassword();
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();
        private readonly Label lblInfo = new Label();
        private readonly Label lblUsuario = new Label();
        private readonly TextBox txtUsuario = new TextBox();
        private readonly Button btnEnviar = new Button();
        private readonly Button btnCerrar = new Button();

        public frmRecuperarPassword(string usuario)
        {
            Name = "frmRecuperarPassword";
            Text = Idioma.Texto("RecuperarPassword_Titulo");
            ClientSize = new Size(420, 190);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblInfo.Location = new Point(14, 14);
            lblInfo.Size = new Size(392, 54);
            lblInfo.Text = Idioma.Texto("RecuperarPassword_Info");

            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(14, 84);
            lblUsuario.Text = Idioma.Texto("UsuarioLogin");
            txtUsuario.Location = new Point(90, 80);
            txtUsuario.Size = new Size(316, 20);
            txtUsuario.Text = usuario ?? "";

            btnEnviar.Location = new Point(14, 130);
            btnEnviar.Size = new Size(190, 32);
            btnEnviar.Text = Idioma.Texto("Btn_EnviarClave");
            btnEnviar.Click += btnEnviar_Click;
            btnCerrar.Location = new Point(316, 130);
            btnCerrar.Size = new Size(90, 32);
            btnCerrar.Text = Idioma.Texto("Btn_Cerrar");
            btnCerrar.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblInfo, lblUsuario, txtUsuario, btnEnviar, btnCerrar });
            AcceptButton = btnEnviar;
            CancelButton = btnCerrar;
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (!obligatorios.Validar(txtUsuario))
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                recuperacion.Recuperar(txtUsuario.Text);
                MessageBox.Show(Idioma.Texto("Msg_RecuperacionEnviada"), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // No se pudo enviar el mail (servidor caido, datos de SMTP incorrectos...).
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name, Idioma.Texto("Msg_RecuperacionError"));
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
