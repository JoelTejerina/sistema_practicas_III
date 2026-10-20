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
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();

        // Campos nuevos del alcance: stock minimo/maximo (alertas) e historial de movimientos.
        private Label lblMinimo;
        private Label lblMaximo;
        private TextBox txtMinimo;
        private TextBox txtMaximo;
        private Button btnHistorial;

        public frmStock()
        {
            InitializeComponent();
            CrearCamposStock();
            BuscadorGrilla.Agregar(dgvStock, "NumeroLote");
        }

        private void CrearCamposStock()
        {
            lblMinimo = new Label { AutoSize = true, Location = new System.Drawing.Point(360, 291) };
            txtMinimo = new TextBox { Name = "txtStockMinimo", Location = new System.Drawing.Point(470, 288), Size = new System.Drawing.Size(70, 20) };
            lblMaximo = new Label { AutoSize = true, Location = new System.Drawing.Point(548, 291) };
            txtMaximo = new TextBox { Name = "txtStockMaximo", Location = new System.Drawing.Point(610, 288), Size = new System.Drawing.Size(70, 20) };
            btnHistorial = new Button { Name = "btnHistorial", Location = new System.Drawing.Point(120, 320), Size = new System.Drawing.Size(150, 26) };
            btnHistorial.Click += btnHistorial_Click;
            Controls.AddRange(new Control[] { lblMinimo, txtMinimo, lblMaximo, txtMaximo, btnHistorial });
        }

        // Muestra el historial de movimientos del producto del lote elegido (o de todos).
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            int idProducto = 0;
            string nombre = "";
            if (Convert.ToInt32(lblID.Text) != 0 && cmbProducto.SelectedValue != null)
            {
                idProducto = Convert.ToInt32(cmbProducto.SelectedValue);
                nombre = cmbProducto.Text;
            }
            using (frmHistorialStock historial = new frmHistorialStock(idProducto, nombre))
            {
                historial.ShowDialog(this);
            }
        }

        // Avisa los lotes cuya existencia llego al minimo configurado.
        private void MostrarAlertaStockBajo()
        {
            System.Data.DataTable bajos = Stk.LotesBajoMinimo();
            if (bajos.Rows.Count == 0) return;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(Idioma.Texto("Stock_AlertaBajo"));
            foreach (System.Data.DataRow fila in bajos.Rows)
            {
                sb.AppendLine("- " + fila["Producto"] + " (" + fila["NumeroLote"] + "): " + fila["Cantidad"] +
                    " / " + Idioma.Texto("StockMinimoCorto") + " " + fila["StockMinimo"]);
            }
            MessageBox.Show(sb.ToString().TrimEnd(), Idioma.Texto("Stock_AlertaTitulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Resalta en la grilla los lotes bajo el minimo (rojo) o por encima del maximo (amarillo).
        private void dgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || !dgvStock.Columns.Contains("StockMinimo")) return;
            DataGridViewRow fila = dgvStock.Rows[e.RowIndex];
            int cant, min, max;
            if (!int.TryParse(Convert.ToString(fila.Cells["Cantidad"].Value), out cant)) return;
            int.TryParse(Convert.ToString(fila.Cells["StockMinimo"].Value), out min);
            int.TryParse(Convert.ToString(fila.Cells["StockMaximo"].Value), out max);
            if (min > 0 && cant <= min)
            {
                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(255, 200, 200);
                e.CellStyle.ForeColor = System.Drawing.Color.Black;
            }
            else if (max > 0 && cant > max)
            {
                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(255, 245, 180);
                e.CellStyle.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void frmStock_Load(object sender, EventArgs e)
        {
            dgvStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStock.ReadOnly = true;
            dgvStock.MultiSelect = false;
            dgvStock.AllowUserToAddRows = false;

            dgvStock.CellFormatting += dgvStock_CellFormatting;

            AplicarIdioma();
            CargarCombos();
            MostrarStock();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Stock);
            MostrarAlertaStockBajo();
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
            lblMinimo.Text = Idioma.Texto("StockMinimo");
            lblMaximo.Text = Idioma.Texto("StockMaximo");
            btnHistorial.Text = Idioma.Texto("Btn_Historial");
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
            new CN_LlenarCombos(cmbProducto, "Producto", "IdProducto", "Nombre", "Activo = 1");
        }

        private void MostrarStock()
        {
            dgvStock.DataSource = Stk.MostrarStock();
        }

        private bool DatosValidos()
        {
            // Todos los campos son obligatorios.
            if (!obligatorios.Validar(cmbProducto, txtNumeroLote, txtCantidad, dtpVencimiento, txtPrecio))
            {
                return false;
            }
            int cant;
            if (!int.TryParse(txtCantidad.Text.Trim(), out cant) || cant < 0)
            {
                obligatorios.Marcar(txtCantidad);
                MessageBox.Show(Idioma.Texto("Msg_CantidadInvalida"));
                txtCantidad.Select();
                return false;
            }
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio) || precio < 0)
            {
                obligatorios.Marcar(txtPrecio);
                MessageBox.Show(Idioma.Texto("Msg_PrecioInvalido"));
                txtPrecio.Select();
                return false;
            }

            // Minimo y maximo son opcionales (vacio o 0 = sin alerta); el maximo no puede ser menor que el minimo.
            int min = 0, max = 0;
            bool minOk = txtMinimo.Text.Trim().Length == 0 || (int.TryParse(txtMinimo.Text.Trim(), out min) && min >= 0);
            bool maxOk = txtMaximo.Text.Trim().Length == 0 || (int.TryParse(txtMaximo.Text.Trim(), out max) && max >= 0);
            if (!minOk || !maxOk || (max > 0 && max < min))
            {
                if (!minOk || (max > 0 && max < min)) obligatorios.Marcar(txtMinimo);
                if (!maxOk || (max > 0 && max < min)) obligatorios.Marcar(txtMaximo);
                MessageBox.Show(Idioma.Texto("Msg_MinMaxInvalido"));
                txtMinimo.Select();
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
            Stk.StockMinimo = txtMinimo.Text.Trim().Length == 0 ? 0 : Convert.ToInt32(txtMinimo.Text.Trim());
            Stk.StockMaximo = txtMaximo.Text.Trim().Length == 0 ? 0 : Convert.ToInt32(txtMaximo.Text.Trim());
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
            txtMinimo.Text = "0";
            txtMaximo.Text = "0";
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
            CargarSeleccion();
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
            if (e.RowIndex < 0) return;
            CargarSeleccion();
        }

        /// <summary>Llena los campos con el registro seleccionado en la grilla.</summary>
        private void CargarSeleccion()
        {
            if (dgvStock.SelectedRows.Count == 0) return;
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
            txtMinimo.Text = f.Cells["StockMinimo"].Value.ToString();
            txtMaximo.Text = f.Cells["StockMaximo"].Value.ToString();
        }
    }
}
