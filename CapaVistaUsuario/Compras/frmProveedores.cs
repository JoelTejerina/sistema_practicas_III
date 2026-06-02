using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Compras;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Compras
{
    public partial class frmProveedores : Form
    {
        CN_Proveedor Prov = new CN_Proveedor();

        public frmProveedores()
        {
            InitializeComponent();
        }

        private void frmProveedores_Load(object sender, EventArgs e)
        {
            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.ReadOnly = true;
            dgvProveedores.MultiSelect = false;
            dgvProveedores.AllowUserToAddRows = false;

            AplicarIdioma();
            MostrarProveedores();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Proveedores);
        }

        #region METODOS
        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("Prov_Titulo");
            lblNombre.Text = Idioma.Texto("Nombre");
            lblRazonSocial.Text = Idioma.Texto("RazonSocial");
            lblCategoria.Text = Idioma.Texto("Categoria");
            lblCUIT.Text = Idioma.Texto("CUIT");
            lblCertificadoAfip.Text = Idioma.Texto("CertificadoAfip");
            lblDireccion.Text = Idioma.Texto("Direccion");
            lblEmail.Text = Idioma.Texto("Email");
            lblTelefono.Text = Idioma.Texto("Telefono");
            btnAgregar.Text = Idioma.Texto("Btn_Agregar");
            btnModificar.Text = Idioma.Texto("Btn_Modificar");
            btnGuardar.Text = Idioma.Texto("Btn_Guardar");
            btnGuardaCambios.Text = Idioma.Texto("Btn_GuardarCambios");
            btnEliminar.Text = Idioma.Texto("Btn_Eliminar");
            btnCancelar.Text = Idioma.Texto("Btn_Cancelar");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
        }

        private void MostrarProveedores()
        {
            dgvProveedores.DataSource = Prov.MostrarProveedor();
        }

        private bool DatosValidos()
        {
            if (txtNombre.Text.Trim().Length == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_CampoObligatorio"));
                txtNombre.Select();
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Prov.IdProveedor = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Prov.Nombre = txtNombre.Text.Trim();
            Prov.RazonSocial = txtRazonSocial.Text.Trim();
            Prov.Categoria = txtCategoria.Text.Trim();
            Prov.CUIT = txtCUIT.Text.Trim();
            Prov.CertificadoAfip = txtCertificadoAfip.Text.Trim();
            Prov.Direccion = txtDireccion.Text.Trim();
            Prov.Email = txtEmail.Text.Trim();
            Prov.Telefono = txtTelefono.Text.Trim();
        }
        #endregion

        #region BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            lblID.Text = "0";
            CV_Botonera.btnFormularios(this, btnAgregar);
            txtNombre.Select();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                PasarDatos(false);
                Prov.InsertarProveedor();
                new CL_clsBitacora("Alta de Proveedor", "Alta: " + txtNombre.Text.Trim(), "frmProveedores");
                MostrarProveedores();
                CV_Botonera.btnFormularios(this, btnGuardar);
                CV_Utiles.BloquearControles(this);
                CV_Utiles.LimpiarControles(this);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            CV_Utiles.DesbloquearControles(this);
            CV_Botonera.btnFormularios(this, btnModificar);
            txtNombre.Select();
        }

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                PasarDatos(true);
                Prov.ModificarProveedor();
                new CL_clsBitacora("Modificación de Proveedor", "Modifica id " + lblID.Text, "frmProveedores");
                MostrarProveedores();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            lblID.Text = "0";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            DialogResult r = MessageBox.Show(Idioma.Texto("Msg_ConfirmEliminar"),
                Idioma.Texto("Btn_Eliminar"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (r == DialogResult.OK)
            {
                Prov.IdProveedor = Convert.ToInt32(lblID.Text);
                Prov.EliminarProveedor();
                new CL_clsBitacora("Baja de Proveedor", "Baja id " + lblID.Text, "frmProveedores");
                MostrarProveedores();
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvProveedores.SelectedRows.Count == 0) return;
            var f = dgvProveedores.SelectedRows[0];
            lblID.Text = f.Cells["IdProveedor"].Value.ToString();
            txtNombre.Text = f.Cells["Nombre"].Value.ToString();
            txtRazonSocial.Text = f.Cells["RazonSocial"].Value.ToString();
            txtCategoria.Text = f.Cells["Categoria"].Value.ToString();
            txtCUIT.Text = f.Cells["CUIT"].Value.ToString();
            txtCertificadoAfip.Text = f.Cells["CertificadoAfip"].Value.ToString();
            txtDireccion.Text = f.Cells["Direccion"].Value.ToString();
            txtEmail.Text = f.Cells["Email"].Value.ToString();
            txtTelefono.Text = f.Cells["Telefono"].Value.ToString();
        }
    }
}
