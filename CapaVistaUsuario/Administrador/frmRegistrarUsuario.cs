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
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();
        private int idPersonaExistente;

        private readonly Label lblNacimiento = new Label();
        private DateTimePicker dtpNacimiento;
        private readonly Label lblCalle = new Label();
        private readonly TextBox txtCalle = new TextBox();
        private readonly Label lblNroCalle = new Label();
        private readonly TextBox txtNroCalle = new TextBox();

        public frmRegistrarUsuario()
        {
            InitializeComponent();

            // Fecha de nacimiento (la edad es dato obligatorio del alta): una fila mas en datos personales.
            grpPersonal.Height += 60;
            grpAcceso.Top += 60;
            btnRegistrar.Top += 60;
            btnLimpiar.Top += 60;
            btnSalir.Top += 60;
            ClientSize = new System.Drawing.Size(ClientSize.Width, ClientSize.Height + 60);
            lblNacimiento.Left = 15;
            dtpNacimiento = CV_Edad.CrearSelector(grpPersonal, lblNacimiento, new System.Drawing.Point(120, 194), 200);

            // Direccion (calle y numero): dato obligatorio del alta segun el documento de alcance.
            lblCalle.Name = "lblCalle"; txtCalle.Name = "txtCalle"; lblNroCalle.Name = "lblNroCalle"; txtNroCalle.Name = "txtNroCalle";
            lblCalle.AutoSize = true;
            lblCalle.Location = new System.Drawing.Point(15, 228);
            txtCalle.Location = new System.Drawing.Point(120, 224);
            txtCalle.Size = new System.Drawing.Size(300, 20);
            lblNroCalle.AutoSize = true;
            lblNroCalle.Location = new System.Drawing.Point(435, 228);
            txtNroCalle.Location = new System.Drawing.Point(470, 224);
            txtNroCalle.Size = new System.Drawing.Size(70, 20);
            grpPersonal.Controls.AddRange(new Control[] { lblCalle, txtCalle, lblNroCalle, txtNroCalle });
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
            // El cargo es obligatorio: arranca vacio para que se elija a proposito.
            cmbCargo.SelectedIndex = -1;
            txtNroDoc.Text = "0";
            CV_Documento.Configurar(cmbTipoDoc, txtNroDoc);
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
            lblNacimiento.Text = Idioma.Texto("FechaNacimiento");
            lblCalle.Text = Idioma.Texto("Calle");
            lblNroCalle.Text = Idioma.Texto("NroCalle");
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

            // El alta se confirma antes de crear el usuario (CU-USU-001).
            if (MessageBox.Show(string.Format(Idioma.Texto("Msg_ConfirmAltaUsuario"), txtUsuario.Text.Trim(), cmbGrupo.Text),
                Idioma.Texto("RegistrarUsuario_Titulo"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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

            // Todo es obligatorio salvo la localidad (si no se elige se usa la 1).
            if (!obligatorios.Validar(txtApellido, txtNombres, cmbTipoDoc, txtNroDoc, dtpNacimiento, txtTelefono,
                txtCalle, txtNroCalle, txtCorreo, cmbCargo, txtUsuario, txtPassword, txtConfirmar, cmbGrupo))
            {
                return false;
            }

            if (!obligatorios.ValidarEdad(dtpNacimiento))
            {
                return false;
            }

            int numeroCalle;
            if (!int.TryParse(txtNroCalle.Text.Trim(), out numeroCalle) || numeroCalle < 0)
            {
                obligatorios.Marcar(txtNroCalle);
                MessageBox.Show(Idioma.Texto("Msg_NroCalleInvalido"));
                txtNroCalle.Select();
                return false;
            }

            if (!CV_Documento.Validar(obligatorios, cmbTipoDoc, txtNroDoc))
            {
                return false;
            }
            int nroDoc = Convert.ToInt32(txtNroDoc.Text.Trim());

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

            if (!obligatorios.ValidarCorreo(txtCorreo))
            {
                return false;
            }

            if (registro.ExisteUsuario(txtUsuario.Text.Trim()))
            {
                obligatorios.Marcar(txtUsuario);
                MessageBox.Show(Idioma.Texto("Msg_UsuarioExiste"));
                txtUsuario.Select();
                return false;
            }

            if (txtPassword.Text.Length < 3)
            {
                obligatorios.Marcar(txtPassword);
                MessageBox.Show(Idioma.Texto("Msg_PasswordCorta"));
                txtPassword.Select();
                return false;
            }

            if (txtPassword.Text != txtConfirmar.Text)
            {
                obligatorios.Marcar(txtConfirmar);
                MessageBox.Show(Idioma.Texto("Msg_PasswordNoCoincide"));
                txtConfirmar.Select();
                return false;
            }

            return true;
        }

        private void PasarDatos()
        {
            registro.Apellido = txtApellido.Text.Trim();
            registro.Nombres = txtNombres.Text.Trim();
            registro.IdTipoDoc = Convert.ToInt32(cmbTipoDoc.SelectedValue);
            registro.FechaNacimiento = dtpNacimiento.Value.Date;
            registro.Calle = txtCalle.Text.Trim();
            registro.NroCalle = Convert.ToInt32(txtNroCalle.Text.Trim());
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
            txtCalle.Clear();
            txtNroCalle.Clear();
            txtUsuario.Clear();
            txtPassword.Clear();
            txtConfirmar.Clear();
            if (cmbTipoDoc.Items.Count > 0) cmbTipoDoc.SelectedIndex = 0;
            if (cmbLocalidad.Items.Count > 0) cmbLocalidad.SelectedIndex = 0;
            cmbCargo.SelectedIndex = -1;
            cmbGrupo.SelectedIndex = -1;
            CV_Edad.Establecer(dtpNacimiento, null);
            obligatorios.LimpiarMarcas();
            idPersonaExistente = 0;
            txtApellido.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
