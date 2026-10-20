using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Ventas
{
    public partial class frmFacturacion : Form
    {
        CN_Pedido Ped = new CN_Pedido();
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();

        // Detalle del pedido: uno o mas platillos con su cantidad.
        private readonly BindingList<ItemPedido> items = new BindingList<ItemPedido>();
        private DataTable preciosMenu;
        private string estadoAnterior = "";
        private Label lblDetalle;
        private Button btnAgregarItem;
        private Button btnQuitarItem;
        private DataGridView dgvItems;

        public frmFacturacion()
        {
            InitializeComponent();
            CrearDetalle();
            BuscadorGrilla.Agregar(dgvPedido, "NombreCliente", "Estado", "FormaPago");
        }

        private void frmFacturacion_Load(object sender, EventArgs e)
        {
            dgvPedido.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPedido.ReadOnly = true;
            dgvPedido.MultiSelect = false;
            dgvPedido.AllowUserToAddRows = false;

            // La elaboracion agrega el estado "Listo" entre "En curso" y "Cobrado".
            if (!cmbEstado.Items.Contains("Listo"))
            {
                cmbEstado.Items.Insert(2, "Listo");
            }
            txtPrecioTotal.ReadOnly = true;   // el total se calcula con el detalle

            AplicarIdioma();
            CargarCombos();
            MostrarPedidos();
            CV_Utiles.BloquearControles(this);
            HabilitarDetalle(false);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Pedidos);
        }

        #region DETALLE DEL PEDIDO
        // Arma, debajo de los campos, la lista de platillos del pedido.
        private void CrearDetalle()
        {
            const int extra = 125;
            ClientSize = new Size(ClientSize.Width, ClientSize.Height + extra);
            pnlBotones.Top += extra;

            lblDetalle = new Label { AutoSize = true, Location = new Point(12, 349), Font = new Font(Font, FontStyle.Bold) };
            btnAgregarItem = new Button { Name = "btnAgregarItem", Location = new Point(150, 343), Size = new Size(120, 26) };
            btnQuitarItem = new Button { Name = "btnQuitarItem", Location = new Point(276, 343), Size = new Size(120, 26) };
            btnAgregarItem.Click += btnAgregarItem_Click;
            btnQuitarItem.Click += btnQuitarItem_Click;

            dgvItems = new DataGridView
            {
                Name = "dgvItems",
                Location = new Point(12, 374),
                Size = new Size(760, 100),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = true,
                DataSource = items
            };
            dgvItems.DataBindingComplete += (s, e) => FormatearItems();

            Controls.Add(lblDetalle);
            Controls.Add(btnAgregarItem);
            Controls.Add(btnQuitarItem);
            Controls.Add(dgvItems);
        }

        private void FormatearItems()
        {
            if (dgvItems.Columns.Count == 0) return;
            dgvItems.Columns["IdMenu"].Visible = false;
            dgvItems.Columns["Platillo"].HeaderText = Idioma.Texto("Platillo");
            dgvItems.Columns["Cantidad"].HeaderText = Idioma.Texto("Cantidad");
            dgvItems.Columns["PrecioUnitario"].HeaderText = Idioma.Texto("PrecioUnitario");
            dgvItems.Columns["PrecioUnitario"].DefaultCellStyle.Format = "N2";
            dgvItems.Columns["Subtotal"].HeaderText = Idioma.Texto("Subtotal");
            dgvItems.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
        }

        private void HabilitarDetalle(bool habilitado)
        {
            btnAgregarItem.Enabled = habilitado;
            btnQuitarItem.Enabled = habilitado;
        }

        private void LimpiarItems()
        {
            items.Clear();
            RecalcularTotal();
        }

        private void RecalcularTotal()
        {
            txtPrecioTotal.Text = items.Sum(i => i.Subtotal).ToString("0.00");
        }

        // Agrega el platillo elegido (con su cantidad) al detalle. Si ya estaba, suma la cantidad.
        private bool AgregarItemElegido()
        {
            if (!obligatorios.Validar(cmbMenu, txtCantidad))
            {
                return false;
            }

            int cant;
            if (!int.TryParse(txtCantidad.Text.Trim(), out cant) || cant <= 0)
            {
                obligatorios.Marcar(txtCantidad);
                MessageBox.Show(Idioma.Texto("Msg_CantidadInvalida"));
                txtCantidad.Select();
                return false;
            }

            int idMenu = Convert.ToInt32(cmbMenu.SelectedValue);
            DataRow[] filas = preciosMenu.Select("IdMenu = " + idMenu);
            if (filas.Length == 0)
            {
                obligatorios.Marcar(cmbMenu);
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneMenu"));
                return false;
            }

            ItemPedido existente = items.FirstOrDefault(i => i.IdMenu == idMenu);
            if (existente != null)
            {
                existente.Cantidad += cant;
            }
            else
            {
                items.Add(new ItemPedido
                {
                    IdMenu = idMenu,
                    Platillo = Convert.ToString(filas[0]["Nombre"]),
                    Cantidad = cant,
                    PrecioUnitario = Convert.ToDecimal(filas[0]["Precio"])
                });
            }
            items.ResetBindings();
            RecalcularTotal();
            return true;
        }

        private void btnAgregarItem_Click(object sender, EventArgs e)
        {
            if (AgregarItemElegido())
            {
                cmbMenu.SelectedIndex = -1;
                txtCantidad.Text = "1";
                cmbMenu.Select();
            }
        }

        private void btnQuitarItem_Click(object sender, EventArgs e)
        {
            if (dgvItems.CurrentRow == null || dgvItems.CurrentRow.Index < 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneItem"));
                return;
            }
            items.RemoveAt(dgvItems.CurrentRow.Index);
            RecalcularTotal();
        }
        #endregion

        #region METODOS
        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("Fact_Titulo");
            lblCliente.Text = Idioma.Texto("Cliente");
            lblMenu.Text = Idioma.Texto("Menu");
            lblCantidad.Text = Idioma.Texto("Cantidad");
            lblFormaPago.Text = Idioma.Texto("FormaPago");
            lblEstado.Text = Idioma.Texto("Estado");
            lblPrecioTotal.Text = Idioma.Texto("PrecioTotal");
            lblFecha.Text = Idioma.Texto("Fecha");
            lblInstrucciones.Text = Idioma.Texto("Instrucciones");
            lblDetalle.Text = Idioma.Texto("DetallePedido");
            btnAgregarItem.Text = Idioma.Texto("Btn_AgregarItem");
            btnQuitarItem.Text = Idioma.Texto("Btn_QuitarItem");
            btnAgregar.Text = Idioma.Texto("Btn_Agregar");
            btnModificar.Text = Idioma.Texto("Btn_Modificar");
            btnGuardar.Text = Idioma.Texto("Btn_Guardar");
            btnGuardaCambios.Text = Idioma.Texto("Btn_GuardarCambios");
            btnEliminar.Text = Idioma.Texto("Btn_Eliminar");
            btnCobrar.Text = Idioma.Texto("Btn_Cobrar");
            btnCancelar.Text = Idioma.Texto("Btn_Cancelar");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
            FormatearItems();
        }

        private void CargarCombos()
        {
            new CN_LlenarCombos(cmbMenu, "Menu", "IdMenu", "Nombre", "Activo = 1");
            preciosMenu = new CN_Menu().MostrarMenu();
        }

        private void MostrarPedidos()
        {
            dgvPedido.DataSource = Ped.MostrarPedido();
        }

        private bool DatosValidos()
        {
            // Obligatorios todos menos las instrucciones especiales. El pedido necesita al menos un
            // platillo: si el detalle esta vacio se toma el que esta elegido en Menu y Cantidad.
            if (!obligatorios.Validar(txtCliente, cmbFormaPago, cmbEstado, dtpFecha))
            {
                return false;
            }
            if (items.Count == 0 && !AgregarItemElegido())
            {
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Ped.IdPedido = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Ped.NombreCliente = txtCliente.Text.Trim();
            Ped.FormaPago = cmbFormaPago.Text;
            Ped.InstruccionesEspeciales = txtInstrucciones.Text.Trim();
            Ped.Estado = cmbEstado.Text.Length == 0 ? "Pendiente" : cmbEstado.Text;
            Ped.EstadoAnterior = estadoAnterior;
            Ped.Fecha = dtpFecha.Value;
            Ped.Items = items.ToList();
        }

        // El stock no alcanza: no es un error del sistema, se le explica al usuario que falta.
        private void AvisarStockInsuficiente(StockInsuficienteException ex)
        {
            MessageBox.Show(ex.Message, Idioma.Texto("Stock_AlertaTitulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            LimpiarItems();
            HabilitarDetalle(true);
            cmbMenu.SelectedIndex = -1;
            cmbFormaPago.SelectedIndex = -1;
            cmbEstado.SelectedIndex = 0;
            txtCantidad.Text = "1";
            estadoAnterior = "";
            lblID.Text = "0";
            CV_Botonera.btnFormularios(this, btnAgregar);
            txtCliente.Select();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                PasarDatos(false);
                int numero = Ped.InsertarPedido();
                new CL_clsBitacora("Alta de Pedido", "Pedido " + numero + ", cliente: " + txtCliente.Text.Trim(), "frmFacturacion");
                MostrarPedidos();
                CV_Botonera.btnFormularios(this, btnGuardar);
                CV_Utiles.BloquearControles(this);
                CV_Utiles.LimpiarControles(this);
                LimpiarItems();
                HabilitarDetalle(false);
            }
            catch (StockInsuficienteException ex)
            {
                AvisarStockInsuficiente(ex);
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
            HabilitarDetalle(true);
            CV_Botonera.btnFormularios(this, btnModificar);
            txtCliente.Select();
        }

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;
            try
            {
                PasarDatos(true);
                Ped.ModificarPedido();
                new CL_clsBitacora("Modificación de Pedido", "Modifica id " + lblID.Text, "frmFacturacion");
                MostrarPedidos();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                CV_Utiles.LimpiarControles(this);
                LimpiarItems();
                HabilitarDetalle(false);
                lblID.Text = "0";
            }
            catch (StockInsuficienteException ex)
            {
                AvisarStockInsuficiente(ex);
            }
            catch (InvalidOperationException ex)
            {
                // Reglas de negocio (por ejemplo, solo se cancelan pedidos pendientes).
                MessageBox.Show(ex.Message, Idioma.Texto("Fact_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
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
                try
                {
                    Ped.IdPedido = Convert.ToInt32(lblID.Text);
                    Ped.EliminarPedido();
                    new CL_clsBitacora("Baja de Pedido", "Baja id " + lblID.Text, "frmFacturacion");
                    MostrarPedidos();
                    CV_Utiles.LimpiarControles(this);
                    LimpiarItems();
                    lblID.Text = "0";
                }
                catch (Exception ex)
                {
                    CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
                }
            }
        }

        // Cobrar pedido: marca el pedido seleccionado como Cobrado
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            DialogResult r = MessageBox.Show(Idioma.Texto("Msg_ConfirmCobrar"),
                Idioma.Texto("Btn_Cobrar"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (r == DialogResult.OK)
            {
                Ped.IdPedido = Convert.ToInt32(lblID.Text);
                Ped.CobrarPedido();
                new CL_clsBitacora("Cobro de Pedido", "Cobra id " + lblID.Text, "frmFacturacion");
                MostrarPedidos();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            LimpiarItems();
            HabilitarDetalle(false);
            lblID.Text = "0";
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgvPedido_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            CargarSeleccion();
        }

        /// <summary>Llena los campos y el detalle con el registro seleccionado en la grilla.</summary>
        private void CargarSeleccion()
        {
            if (dgvPedido.SelectedRows.Count == 0) return;
            var f = dgvPedido.SelectedRows[0];
            lblID.Text = f.Cells["IdPedido"].Value.ToString();
            txtCliente.Text = f.Cells["NombreCliente"].Value.ToString();
            cmbFormaPago.Text = f.Cells["FormaPago"].Value.ToString();
            cmbEstado.Text = f.Cells["Estado"].Value.ToString();
            estadoAnterior = cmbEstado.Text;
            txtInstrucciones.Text = f.Cells["InstruccionesEspeciales"].Value.ToString();
            if (f.Cells["Fecha"].Value != DBNull.Value && f.Cells["Fecha"].Value != null)
            {
                dtpFecha.Value = Convert.ToDateTime(f.Cells["Fecha"].Value);
            }

            // Detalle: todos los platillos del pedido. Menu y Cantidad muestran el primero.
            items.Clear();
            foreach (DataRow fila in Ped.DetallePedido(Convert.ToInt32(lblID.Text)).Rows)
            {
                items.Add(new ItemPedido
                {
                    IdMenu = Convert.ToInt32(fila["IdMenu"]),
                    Platillo = Convert.ToString(fila["Platillo"]),
                    Cantidad = Convert.ToInt32(fila["Cantidad"]),
                    PrecioUnitario = Convert.ToDecimal(fila["PrecioUnitario"])
                });
            }

            if (items.Count > 0)
            {
                cmbMenu.SelectedValue = items[0].IdMenu;
                txtCantidad.Text = items[0].Cantidad.ToString();
                RecalcularTotal();
            }
            else
            {
                if (f.Cells["IdMenu"].Value != DBNull.Value && f.Cells["IdMenu"].Value != null)
                {
                    cmbMenu.SelectedValue = Convert.ToInt32(f.Cells["IdMenu"].Value);
                }
                txtCantidad.Text = f.Cells["Cantidad"].Value.ToString();
                txtPrecioTotal.Text = f.Cells["PrecioTotal"].Value.ToString();
            }
        }
    }
}
