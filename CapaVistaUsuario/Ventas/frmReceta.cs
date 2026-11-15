using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;

namespace CapaVistaUsuario.Ventas
{
    /// <summary>
    /// Receta de un platillo (CU-007-002): ingredientes principales y secundarios con la cantidad
    /// que lleva cada unidad. Con esto el sistema sabe si el stock alcanza para un pedido.
    /// </summary>
    public class frmReceta : Form
    {
        private readonly int idMenu;
        private readonly string platillo;
        private readonly bool soloLectura;
        private readonly CN_Receta receta = new CN_Receta();
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();

        private readonly Label lblPlatillo = new Label();
        private readonly Label lblCapacidad = new Label();
        private readonly DataGridView dgvReceta = new DataGridView();
        private readonly Label lblIngrediente = new Label();
        private readonly ComboBox cmbIngrediente = new ComboBox();
        private readonly Label lblCantidad = new Label();
        private readonly TextBox txtCantidad = new TextBox();
        private readonly CheckBox chkPrincipal = new CheckBox();
        private readonly Button btnGuardar = new Button();
        private readonly Button btnQuitar = new Button();
        private readonly Button btnFinalizar = new Button();
        private readonly Button btnCerrar = new Button();

        public frmReceta(int idMenu, string platillo, bool soloLectura)
        {
            this.idMenu = idMenu;
            this.platillo = platillo;
            this.soloLectura = soloLectura;
            Name = "frmReceta";
            ConstruirPantalla();
            Load += (s, e) => Cargar();
        }

        private void ConstruirPantalla()
        {
            Text = Idioma.Texto("Receta_Titulo");
            ClientSize = new Size(660, 470);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            lblPlatillo.AutoSize = true;
            lblPlatillo.Font = new Font(Font.FontFamily, 12f, FontStyle.Bold);
            lblPlatillo.Location = new Point(12, 12);
            lblPlatillo.Text = platillo;

            lblCapacidad.AutoSize = false;
            lblCapacidad.Location = new Point(12, 40);
            lblCapacidad.Size = new Size(636, 20);

            dgvReceta.Location = new Point(12, 68);
            dgvReceta.Size = new Size(636, 240);
            dgvReceta.ReadOnly = true;
            dgvReceta.AllowUserToAddRows = false;
            dgvReceta.AllowUserToDeleteRows = false;
            dgvReceta.MultiSelect = false;
            dgvReceta.RowHeadersVisible = false;
            dgvReceta.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReceta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReceta.CellClick += (s, e) => { if (e.RowIndex >= 0) CargarIngredienteSeleccionado(); };

            lblIngrediente.AutoSize = true;
            lblIngrediente.Location = new Point(12, 326);
            cmbIngrediente.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIngrediente.Location = new Point(110, 322);
            cmbIngrediente.Size = new Size(250, 21);

            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(380, 326);
            txtCantidad.Location = new Point(450, 322);
            txtCantidad.Size = new Size(80, 20);

            chkPrincipal.AutoSize = true;
            chkPrincipal.Location = new Point(110, 356);

            btnGuardar.Location = new Point(12, 400);
            btnGuardar.Size = new Size(140, 30);
            btnGuardar.Click += btnGuardar_Click;
            btnQuitar.Location = new Point(160, 400);
            btnQuitar.Size = new Size(140, 30);
            btnQuitar.Click += btnQuitar_Click;
            btnFinalizar.Name = "btnFinalizarReceta";
            btnFinalizar.Location = new Point(308, 400);
            btnFinalizar.Size = new Size(140, 30);
            btnFinalizar.Click += btnFinalizar_Click;
            btnCerrar.Location = new Point(528, 400);
            btnCerrar.Size = new Size(120, 30);
            btnCerrar.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblPlatillo, lblCapacidad, dgvReceta, lblIngrediente, cmbIngrediente, lblCantidad,
                txtCantidad, chkPrincipal, btnGuardar, btnQuitar, btnFinalizar, btnCerrar
            });

            AplicarIdioma();

            if (soloLectura)
            {
                lblIngrediente.Visible = cmbIngrediente.Visible = lblCantidad.Visible = txtCantidad.Visible = false;
                chkPrincipal.Visible = btnGuardar.Visible = btnQuitar.Visible = btnFinalizar.Visible = false;
            }
        }

        private void AplicarIdioma()
        {
            lblIngrediente.Text = Idioma.Texto("Ingrediente");
            lblCantidad.Text = Idioma.Texto("Cantidad");
            chkPrincipal.Text = Idioma.Texto("Receta_Principal");
            btnGuardar.Text = Idioma.Texto("Btn_AgregarIngrediente");
            btnQuitar.Text = Idioma.Texto("Btn_QuitarIngrediente");
            btnFinalizar.Text = Idioma.Texto("Btn_FinalizarReceta");
            btnCerrar.Text = Idioma.Texto("Btn_Cerrar");
        }

        // Finalizar la receta (CU-006): exige al menos un ingrediente y uno principal; muestra el resumen y confirma.
        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            DataTable tabla = receta.Listar(idMenu);
            if (tabla.Rows.Count == 0)
            {
                MessageBox.Show(Idioma.Texto("Receta_FinalizarSinIngredientes"), Idioma.Texto("Receta_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (tabla.Select("Tipo = 'Principal'").Length == 0)
            {
                MessageBox.Show(Idioma.Texto("Receta_FinalizarSinPrincipal"), Idioma.Texto("Receta_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StringBuilder resumen = new StringBuilder();
            foreach (DataRow fila in tabla.Rows)
            {
                resumen.AppendLine("- " + fila["Ingrediente"] + ": " + Convert.ToDecimal(fila["Cantidad"]).ToString("0.###") + " " + fila["Medida"]
                    + (Convert.ToString(fila["Tipo"]) == "Principal" ? " *" : ""));
            }
            if (MessageBox.Show(string.Format(Idioma.Texto("Receta_ConfirmFinalizar"), platillo, tabla.Rows.Count, resumen),
                Idioma.Texto("Receta_Titulo"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            try
            {
                new CL_clsBitacora("Receta finalizada", platillo + ": " + tabla.Rows.Count + " ingrediente(s)", "frmReceta");
                MessageBox.Show(Idioma.Texto("Receta_FinalizadaOk"), Idioma.Texto("Receta_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void Cargar()
        {
            try
            {
                DataTable productos = receta.ProductosDisponibles();
                productos.Columns.Add("Etiqueta", typeof(string),
                    "IIF(Medida = '', Nombre, Nombre + ' (' + Medida + ')')");
                cmbIngrediente.DataSource = productos;
                cmbIngrediente.DisplayMember = "Etiqueta";
                cmbIngrediente.ValueMember = "IdProducto";
                cmbIngrediente.SelectedIndex = -1;
                MostrarReceta();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void MostrarReceta()
        {
            DataTable tabla = receta.Listar(idMenu);
            dgvReceta.DataSource = tabla;
            if (dgvReceta.Columns.Count > 0)
            {
                dgvReceta.Columns["IdProducto"].Visible = false;
                dgvReceta.Columns["Ingrediente"].HeaderText = Idioma.Texto("Ingrediente");
                dgvReceta.Columns["Medida"].HeaderText = Idioma.Texto("Medida");
                dgvReceta.Columns["Cantidad"].HeaderText = Idioma.Texto("Cantidad");
                dgvReceta.Columns["Tipo"].HeaderText = Idioma.Texto("Tipo");
                dgvReceta.Columns["StockDisponible"].HeaderText = Idioma.Texto("StockDisponible");
            }

            // Cuantas unidades del platillo se pueden preparar con el stock actual.
            lblCapacidad.Text = Idioma.Texto("Receta_SinReceta");
            if (tabla.Rows.Count > 0)
            {
                foreach (DataRow fila in receta.Capacidad().Rows)
                {
                    if (Convert.ToInt32(fila["IdMenu"]) == idMenu && fila["Capacidad"] != DBNull.Value)
                    {
                        lblCapacidad.Text = string.Format(Idioma.Texto("Receta_Capacidad"), fila["Capacidad"]);
                    }
                }
            }
        }

        private void CargarIngredienteSeleccionado()
        {
            if (dgvReceta.CurrentRow == null) return;
            DataGridViewRow f = dgvReceta.CurrentRow;
            cmbIngrediente.SelectedValue = Convert.ToInt32(f.Cells["IdProducto"].Value);
            txtCantidad.Text = Convert.ToDecimal(f.Cells["Cantidad"].Value).ToString("0.###");
            chkPrincipal.Checked = Convert.ToString(f.Cells["Tipo"].Value) == "Principal";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!obligatorios.Validar(cmbIngrediente, txtCantidad))
            {
                return;
            }

            decimal cantidad;
            if (!decimal.TryParse(txtCantidad.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out cantidad) || cantidad <= 0)
            {
                obligatorios.Marcar(txtCantidad);
                MessageBox.Show(Idioma.Texto("Msg_CantidadInvalida"));
                txtCantidad.Select();
                return;
            }

            try
            {
                receta.Guardar(idMenu, Convert.ToInt32(cmbIngrediente.SelectedValue), cantidad, chkPrincipal.Checked);
                new CL_clsBitacora("Modificación de Receta", platillo + ": " + cmbIngrediente.Text, "frmReceta");
                cmbIngrediente.SelectedIndex = -1;
                txtCantidad.Clear();
                chkPrincipal.Checked = false;
                MostrarReceta();
            }
            catch (InvalidOperationException ex)
            {
                // Reglas de negocio (cantidad invalida, ingrediente sin medida).
                obligatorios.Marcar(cmbIngrediente);
                MessageBox.Show(ex.Message, Idioma.Texto("Receta_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvReceta.CurrentRow == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }

            try
            {
                int idProducto = Convert.ToInt32(dgvReceta.CurrentRow.Cells["IdProducto"].Value);
                receta.Quitar(idMenu, idProducto);
                new CL_clsBitacora("Modificación de Receta", platillo + ": se quitó un ingrediente", "frmReceta");
                MostrarReceta();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }
    }
}
