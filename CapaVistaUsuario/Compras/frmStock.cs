using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Compras;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Compras
{
    public partial class frmStock : Form
    {
        CN_Stock Stk = new CN_Stock();

        public frmStock()
        {
            InitializeComponent();
        }

        private void frmStock_Load(object sender, EventArgs e)
        {
            dgvStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStock.ReadOnly = true;
            dgvStock.MultiSelect = false;
            dgvStock.AllowUserToAddRows = false;

            AplicarIdioma();
            CargarCombos();
            MostrarStock();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Stock);
        }

        #region METODOS
        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("Stock_Titulo");
            lblProducto.Text = Idioma.Texto("Producto");
            lblNumeroLote.Text = Idioma.Texto("NumeroLote");
            lblCantidad.Text = Idioma.Texto("Cantidad");
            lblVencimiento.Text = Idioma.Texto("Vencimiento");
            lblPrecio.Text = Idioma.Texto("Precio");
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
            new CN_LlenarCombos(cmbProducto, "Producto", "IdProducto", "Nombre");
        }

        private void MostrarStock()
        {
            dgvStock.DataSource = Stk.MostrarStock();
        }

        private bool DatosValidos()
        {
            if (cmbProducto.SelectedValue == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneProducto"));
                cmbProducto.Select();
                return false;
            }
            int cant;
            if (!int.TryParse(txtCantidad.Text.Trim(), out cant) || cant < 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_CantidadInvalida"));
                txtCantidad.Select();
                return false;
            }
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio) || precio < 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_PrecioInvalido"));
                txtPrecio.Select();
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Stk.IdStock = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Stk.IdProducto = Convert.ToInt32(cmbProducto.SelectedValue);
            Stk.NumeroLote = txtNumeroLote.Text.Trim();
            Stk.Cantidad = Convert.ToInt32(txtCantidad.Text.Trim());
            Stk.FechaVencimiento = dtpVencimiento.Value;
            Stk.Precio = Convert.ToDecimal(txtPrecio.Text.Trim());
        }
        #endregion

        #region BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            cmbProducto.SelectedIndex = -1;
            txtCantidad.Text = "0";
            txtPrecio.Text = "0";
            lblID.Text = "0";
            CV_Botonera.btnFormularios(this, btnAgregar);
            cmbProducto.Select();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                PasarDatos(false);
                Stk.InsertarStock();
                new CL_clsBitacora("Ingreso de Stock", "Lote: " + txtNumeroLote.Text.Trim(), "frmStock");
                MostrarStock();
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
            cmbProducto.Select();
        }

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                PasarDatos(true);
                Stk.ModificarStock();
                new CL_clsBitacora("Modificación de Stock", "Modifica id " + lblID.Text, "frmStock");
                MostrarStock();
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
                Stk.IdStock = Convert.ToInt32(lblID.Text);
                Stk.EliminarStock();
                new CL_clsBitacora("Baja de Stock", "Baja id " + lblID.Text, "frmStock");
                MostrarStock();
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgvStock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvStock.SelectedRows.Count == 0) return;
            var f = dgvStock.SelectedRows[0];
            lblID.Text = f.Cells["IdStock"].Value.ToString();
            if (f.Cells["IdProducto"].Value != DBNull.Value && f.Cells["IdProducto"].Value != null)
            {
                cmbProducto.SelectedValue = Convert.ToInt32(f.Cells["IdProducto"].Value);
            }
            txtNumeroLote.Text = f.Cells["NumeroLote"].Value.ToString();
            txtCantidad.Text = f.Cells["Cantidad"].Value.ToString();
            if (f.Cells["FechaVencimiento"].Value != DBNull.Value && f.Cells["FechaVencimiento"].Value != null)
            {
                dtpVencimiento.Value = Convert.ToDateTime(f.Cells["FechaVencimiento"].Value);
            }
            txtPrecio.Text = f.Cells["Precio"].Value.ToString();
        }
    }
}
