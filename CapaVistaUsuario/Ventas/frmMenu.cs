using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Ventas
{
    public partial class frmMenu : Form
    {
        CN_Menu Men = new CN_Menu();

        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenu.ReadOnly = true;
            dgvMenu.MultiSelect = false;
            dgvMenu.AllowUserToAddRows = false;

            AplicarIdioma();
            CargarCombos();
            MostrarMenu();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Menu);
        }

        #region METODOS
        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("Menu_Titulo");
            lblNombre.Text = Idioma.Texto("Nombre");
            lblDescripcion.Text = Idioma.Texto("Descripcion");
            lblIngredientes.Text = Idioma.Texto("Ingredientes");
            lblPopularidad.Text = Idioma.Texto("Popularidad");
            lblCategoria.Text = Idioma.Texto("Categoria");
            lblRegion.Text = Idioma.Texto("Region");
            lblTemporada.Text = Idioma.Texto("Temporada");
            lblTipoEvento.Text = Idioma.Texto("TipoEvento");
            lblPrecio.Text = Idioma.Texto("Precio");
            lblTiempo.Text = Idioma.Texto("Tiempo");
            lblStock.Text = Idioma.Texto("Stock");
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
            new CN_LlenarCombos(cmbStock, "Stock", "IdStock", "NumeroLote");
        }

        private void MostrarMenu()
        {
            dgvMenu.DataSource = Men.MostrarMenu();
        }

        private bool DatosValidos()
        {
            if (txtNombre.Text.Trim().Length == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_CampoObligatorio"));
                txtNombre.Select();
                return false;
            }
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio) || precio < 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_PrecioInvalido"));
                txtPrecio.Select();
                return false;
            }
            int tiempo;
            if (!int.TryParse(txtTiempo.Text.Trim(), out tiempo) || tiempo < 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_TiempoInvalido"));
                txtTiempo.Select();
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Men.IdMenu = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Men.Nombre = txtNombre.Text.Trim();
            Men.Descripcion = txtDescripcion.Text.Trim();
            Men.Categoria = txtCategoria.Text.Trim();
            Men.Ingredientes = txtIngredientes.Text.Trim();
            Men.Precio = Convert.ToDecimal(txtPrecio.Text.Trim());
            Men.Region = txtRegion.Text.Trim();
            Men.Temporada = txtTemporada.Text.Trim();
            Men.Popularidad = txtPopularidad.Text.Trim();
            Men.TiempoPreparacion = Convert.ToInt32(txtTiempo.Text.Trim());
            Men.TipoEvento = txtTipoEvento.Text.Trim();
            Men.IdStock = cmbStock.SelectedValue == null ? 0 : Convert.ToInt32(cmbStock.SelectedValue);
        }
        #endregion

        #region BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            cmbStock.SelectedIndex = -1;
            txtPrecio.Text = "0";
            txtTiempo.Text = "0";
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
                Men.InsertarMenu();
                new CL_clsBitacora("Alta de Menú", "Alta: " + txtNombre.Text.Trim(), "frmMenu");
                MostrarMenu();
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
                Men.ModificarMenu();
                new CL_clsBitacora("Modificación de Menú", "Modifica id " + lblID.Text, "frmMenu");
                MostrarMenu();
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
                Men.IdMenu = Convert.ToInt32(lblID.Text);
                Men.EliminarMenu();
                new CL_clsBitacora("Baja de Menú", "Baja id " + lblID.Text, "frmMenu");
                MostrarMenu();
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgvMenu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvMenu.SelectedRows.Count == 0) return;
            var f = dgvMenu.SelectedRows[0];
            lblID.Text = f.Cells["IdMenu"].Value.ToString();
            txtNombre.Text = f.Cells["Nombre"].Value.ToString();
            txtDescripcion.Text = f.Cells["Descripcion"].Value.ToString();
            txtCategoria.Text = f.Cells["Categoria"].Value.ToString();
            txtIngredientes.Text = f.Cells["Ingredientes"].Value.ToString();
            txtPrecio.Text = f.Cells["Precio"].Value.ToString();
            txtRegion.Text = f.Cells["Region"].Value.ToString();
            txtTemporada.Text = f.Cells["Temporada"].Value.ToString();
            txtPopularidad.Text = f.Cells["Popularidad"].Value.ToString();
            txtTiempo.Text = f.Cells["TiempoPreparacion"].Value.ToString();
            txtTipoEvento.Text = f.Cells["TipoEvento"].Value.ToString();
            if (f.Cells["IdStock"].Value != DBNull.Value && f.Cells["IdStock"].Value != null)
            {
                cmbStock.SelectedValue = Convert.ToInt32(f.Cells["IdStock"].Value);
            }
        }
    }
}
