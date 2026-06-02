using System;
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

        public frmFacturacion()
        {
            InitializeComponent();
        }

        private void frmFacturacion_Load(object sender, EventArgs e)
        {
            dgvPedido.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPedido.ReadOnly = true;
            dgvPedido.MultiSelect = false;
            dgvPedido.AllowUserToAddRows = false;

            AplicarIdioma();
            CargarCombos();
            MostrarPedidos();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_ABM_Permisos.AplicarModoLectura(this, PermisosSistema.Pedidos);
        }

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
            btnAgregar.Text = Idioma.Texto("Btn_Agregar");
            btnModificar.Text = Idioma.Texto("Btn_Modificar");
            btnGuardar.Text = Idioma.Texto("Btn_Guardar");
            btnGuardaCambios.Text = Idioma.Texto("Btn_GuardarCambios");
            btnEliminar.Text = Idioma.Texto("Btn_Eliminar");
            btnCobrar.Text = Idioma.Texto("Btn_Cobrar");
            btnCancelar.Text = Idioma.Texto("Btn_Cancelar");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
        }

        private void CargarCombos()
        {
            new CN_LlenarCombos(cmbMenu, "Menu", "IdMenu", "Nombre");
        }

        private void MostrarPedidos()
        {
            dgvPedido.DataSource = Ped.MostrarPedido();
        }

        private bool DatosValidos()
        {
            if (txtCliente.Text.Trim().Length == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_CampoObligatorio"));
                txtCliente.Select();
                return false;
            }
            if (cmbMenu.SelectedValue == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneMenu"));
                cmbMenu.Select();
                return false;
            }
            int cant;
            if (!int.TryParse(txtCantidad.Text.Trim(), out cant) || cant <= 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_CantidadInvalida"));
                txtCantidad.Select();
                return false;
            }
            decimal total;
            if (!decimal.TryParse(txtPrecioTotal.Text.Trim(), out total) || total < 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_PrecioInvalido"));
                txtPrecioTotal.Select();
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Ped.IdPedido = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Ped.NombreCliente = txtCliente.Text.Trim();
            Ped.IdMenu = Convert.ToInt32(cmbMenu.SelectedValue);
            Ped.Cantidad = Convert.ToInt32(txtCantidad.Text.Trim());
            Ped.FormaPago = cmbFormaPago.Text;
            Ped.InstruccionesEspeciales = txtInstrucciones.Text.Trim();
            Ped.Estado = cmbEstado.Text.Length == 0 ? "Pendiente" : cmbEstado.Text;
            Ped.PrecioTotal = Convert.ToDecimal(txtPrecioTotal.Text.Trim());
            Ped.Fecha = dtpFecha.Value;
        }
        #endregion

        #region BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            cmbMenu.SelectedIndex = -1;
            cmbFormaPago.SelectedIndex = -1;
            cmbEstado.SelectedIndex = 0;
            txtCantidad.Text = "0";
            txtPrecioTotal.Text = "0";
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
                Ped.InsertarPedido();
                new CL_clsBitacora("Alta de Pedido", "Cliente: " + txtCliente.Text.Trim(), "frmFacturacion");
                MostrarPedidos();
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
                lblID.Text = "0";
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
                Ped.IdPedido = Convert.ToInt32(lblID.Text);
                Ped.EliminarPedido();
                new CL_clsBitacora("Baja de Pedido", "Baja id " + lblID.Text, "frmFacturacion");
                MostrarPedidos();
                CV_Utiles.LimpiarControles(this);
                lblID.Text = "0";
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
            lblID.Text = "0";
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dgvPedido_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvPedido.SelectedRows.Count == 0) return;
            var f = dgvPedido.SelectedRows[0];
            lblID.Text = f.Cells["IdPedido"].Value.ToString();
            txtCliente.Text = f.Cells["NombreCliente"].Value.ToString();
            if (f.Cells["IdMenu"].Value != DBNull.Value && f.Cells["IdMenu"].Value != null)
            {
                cmbMenu.SelectedValue = Convert.ToInt32(f.Cells["IdMenu"].Value);
            }
            txtCantidad.Text = f.Cells["Cantidad"].Value.ToString();
            cmbFormaPago.Text = f.Cells["FormaPago"].Value.ToString();
            cmbEstado.Text = f.Cells["Estado"].Value.ToString();
            txtPrecioTotal.Text = f.Cells["PrecioTotal"].Value.ToString();
            txtInstrucciones.Text = f.Cells["InstruccionesEspeciales"].Value.ToString();
            if (f.Cells["Fecha"].Value != DBNull.Value && f.Cells["Fecha"].Value != null)
            {
                dtpFecha.Value = Convert.ToDateTime(f.Cells["Fecha"].Value);
            }
        }
    }
}
