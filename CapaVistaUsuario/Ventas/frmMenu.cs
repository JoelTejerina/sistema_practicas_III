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
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();

        private Button btnReceta;
        private Button btnDisponibilidad;
        private Button btnPrecios;
        private Button btnBuscarReceta;

        public frmMenu()
        {
            InitializeComponent();
            BuscadorGrilla.Agregar(dgvMenu, "Nombre", "Categoria", "TipoEvento", "Temporada");

            // La receta (ingredientes del platillo) se abre desde un boton junto a los campos.
            btnReceta = new Button { Name = "btnReceta", Location = new System.Drawing.Point(562, 303), Size = new System.Drawing.Size(82, 26) };
            btnReceta.Click += btnReceta_Click;
            Controls.Add(btnReceta);

            // Fila extra: disponibilidad del platillo, ajuste masivo de precios y busqueda de recetas.
            const int extra = 40;
            ClientSize = new System.Drawing.Size(ClientSize.Width, ClientSize.Height + extra);
            pnlBotones.Top += extra;
            btnDisponibilidad = new Button { Name = "btnDisponibilidad", Location = new System.Drawing.Point(120, 340), Size = new System.Drawing.Size(210, 26) };
            btnPrecios = new Button { Name = "btnActualizarPrecios", Location = new System.Drawing.Point(340, 340), Size = new System.Drawing.Size(190, 26) };
            btnBuscarReceta = new Button { Name = "btnBuscarReceta", Location = new System.Drawing.Point(540, 340), Size = new System.Drawing.Size(140, 26) };
            btnDisponibilidad.Click += btnDisponibilidad_Click;
            btnPrecios.Click += btnPrecios_Click;
            btnBuscarReceta.Click += btnBuscarReceta_Click;
            Controls.AddRange(new Control[] { btnDisponibilidad, btnPrecios, btnBuscarReceta });
            dgvMenu.RowPrePaint += dgvMenu_RowPrePaint;
        }

        // Los platillos no disponibles se ven en gris en la grilla.
        private void dgvMenu_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridViewRow fila = dgvMenu.Rows[e.RowIndex];
            if (dgvMenu.Columns.Contains("Disponible") && fila.Cells["Disponible"].Value is bool && !(bool)fila.Cells["Disponible"].Value)
            {
                fila.DefaultCellStyle.ForeColor = System.Drawing.Color.Gray;
                fila.DefaultCellStyle.Font = new System.Drawing.Font(dgvMenu.Font, System.Drawing.FontStyle.Italic);
            }
            else
            {
                fila.DefaultCellStyle.ForeColor = System.Drawing.Color.Empty;
                fila.DefaultCellStyle.Font = null;
            }
        }

        // Marca el platillo seleccionado como disponible / no disponible (se agoto un insumo, por ejemplo).
        private void btnDisponibilidad_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0 || dgvMenu.CurrentRow == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            int id = Convert.ToInt32(lblID.Text);
            object actual = dgvMenu.CurrentRow.Cells["Disponible"].Value;
            bool disponible = actual is bool && (bool)actual;
            try
            {
                Men.CambiarDisponibilidad(id, !disponible);
                new CL_clsBitacora("Disponibilidad de platillo", "Menu " + id + (disponible ? ": no disponible" : ": disponible"), "frmMenu");
                MessageBox.Show(Idioma.Texto(disponible ? "Menu_MarcadoNoDisponible" : "Menu_MarcadoDisponible"),
                    Idioma.Texto("Menu_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                MostrarMenu();
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Ajuste masivo de precios: un porcentaje (positivo o negativo) sobre toda la carta o una categoria.
        private void btnPrecios_Click(object sender, EventArgs e)
        {
            using (Form dlg = new Form())
            using (Label lblPct = new Label { AutoSize = true, Location = new System.Drawing.Point(12, 18), Text = Idioma.Texto("Menu_Porcentaje") })
            using (TextBox txtPct = new TextBox { Name = "txtPorcentaje", Location = new System.Drawing.Point(150, 15), Width = 90 })
            using (Label lblCat = new Label { AutoSize = true, Location = new System.Drawing.Point(12, 52), Text = Idioma.Texto("Categoria") + ":" })
            using (ComboBox cmbCat = new ComboBox { Name = "cmbCategoriaPrecios", DropDownStyle = ComboBoxStyle.DropDownList, Location = new System.Drawing.Point(150, 49), Width = 180 })
            using (Button ok = new Button { Text = Idioma.Texto("Btn_Aceptar"), DialogResult = DialogResult.OK, Location = new System.Drawing.Point(150, 90), Size = new System.Drawing.Size(85, 28) })
            using (Button cancel = new Button { Text = Idioma.Texto("Btn_Cancelar"), DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(245, 90), Size = new System.Drawing.Size(85, 28) })
            {
                dlg.Text = Idioma.Texto("Menu_ActualizarPrecios");
                dlg.ClientSize = new System.Drawing.Size(350, 135);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.AcceptButton = ok;
                dlg.CancelButton = cancel;

                System.Collections.Generic.List<string> categorias = new System.Collections.Generic.List<string> { Idioma.Texto("Todos") };
                foreach (System.Data.DataRow fila in Men.Categorias().Rows)
                {
                    categorias.Add(Convert.ToString(fila[0]));
                }
                cmbCat.DataSource = categorias;
                dlg.Controls.AddRange(new Control[] { lblPct, txtPct, lblCat, cmbCat, ok, cancel });

                while (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    decimal pct;
                    if (!decimal.TryParse(txtPct.Text.Trim().Replace(',', '.'), System.Globalization.NumberStyles.Number,
                        System.Globalization.CultureInfo.InvariantCulture, out pct) || pct == 0 || pct < -90 || pct > 300)
                    {
                        MessageBox.Show(Idioma.Texto("Msg_PorcentajeInvalido"));
                        continue;
                    }
                    string categoria = cmbCat.SelectedIndex <= 0 ? null : Convert.ToString(cmbCat.SelectedItem);
                    try
                    {
                        int cantidad = Men.CantidadAfectados(categoria);
                        if (cantidad == 0)
                        {
                            MessageBox.Show(Idioma.Texto("Menu_SinPlatillos"));
                            return;
                        }
                        if (MessageBox.Show(string.Format(Idioma.Texto("Menu_ConfirmPrecios"), pct.ToString("0.##"), cantidad),
                            Idioma.Texto("Menu_ActualizarPrecios"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        {
                            return;
                        }
                        int cambiados = Men.ActualizarPrecios(pct, categoria);
                        new CL_clsBitacora("Actualización de precios", pct.ToString("0.##") + "% a " + cambiados + " platillo(s)" +
                            (categoria == null ? "" : " de " + categoria), "frmMenu");
                        MessageBox.Show(string.Format(Idioma.Texto("Menu_PreciosActualizados"), cambiados),
                            Idioma.Texto("Menu_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MostrarMenu();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
                    }
                    return;
                }
            }
        }

        private void btnBuscarReceta_Click(object sender, EventArgs e)
        {
            using (frmBuscarReceta f = new frmBuscarReceta())
            {
                f.ShowDialog(this);
            }
        }

        private void btnReceta_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            using (frmReceta receta = new frmReceta(Convert.ToInt32(lblID.Text), txtNombre.Text,
                PermisosSistema.SoloLectura(PermisosSistema.Menu)))
            {
                receta.ShowDialog(this);
            }
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenu.ReadOnly = true;
            dgvMenu.MultiSelect = false;
            dgvMenu.AllowUserToAddRows = false;

            AplicarIdioma();
            btnReceta.Text = Idioma.Texto("Btn_Receta");
            btnDisponibilidad.Text = Idioma.Texto("Menu_CambiarDisponibilidad");
            btnPrecios.Text = Idioma.Texto("Menu_ActualizarPrecios");
            btnBuscarReceta.Text = Idioma.Texto("BuscarReceta_Titulo");
            CargarCombos();
            MostrarMenu();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Menu);
            if (PermisosSistema.SoloLectura(PermisosSistema.Menu))
            {
                btnDisponibilidad.Enabled = false;
                btnPrecios.Enabled = false;
            }
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
            // Opcionales: Popularidad, Region, Temporada y Tipo de evento.
            if (!obligatorios.Validar(txtNombre, txtDescripcion, txtIngredientes, txtCategoria,
                txtPrecio, txtTiempo, cmbStock))
            {
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
            int tiempo;
            if (!int.TryParse(txtTiempo.Text.Trim(), out tiempo) || tiempo < 0)
            {
                obligatorios.Marcar(txtTiempo);
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
                MessageBox.Show(Idioma.Texto("Msg_GuardadoOk"), Idioma.Texto("Menu_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            CargarSeleccion();
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
                MessageBox.Show(Idioma.Texto("Msg_GuardadoOk"), Idioma.Texto("Menu_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(Idioma.Texto("Msg_BajaOk"), Idioma.Texto("Menu_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (e.RowIndex < 0) return;
            CargarSeleccion();
        }

        /// <summary>Llena los campos con el registro seleccionado en la grilla.</summary>
        private void CargarSeleccion()
        {
            if (dgvMenu.SelectedRows.Count == 0) return;
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
