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
    public partial class frmProducto: Form
    {
        CN_Producto producto = new CN_Producto();

        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            CV_Utiles.InicializarLabelPanel("Producto", panelProducto);

            dgvProducto.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducto.ReadOnly = true;
            dgvProducto.MultiSelect = false;
            dgvProducto.AllowUserToAddRows = false;

            MostrarProductos();
            dgvProducto.Select();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
        }

        private void dgvProducto_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProducto.SelectedRows.Count > 0)
            {
                txtNombre.Text = dgvProducto.Rows[dgvProducto.SelectedRows[0].Index].Cells["nombre"].Value.ToString();
                txtCategoria.Text = dgvProducto.Rows[dgvProducto.SelectedRows[0].Index].Cells["categoria"].Value.ToString();
                txtMarca.Text = dgvProducto.Rows[dgvProducto.SelectedRows[0].Index].Cells["marca"].Value.ToString();
            }
        }


        #region BOTONES

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(false);

                producto.InsertarProducto();
                MostrarProductos();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvProducto.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
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
            dgvProducto.Select();
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

                producto.ModificarProducto();

                MostrarProductos();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvProducto.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro de ELIMINAR definitivamente el producto?",
                                            "ELIMINAR PRODUCTO",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Question);
            if (resultado == DialogResult.OK)
            {
                producto.IdProducto = Convert.ToInt32(dgvProducto.Rows[dgvProducto.SelectedRows[0].Index].Cells["idProducto"].Value.ToString());
                producto.EliminarProducto();
                MostrarProductos();
                dgvProducto.Select();
            }
        }

        #endregion

        private void MostrarProductos()
        {
            dgvProducto.DataSource = producto.MostrarProductos();
        }

        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                producto.IdProducto = Convert.ToInt32(dgvProducto.Rows[dgvProducto.SelectedRows[0].Index].Cells["idProducto"].Value.ToString());
            }
            else
            {
                producto.IdProducto = 0;
            }

            producto.Nombre = txtNombre.Text;
            producto.Categoria = txtCategoria.Text;
            producto.Marca = txtMarca.Text;
        }

    }
}
