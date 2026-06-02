using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaVistaUsuario;
using CapaLogicaNegocio.Administrador;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmRegistrarUsuario : Form
    {
        private readonly CN_RegistroUsuario registro = new CN_RegistroUsuario();
        private readonly CV_Validar_Mail validarCorreo = new CV_Validar_Mail();
        private int idPersonaExistente;

        public frmRegistrarUsuario()
        {
            InitializeComponent();
        }

        private void frmRegistrarUsuario_Load(object sender, EventArgs e)
        {
            AplicarIdioma();
            new CN_LlenarCombos(cmbTipoDoc, "TipoDoc", "Id", "Tipo");
            new CN_LlenarCombos(cmbLocalidad, "Localidades", "IdLocalidad", "Localidades");
            new CN_LlenarCombos(cmbGrupo, "Grupos", "IdGrupo", "Grupo");
            new CN_LlenarCombos(cmbCargo, "Cargos", "IdCargo", "Cargo");

            if (cmbTipoDoc.Items.Count > 0) cmbTipoDoc.SelectedIndex = 0;
            if (cmbLocalidad.Items.Count > 0) cmbLocalidad.SelectedIndex = 0;
            if (cmbCargo.Items.Count > 0) cmbCargo.SelectedIndex = 0;
            txtNroDoc.Text = "0";
        }

        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("RegistrarUsuario_Titulo");
            grpPersonal.Text = Idioma.Texto("RegistrarUsuario_Personal");
            grpAcceso.Text = Idioma.Texto("RegistrarUsuario_Acceso");
            lblApellido.Text = Idioma.Texto("Apellido");
            lblNombres.Text = Idioma.Texto("Nombres");
            lblTipoDoc.Text = Idioma.Texto("TipoDoc");
            lblNroDoc.Text = Idioma.Texto("NroDoc");
            lblTelefono.Text = Idioma.Texto("Telefono");
            lblCorreo.Text = Idioma.Texto("Correo");
            lblLocalidad.Text = Idioma.Texto("Localidad");
            lblUsuario.Text = Idioma.Texto("UsuarioLogin");
            lblPassword.Text = Idioma.Texto("Password");
            lblConfirmar.Text = Idioma.Texto("ConfirmarPassword");
            lblGrupo.Text = Idioma.Texto("RolUsuario");
            lblCargo.Text = Idioma.Texto("Cargo");
            btnRegistrar.Text = Idioma.Texto("Btn_Registrar");
            btnLimpiar.Text = Idioma.Texto("Btn_Limpiar");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
            {
                return;
            }

            try
            {
                PasarDatos();
                int idUsuario = registro.Registrar(idPersonaExistente);
                new CL_clsBitacora("Alta de usuario",
                    "Usuario " + txtUsuario.Text.Trim() + " (Id " + idUsuario + "), rol " + cmbGrupo.Text,
                    "frmRegistrarUsuario");
                MessageBox.Show(Idioma.Texto("Msg_UsuarioRegistrado"));
                idPersonaExistente = 0;
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private bool ValidarCampos()
        {
            idPersonaExistente = 0;

            if (string.IsNullOrWhiteSpace(txtApellido.Text) || string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                MessageBox.Show(Idioma.Texto("Msg_CompletarApellidoNombre"));
                return false;
            }

            if (!int.TryParse(txtNroDoc.Text, out int nroDoc) || nroDoc <= 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_NroDocInvalido"));
                return false;
            }

            if (cmbTipoDoc.SelectedValue == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneTipoDoc"));
                return false;
            }

            int idTipoDoc = Convert.ToInt32(cmbTipoDoc.SelectedValue);
            idPersonaExistente = registro.ObtenerIdPersonaPorDocumento(idTipoDoc, nroDoc);
            if (idPersonaExistente > 0)
            {
                if (registro.ExisteUsuarioPorPersona(idPersonaExistente))
                {
                    MessageBox.Show(Idioma.Texto("Msg_PersonaYaTieneUsuario"));
                    return false;
                }
                DialogResult vincular = MessageBox.Show(
                    Idioma.Texto("Msg_PersonaExisteCrearUsuario"),
                    Idioma.Texto("RegistrarUsuario_Titulo"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (vincular != DialogResult.Yes)
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                validarCorreo.Correo = txtCorreo.Text.Trim();
                if (!validarCorreo.Valid())
                {
                    MessageBox.Show(Idioma.Texto("Msg_CorreoInvalido"));
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show(Idioma.Texto("Msg_CompletarUsuario"));
                return false;
            }

            if (registro.ExisteUsuario(txtUsuario.Text.Trim()))
            {
                MessageBox.Show(Idioma.Texto("Msg_UsuarioExiste"));
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Length < 3)
            {
                MessageBox.Show(Idioma.Texto("Msg_PasswordCorta"));
                return false;
            }

            if (txtPassword.Text != txtConfirmar.Text)
            {
                MessageBox.Show(Idioma.Texto("Msg_PasswordNoCoincide"));
                return false;
            }

            if (cmbGrupo.SelectedValue == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRol"));
                return false;
            }

            return true;
        }

        private void PasarDatos()
        {
            registro.Apellido = txtApellido.Text.Trim();
            registro.Nombres = txtNombres.Text.Trim();
            registro.IdTipoDoc = Convert.ToInt32(cmbTipoDoc.SelectedValue);
            registro.NroDoc = Convert.ToInt32(txtNroDoc.Text);
            registro.Telefono = txtTelefono.Text.Trim();
            registro.Correo = txtCorreo.Text.Trim();
            registro.IdLocalidad = cmbLocalidad.SelectedValue != null
                ? Convert.ToInt32(cmbLocalidad.SelectedValue) : 1;
            registro.IdCargo = cmbCargo.SelectedValue != null
                ? Convert.ToInt32(cmbCargo.SelectedValue) : 6;
            registro.Usuario = txtUsuario.Text.Trim();
            registro.Password = txtPassword.Text;
            registro.IdGrupo = Convert.ToInt32(cmbGrupo.SelectedValue);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            txtApellido.Clear();
            txtNombres.Clear();
            txtNroDoc.Text = "0";
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtUsuario.Clear();
            txtPassword.Clear();
            txtConfirmar.Clear();
            if (cmbTipoDoc.Items.Count > 0) cmbTipoDoc.SelectedIndex = 0;
            if (cmbLocalidad.Items.Count > 0) cmbLocalidad.SelectedIndex = 0;
            if (cmbCargo.Items.Count > 0) cmbCargo.SelectedIndex = 0;
            cmbGrupo.SelectedIndex = -1;
            idPersonaExistente = 0;
            txtApellido.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
