using System;
using System.Data;
using System.Windows.Forms;
using CapaLogicaNegocio.Administrador;
using CapaComun;
using CapaLogicaNegocio;

namespace CapaVistaUsuario
{
    public partial class frmFacturacion : Form
    {
        CN_Facturacion facturacion = new CN_Facturacion();
        CN_Menu menu = new CN_Menu();

        public frmFacturacion()
        {
            InitializeComponent();
        }

        private void frmFacturacion_Load(object sender, EventArgs e)
        {
            cbxOpcionPago.Items.Add("DEBITO");
            cbxOpcionPago.Items.Add("CREDITO");
            cbxOpcionPago.Items.Add("EFECTIVO");

            // Seleccionar el primer elemento por defecto si hay opciones disponibles
            if (cbxOpcionPago.Items.Count > 0)
            {
                cbxOpcionPago.SelectedIndex = 0;
            }

            CV_Utiles.InicializarLabelPanel("Facturacion", panelFacturacion);
            dgvFacturacion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFacturacion.ReadOnly = true;
            dgvFacturacion.MultiSelect = false;
            dgvFacturacion.AllowUserToAddRows = false;

            CN_LlenarCombos llenarCMB2 = new CN_LlenarCombos(cbxMenu, "Menu", "idMenu", "nombre", "precio", "descripcion");

            MostrarFacturaciones();
            dgvFacturacion.Select();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
        }

        private void dgvFacturacion_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvFacturacion.SelectedRows.Count > 0)
            {
                txtNombre.Text = dgvFacturacion.Rows[dgvFacturacion.SelectedRows[0].Index].Cells["nombreDelCliente"].Value.ToString();
                txtCantidad.Text = dgvFacturacion.Rows[dgvFacturacion.SelectedRows[0].Index].Cells["cantidades"].Value.ToString();
                cbxMenu.Text = dgvFacturacion.Rows[dgvFacturacion.SelectedRows[0].Index].Cells["idMenu"].Value.ToString();
                cbxOpcionPago.Text = dgvFacturacion.Rows[dgvFacturacion.SelectedRows[0].Index].Cells["formaDePago"].Value.ToString();
            }
        }

        #region BOTONES

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(false);

                facturacion.InsertarFacturacion();
                MostrarFacturaciones();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvFacturacion.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }

            errorProvider1.Dispose();
            errorProvider2.Dispose();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            CV_Botonera.btnFormularios(this, btnAgregar);
            txtNombre.Select();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            dgvFacturacion.Select();
            errorProvider1.Dispose();
            errorProvider2.Dispose();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnModificar);
            CV_Utiles.DesbloquearControles(this);
            txtNombre.Select();
        }

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(true);

                facturacion.ModificarFacturacion();

                MostrarFacturaciones();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvFacturacion.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
            errorProvider1.Dispose();
            errorProvider2.Dispose();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro de ELIMINAR definitivamente a la factura?",
                                            "ELIMINAR FACTURACIÓN",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Question);
            if (resultado == DialogResult.OK)
            {
                facturacion.IdFacturacion = Convert.ToInt32(dgvFacturacion.Rows[dgvFacturacion.SelectedRows[0].Index].Cells["idFacturacion"].Value.ToString());
                facturacion.EliminarFacturacion();
                MostrarFacturaciones();
                dgvFacturacion.Select();
            }
        }

        #endregion

        #region Validaciones_Nivel_Formulario

        private void txtNombre_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtNombre.Text))
            {
                errorProvider2.Dispose();
                errorProvider1.SetError(this.txtNombre, "Debe tener un valor");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider1.SetError(this.txtNombre, "");
                errorProvider2.SetError(this.txtNombre, "CORRECTO");
            }
        }

        #endregion

        #region METODOS

        private void MostrarFacturaciones()
        {
            dgvFacturacion.DataSource = facturacion.MostrarFacturacion();
        }

        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                facturacion.IdFacturacion = Convert.ToInt32(dgvFacturacion.Rows[dgvFacturacion.SelectedRows[0].Index].Cells["idFacturacion"].Value.ToString());
            }
            else
            {
                facturacion.IdFacturacion = 0;
            }
            if (cbxMenu.SelectedItem == null)
            {
                facturacion.IdMenu = "0";
            }
            else
            {
                facturacion.IdMenu = cbxMenu.SelectedValue.ToString();
            }
            facturacion.NombreDelCliente = txtNombre.Text;
            facturacion.Cantidades = Convert.ToInt32(txtCantidad.Text);
            menu.obtenerPrecio(facturacion.IdMenu);
            
            facturacion.FormaDePago = cbxOpcionPago.Text;
        }
        #endregion
    }
}