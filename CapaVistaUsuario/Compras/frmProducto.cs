using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Compras;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Compras
{
    public partial class frmProducto : Form
    {
        CN_Producto Prod = new CN_Producto();

        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            dgvProducto.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducto.ReadOnly = true;
            dgvProducto.MultiSelect = false;
            dgvProducto.AllowUserToAddRows = false;

            AplicarIdioma();
            CargarCombos();
            MostrarProductos();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Productos);
        }

        #region METODOS
        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("Prod_Titulo");
            lblNombre.Text = Idioma.Texto("Nombre");
            lblDescripcion.Text = Idioma.Texto("Descripcion");
            lblMarca.Text = Idioma.Texto("Marca");
            lblCategoria.Text = Idioma.Texto("Categoria");
            lblMedida.Text = Idioma.Texto("Medida");
            lblProveedor.Text = Idioma.Texto("Proveedor");
            btnAgregar.Text = Idioma.Texto("Btn_Agregar");
            btnModificar.Text = Idioma.Texto("Btn_Modificar");
            btnGuardar.Text = Idioma.Texto("Btn_Guardar");
            btnGuardaCambios.Text = Idioma.Texto("Btn_GuardarCambios");
            btnEliminar.Text = Idioma.Texto("Btn_Eliminar");
            btnCancelar.Text = Idioma.Texto("Btn_Cancelar");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
        }

        private void CargarCombos()
        {
            new CN_LlenarCombos(cmbProveedor, "Proveedor", "IdProveedor", "Nombre");
        }

        private void MostrarProductos()
        {
            dgvProducto.DataSource = Prod.MostrarProducto();
        }

        private bool DatosValidos()
        {
            if (txtNombre.Text.Trim().Length == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_CampoObligatorio"));
                txtNombre.Select();
                return false;
            }
            if (cmbProveedor.SelectedValue == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneProveedor"));
                cmbProveedor.Select();
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Prod.IdProducto = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Prod.Nombre = txtNombre.Text.Trim();
            Prod.Descripcion = txtDescripcion.Text.Trim();
            Prod.Marca = txtMarca.Text.Trim();
            Prod.Categoria = txtCategoria.Text.Trim();
            Prod.Medida = txtMedida.Text.Trim();
            Prod.IdProveedor = Convert.ToInt32(cmbProveedor.SelectedValue);
        }
        #endregion

        #region BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            cmbProveedor.SelectedIndex = -1;
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
                Prod.InsertarProducto();
                new CL_clsBitacora("Alta de Producto", "Alta: " + txtNombre.Text.Trim(), "frmProducto");
                MostrarProductos();
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
                Prod.ModificarProducto();
                new CL_clsBitacora("Modificación de Producto", "Modifica id " + lblID.Text, "frmProducto");
                MostrarProductos();
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
                Prod.IdProducto = Convert.ToInt32(lblID.Text);
                Prod.EliminarProducto();
                new CL_clsBitacora("Baja de Producto", "Baja id " + lblID.Text, "frmProducto");
                MostrarProductos();
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgvProducto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvProducto.SelectedRows.Count == 0) return;
            var f = dgvProducto.SelectedRows[0];
            lblID.Text = f.Cells["IdProducto"].Value.ToString();
            txtNombre.Text = f.Cells["Nombre"].Value.ToString();
            txtDescripcion.Text = f.Cells["Descripcion"].Value.ToString();
            txtMarca.Text = f.Cells["Marca"].Value.ToString();
            txtCategoria.Text = f.Cells["Categoria"].Value.ToString();
            txtMedida.Text = f.Cells["Medida"].Value.ToString();
            if (f.Cells["IdProveedor"].Value != DBNull.Value && f.Cells["IdProveedor"].Value != null)
            {
                cmbProveedor.SelectedValue = Convert.ToInt32(f.Cells["IdProveedor"].Value);
            }
        }
    }
}
