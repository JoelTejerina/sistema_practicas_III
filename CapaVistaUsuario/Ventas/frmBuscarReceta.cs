using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;

namespace CapaVistaUsuario.Ventas
{
    /// <summary>
    /// Buscar receta (CU-007-001): filtra los platillos por ingrediente, region, categoria, popularidad,
    /// temporada, tipo de evento, precio y tiempo, y muestra cuantos se pueden preparar con el stock actual.
    /// </summary>
    public class frmBuscarReceta : Form
    {
        private readonly CN_Receta receta = new CN_Receta();

        private readonly ComboBox cmbIngrediente = new ComboBox();
        private readonly TextBox txtRegion = new TextBox();
        private readonly TextBox txtCategoria = new TextBox();
        private readonly TextBox txtPopularidad = new TextBox();
        private readonly TextBox txtTemporada = new TextBox();
        private readonly TextBox txtTipoEvento = new TextBox();
        private readonly TextBox txtPrecioMin = new TextBox();
        private readonly TextBox txtPrecioMax = new TextBox();
        private readonly TextBox txtTiempoMax = new TextBox();
        private readonly CheckBox chkPreparables = new CheckBox();
        private readonly Button btnBuscar = new Button();
        private readonly Button btnLimpiar = new Button();
        private readonly Button btnVerReceta = new Button();
        private readonly Button btnCerrar = new Button();
        private readonly Label lblResultados = new Label();
        private readonly DataGridView dgvResultados = new DataGridView();

        public frmBuscarReceta()
        {
            Name = "frmBuscarReceta";
            Text = Idioma.Texto("BuscarReceta_Titulo");
            ClientSize = new Size(960, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;

            cmbIngrediente.Name = "cmbIngrediente";
            cmbIngrediente.DropDownStyle = ComboBoxStyle.DropDownList;
            txtRegion.Name = "txtRegion";
            txtCategoria.Name = "txtCategoria";
            txtPopularidad.Name = "txtPopularidad";
            txtTemporada.Name = "txtTemporada";
            txtTipoEvento.Name = "txtTipoEvento";
            txtPrecioMin.Name = "txtPrecioMin";
            txtPrecioMax.Name = "txtPrecioMax";
            txtTiempoMax.Name = "txtTiempoMax";
            chkPreparables.Name = "chkPreparables";
            chkPreparables.Text = Idioma.Texto("BuscarReceta_Preparables");
            chkPreparables.AutoSize = true;

            // Tres columnas de filtros: etiqueta + campo.
            Fila(cmbIngrediente, "BuscarReceta_Ingrediente", 12, 14, 130, 170);
            Fila(txtRegion, "Region", 12, 44, 130, 170);
            Fila(txtCategoria, "Categoria", 12, 74, 130, 170);
            Fila(txtPopularidad, "Popularidad", 340, 14, 120, 150);
            Fila(txtTemporada, "Temporada", 340, 44, 120, 150);
            Fila(txtTipoEvento, "TipoEvento", 340, 74, 120, 150);
            Fila(txtPrecioMin, "BuscarReceta_PrecioMin", 650, 14, 130, 90);
            Fila(txtPrecioMax, "BuscarReceta_PrecioMax", 650, 44, 130, 90);
            Fila(txtTiempoMax, "BuscarReceta_TiempoMax", 650, 74, 130, 90);
            chkPreparables.Location = new Point(12, 106);

            btnBuscar.Name = "btnBuscar";
            btnBuscar.Text = Idioma.Texto("Btn_Buscar");
            btnBuscar.Location = new Point(650, 102);
            btnBuscar.Size = new Size(100, 28);
            btnBuscar.Click += (s, e) => Buscar();
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Text = Idioma.Texto("Btn_Limpiar");
            btnLimpiar.Location = new Point(758, 102);
            btnLimpiar.Size = new Size(100, 28);
            btnLimpiar.Click += (s, e) => Limpiar();

            lblResultados.AutoSize = true;
            lblResultados.Location = new Point(12, 142);

            dgvResultados.Location = new Point(12, 164);
            dgvResultados.Size = new Size(936, 384);
            dgvResultados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvResultados.ReadOnly = true;
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.MultiSelect = false;
            dgvResultados.RowHeadersVisible = false;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvResultados.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) VerReceta(); };

            btnVerReceta.Name = "btnVerReceta";
            btnVerReceta.Text = Idioma.Texto("BuscarReceta_Ver");
            btnVerReceta.Size = new Size(140, 30);
            btnVerReceta.Location = new Point(12, 560);
            btnVerReceta.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnVerReceta.Click += (s, e) => VerReceta();

            btnCerrar.Text = Idioma.Texto("Btn_Cerrar");
            btnCerrar.Size = new Size(110, 30);
            btnCerrar.Location = new Point(838, 560);
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                chkPreparables, btnBuscar, btnLimpiar, lblResultados, dgvResultados, btnVerReceta, btnCerrar
            });

            Load += (s, e) =>
            {
                try
                {
                    CargarIngredientes();
                    Buscar();
                }
                catch (Exception ex)
                {
                    CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
                }
            };
        }

        private void Fila(Control campo, string claveEtiqueta, int x, int y, int anchoEtiqueta, int anchoCampo)
        {
            Label etiqueta = new Label { AutoSize = true, Text = Idioma.Texto(claveEtiqueta), Location = new Point(x, y + 3) };
            campo.Location = new Point(x + anchoEtiqueta, y);
            campo.Width = anchoCampo;
            Controls.Add(etiqueta);
            Controls.Add(campo);
        }

        private void CargarIngredientes()
        {
            DataTable productos = receta.ProductosDisponibles();
            DataRow todos = productos.NewRow();
            todos["IdProducto"] = 0;
            todos["Nombre"] = Idioma.Texto("Todos");
            todos["Medida"] = "";
            productos.Rows.InsertAt(todos, 0);
            cmbIngrediente.DataSource = productos;
            cmbIngrediente.DisplayMember = "Nombre";
            cmbIngrediente.ValueMember = "IdProducto";
        }

        private static bool LeerDecimal(TextBox caja, out decimal? valor)
        {
            valor = null;
            if (caja.Text.Trim().Length == 0) return true;
            decimal d;
            if (!decimal.TryParse(caja.Text.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out d) || d < 0) return false;
            valor = d;
            return true;
        }

        // Valida los numeros del filtro antes de consultar; todos los filtros son opcionales y se combinan.
        private void Buscar()
        {
            decimal? precioMin, precioMax;
            int tiempo = 0;
            bool tiempoOk = txtTiempoMax.Text.Trim().Length == 0 || (int.TryParse(txtTiempoMax.Text.Trim(), out tiempo) && tiempo >= 0);
            if (!LeerDecimal(txtPrecioMin, out precioMin) || !LeerDecimal(txtPrecioMax, out precioMax) || !tiempoOk)
            {
                MessageBox.Show(Idioma.Texto("BuscarReceta_NumeroInvalido"));
                return;
            }
            try
            {
                DataTable r = receta.Buscar(
                    Convert.ToInt32(cmbIngrediente.SelectedValue ?? 0), txtRegion.Text, txtCategoria.Text, txtPopularidad.Text,
                    txtTemporada.Text, txtTipoEvento.Text, precioMin, precioMax,
                    txtTiempoMax.Text.Trim().Length == 0 ? (int?)null : tiempo, chkPreparables.Checked);
                dgvResultados.DataSource = r;
                if (dgvResultados.Columns.Contains("IdMenu")) dgvResultados.Columns["IdMenu"].Visible = false;
                if (dgvResultados.Columns.Contains("Disponible")) dgvResultados.Columns["Disponible"].Visible = false;
                lblResultados.Text = string.Format(Idioma.Texto("BuscarReceta_Resultados"), r.Rows.Count);
                new CL_clsBitacora("Búsqueda de receta", r.Rows.Count + " resultado(s)", Name);
                if (r.Rows.Count == 0)
                {
                    MessageBox.Show(Idioma.Texto("BuscarReceta_SinResultados"), Idioma.Texto("BuscarReceta_Titulo"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void Limpiar()
        {
            foreach (TextBox t in new[] { txtRegion, txtCategoria, txtPopularidad, txtTemporada, txtTipoEvento, txtPrecioMin, txtPrecioMax, txtTiempoMax })
            {
                t.Clear();
            }
            cmbIngrediente.SelectedIndex = 0;
            chkPreparables.Checked = false;
            Buscar();
        }

        // Abre la receta del platillo elegido, solo para consultar.
        private void VerReceta()
        {
            if (dgvResultados.CurrentRow == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            int id = Convert.ToInt32(dgvResultados.CurrentRow.Cells["IdMenu"].Value);
            string nombre = Convert.ToString(dgvResultados.CurrentRow.Cells["Nombre"].Value);
            using (frmReceta f = new frmReceta(id, nombre, true))
            {
                f.ShowDialog(this);
            }
        }
    }
}
