using CapaLogicaNegocio.Administrador;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmProveedores : Form
    {
        CN_Proveedores proveedor = new CN_Proveedores();

        public frmProveedores()
        {
            InitializeComponent();
        }

        private void frmProveedores_Load_1(object sender, EventArgs e)
        {
            CV_Utiles.InicializarLabelPanel("Proveedores", panelProveedores);

            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.ReadOnly = true;
            dgvProveedores.MultiSelect = false;
            dgvProveedores.AllowUserToAddRows = false;

            MostrarProveedores();
            dgvProveedores.Select();
        }

        private void dgvProveedores_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count > 0)
            {
                txtNombre.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["nombre"].Value.ToString();
                txtRazonSocial.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["razonSocial"].Value.ToString();
                txtCuit.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["cuit"].Value.ToString();
                txtEmail.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["direccionDeEmail"].Value.ToString();
                txtTelefono.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["telefono"].Value.ToString();
                txtCertificadoAfip.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["certificadoAfip"].Value.ToString();
                txtCategoria.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["categoria"].Value.ToString();
                txtProducto.Text = dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["producto"].Value.ToString();
            }
        }

        #region BOTONES

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(true);
                proveedor.ModificarProveedor();
                MostrarProveedores();
                // Puedes agregar más lógica según tus necesidades.
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }

            errorNombre.Dispose();
            errorCuit.Dispose();
            errorRazonSocial.Dispose();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            CV_Botonera.btnFormularios(this, btnAgregar);
            txtNombre.Select();
        }

        private void btnCancelar_Click_1(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            dgvProveedores.Select();
            errorNombre.Dispose();
            errorCuit.Dispose();
            errorRazonSocial.Dispose();
        }

        private void btnModificar_Click_1(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnModificar);
            CV_Utiles.DesbloquearControles(this);
            txtNombre.Select();
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(false);
                proveedor.InsertarProveedor();
                MostrarProveedores();
                // Puedes agregar más lógica según tus necesidades.
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }

            errorNombre.Dispose();
            errorCuit.Dispose();
            errorRazonSocial.Dispose();
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro de ELIMINAR definitivamente este PROVEEDOR?",
                    "ELIMINAR PROVEEDOR",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);
            if (resultado == DialogResult.OK)
            {
                proveedor.IdProveedor = Convert.ToInt32(dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["idProveedor"].Value.ToString());
                proveedor.EliminarProveedor();
                MostrarProveedores();
                dgvProveedores.Select();
            }
        }

        #endregion

        #region Validaciones_Nivel_Formulario

        private void txtRazonSocial_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtRazonSocial.Text))
            {
                errorRazonSocial.Dispose();
                errorRazonSocial.SetError(this.txtRazonSocial, "Debe tener un valor");
            }
            else
            {
                errorRazonSocial.Dispose();
                errorRazonSocial.SetError(this.txtRazonSocial, "");
                errorRazonSocial.SetError(this.txtRazonSocial, "CORRECTO");
            }
        }

        #endregion

        #region METODOS

        private void MostrarProveedores()
        {
            dgvProveedores.DataSource = proveedor.MostrarProveedores();
        }

        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                proveedor.IdProveedor = Convert.ToInt32(dgvProveedores.Rows[dgvProveedores.SelectedRows[0].Index].Cells["idProveedor"].Value.ToString());
            }
            else
            {
                proveedor.IdProveedor = 0;
            }
            proveedor.Nombre = txtNombre.Text;
            proveedor.RazonSocial = txtRazonSocial.Text;
            proveedor.Cuit = txtCuit.Text;
            proveedor.DireccionDeEmail = txtEmail.Text;
            proveedor.Telefono = txtTelefono.Text;
            proveedor.CertificadoAfip = txtCertificadoAfip.Text;
            proveedor.Categoria = txtCategoria.Text;
            proveedor.Producto = txtProducto.Text;
        }
        #endregion
    }
}
