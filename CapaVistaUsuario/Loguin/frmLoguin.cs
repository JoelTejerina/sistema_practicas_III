using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaLogicaNegocio;
using CapaComun;
using System.Runtime.InteropServices;
using System.Collections;
using CapaLogicaNegocio.Loguin;

namespace CapaVistaUsuario

{
    public partial class frmLoguin : Form
    {
        private LinkLabel lnkRecuperar;

        public frmLoguin()
        {
            InitializeComponent();

            // Recuperar la contraseña por mail (CU-SEG-003), debajo del boton Ingresar.
            lnkRecuperar = new LinkLabel
            {
                AutoSize = true,
                Location = new Point(12, 68),
                Text = Idioma.Texto("OlvidoPassword"),
                LinkColor = Color.LightGray,
                ActiveLinkColor = Color.White,
                BackColor = Color.Transparent
            };
            lnkRecuperar.LinkClicked += (s, e) => AbrirRecuperacion();
            panel6.Controls.Add(lnkRecuperar);
        }

        private void AbrirRecuperacion()
        {
            using (Loguin.frmRecuperarPassword f = new Loguin.frmRecuperarPassword(txtUsuario.Text == "USUARIO" ? "" : txtUsuario.Text))
            {
                f.ShowDialog(this);
            }
        }

        // Controles posteriores a validar usuario y contraseña (CU-SEG-001, puntos 5 y siguientes).
        // Devuelve false si el ingreso no puede continuar (ya mostro el motivo).
        private bool ValidarSeguridadIngreso()
        {
            CN_Seguridad seguridad = new CN_Seguridad();
            try
            {
                // Digito verificador: si alguien modifico los usuarios fuera del sistema, se bloquea la terminal.
                if (!seguridad.IntegridadValida())
                {
                    seguridad.BloquearTerminalPorIntegridad();
                    MessageBox.Show(string.Format(Idioma.Texto("Msg_IntegridadInvalida"), CN_Seguridad.MinutosBloqueoTerminal),
                        Idioma.Texto("Msg_SeguridadTitulo"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    UserCache.Limpiar();
                    txtPassword.Clear();
                    return false;
                }

                // Contraseña temporal (recuperacion): hay que cambiarla antes de ingresar.
                if (UserCache.DebeCambiarPassword)
                {
                    using (Loguin.frmCambioPassword cambio = new Loguin.frmCambioPassword { Forzado = true })
                    {
                        cambio.ShowDialog(this);
                    }
                    UserCache.Limpiar();
                    txtPassword.Clear();
                    return false;
                }

                // Una sola sesion por usuario.
                string terminal;
                if (!seguridad.AbrirSesion(UserCache.IdUsuario, out terminal))
                {
                    new CL_clsBitacora("Ingreso al Sistema", "Sesión ya abierta en la terminal " + terminal, "frmLoguin");
                    MessageBox.Show(string.Format(Idioma.Texto("Msg_SesionAbierta"), terminal),
                        Idioma.Texto("Msg_SeguridadTitulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UserCache.Limpiar();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                UserCache.Limpiar();
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
                return false;
            }
        }



        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwmd, int wmsg, int wparam, int lparam);


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //pregunto si desea cerrar, dando la opción de cancelar la operación
            DialogResult resultado = MessageBox.Show("Esta seguro de CERRAR el sistema?",
                                "FINALIZAR SISTEMA",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Asterisk,
                                MessageBoxDefaultButton.Button2);

            if (resultado != DialogResult.Cancel)
            {
                Application.Exit(); //Cierre del formulario
            }
            
        }

        private void frmLoguin_Load(object sender, EventArgs e)
        {
            btnCancelar.Select();
            CN_LlenarCombos llenarCMB = new CN_LlenarCombos(cmbProv, "Provincias", "IdProvincia", "Provincia");
        }

        private void txtUsuario_Enter(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "USUARIO")
            {
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.LightGray;
                txtUsuario.BackColor = Color.FromArgb(51, 51, 51);
                PnlUser.BackColor = Color.FromArgb(51, 51, 51);
            }
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "")
            {
                txtUsuario.Text = "USUARIO";
                txtUsuario.ForeColor = Color.DimGray;
                txtUsuario.BackColor = Color.Black;
                PnlUser.BackColor = Color.Black;
            }

        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "PASSWORD")
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.LightGray;
                txtPassword.UseSystemPasswordChar = true;
                txtPassword.BackColor = Color.FromArgb(51, 51, 51);
                PnlPassword.BackColor = Color.FromArgb(51, 51, 51);
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                txtPassword.Text = "PASSWORD";
                txtPassword.ForeColor = Color.DimGray;
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.BackColor = Color.Black;
                PnlPassword.BackColor = Color.Black;
            }
        }

        private void btnIngresar_MouseEnter(object sender, EventArgs e)
        {
            btnIngresar.ForeColor = Color.Black; ;
            btnIngresar.Font = new Font(btnIngresar.Font, FontStyle.Bold);
        }

        private void btnIngresar_MouseLeave(object sender, EventArgs e)
        {
            btnIngresar.ForeColor = Color.DarkGray;
            btnIngresar.Font = new Font(btnIngresar.Font, FontStyle.Regular);
        }

        private void btnCancelar_MouseEnter(object sender, EventArgs e)
        {
            btnCancelar.ForeColor = Color.Black; ;
            btnCancelar.Font = new Font(btnCancelar.Font, FontStyle.Bold);
        }

        private void btnCancelar_MouseLeave(object sender, EventArgs e)
        {
            btnCancelar.ForeColor = Color.DarkGray;
            btnCancelar.Font = new Font(btnCancelar.Font, FontStyle.Regular);
        }

        private void pctVerPass_Click(object sender, EventArgs e)
        {            
            if (txtPassword.UseSystemPasswordChar == true)
            {
                txtPassword.UseSystemPasswordChar = false;
                pctVerPass.Visible = false;
                pctAsteriscos.Visible = true;
            }
        }

        private void pctAsteriscos_Click(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar == false)
            {
                txtPassword.UseSystemPasswordChar = true;
                pctVerPass.Visible = true;
                pctAsteriscos.Visible = false;
            }
        }
        private void frmLoguin_MouseDown(object sender, MouseEventArgs e)
        {
            MoverForm();
        }
        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            MoverForm();
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            MoverForm();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MoverForm();
        }
        private void MoverForm()
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Terminal bloqueada por una falla del digito verificador: no se puede ingresar hasta que pase el tiempo.
            try
            {
                DateTime? bloqueadaHasta = new CN_Seguridad().TerminalBloqueadaHasta();
                if (bloqueadaHasta.HasValue)
                {
                    MessageBox.Show(string.Format(Idioma.Texto("Msg_TerminalBloqueada"), bloqueadaHasta.Value.ToString("HH:mm")),
                        Idioma.Texto("Msg_SeguridadTitulo"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
                return;
            }

            clsLogicaLoguin BuscarUsuario = new clsLogicaLoguin();
            if (txtUsuario.Text == "" || txtPassword.Text == "" ||
                txtUsuario.Text == "USUARIO" || txtPassword.Text == "PASSWORD")
            {
                MessageBox.Show("Debe completar todos los campos");
            }
            else 
            {
                var resultado = BuscarUsuario.LoginUser(txtUsuario.Text, txtPassword.Text);
                if (resultado.Item1== false)
                {
                    resultado = BuscarUsuario.LoginUser(txtUsuario.Text, null);
                    if (resultado.Item1 == false) {
                        // Si el usuario no exise 
                        UserCache.Apellido = txtUsuario.Text + " (Usuario no encontrado)";
                        CL_clsBitacora Guardar = new CL_clsBitacora("Ingreso al Sistema", "Usuario Inexistente", "frmLoguin");
                        MessageBox.Show("Usuario inexistente");
                        // ----------------------- OK
                    }
                    else
                    { /// existe
                        //Grabo en la BITACORA el ingreso del usuario no existente
                     CL_clsBitacora Guardar = new CL_clsBitacora("Ingreso al Sistema", "Ingreso Erroneo", "frmLoguin");
                        MessageBox.Show(resultado.Item2);
                    }
                    
                }
                else
                {
                    if(UserCache.UsuarioDesactivado == false) { 
                        if (!ValidarSeguridadIngreso())
                        {
                            return;
                        }
                        //Grabo en la BITACORA el ingreso del usuario
                        CL_clsBitacora Guardar = new CL_clsBitacora("Ingreso al Sistema", "Ingreso Exitoso", "frmLoguin");
                        string perm = "";
                        //leo todos los permisos de la HashTable del usuario
                        foreach (DictionaryEntry elementos in UserCache.PermisosUsuario)
                        {
                            perm += elementos.Key  + " " + elementos.Value  + "\n";
                        }

                        //MessageBox.Show("Ingreso Exitoso!!! \n \n" + UserCache.Apellido + " " + UserCache.Nombres +
                        //     "\n CARGO: " + UserCache.Cargo + "\n \n PERMISOS: \n" + perm);
                        

                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        CL_clsBitacora Guardar = new CL_clsBitacora("Ingreso al Sistema", "Ingreso Bloqueado", "frmLoguin");
                        MessageBox.Show("El usuario " + UserCache.Apellido.ToUpper() + " " + UserCache.Nombres.ToUpper() + " se encuentra BLOQUEADO.\n \n " +
                            "Comuniquese con el ADMINISTRADOR del sistema.");
                    }


                }

            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnIngresar.PerformClick();
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
