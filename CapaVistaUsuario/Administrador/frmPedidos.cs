using System;
using System.Drawing;
using System.Windows.Forms;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador;
using CapaComun;

namespace CapaVistaUsuario
{
    public partial class frmPedidos : Form
    {
        //Instancio los objetos de las clses que utilizo en el form
        CN_Pedidos pedido = new CN_Pedidos();
        CV_Validar_Mail ValidaCorreo = new CV_Validar_Mail();


        public frmPedidos()
        {
            InitializeComponent();
        }

        private void frmPedidos_Load(object sender, EventArgs e)
        {
            dgvPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //Selecciona toda la fila
            dgvPedidos.ReadOnly = true; //hace que la grilla no se pueda editar
            dgvPedidos.MultiSelect = false; //desactiva la seleccion multiple
            dgvPedidos.AllowUserToAddRows = false; //desactiva  la ultima fila 

            txtInstrucionesEspeciales.Text = "0";

            MostrarPedidos();
            dgvPedidos.Select();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
        }

        private void dgvPedido_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPedidos.SelectedRows.Count > 0)
            {
                /*grpPersonas.Text = "Identificacion Persona Nº " + dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdPersona"].Value.ToString();
                txtApellido.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Apellido"].Value.ToString();
                txtNombres.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Nombres"].Value.ToString();

                cmbLocalidad.SelectedValue = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdLocalidad"].Value; ;*/
                txtNombre.Text = dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["nombre"].Value.ToString();
                txtApellido.Text = dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["apellido"].Value.ToString();
                txtCantidad.Text = dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["cantidad"].Value.ToString();
                txtInstrucionesEspeciales.Text = dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["instrucionesEspeciales"].Value.ToString();
                cbxOpcionPago.Text = dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["opcionDePago"].Value.ToString();
                cbxMenu.SelectedValue = dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["IdMenu"].Value;
                //pedido.NombreCliente = txtNombre.Text;
                //pedido.Apellido = txtApellido.Text;
                //pedido.Cantidad = txtCantidad.Text;
                //pedido.InstruccionesEspeciales = txtInstrucionesEspeciales.Text;
                //pedido.OpcionDePago = cbxOpcionPago.Text;

            }
        }

        #region BOTONES

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(false);

                pedido.InsertarPedido();
                MostrarPedidos();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPedidos.Select();
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
            dgvPedidos.Select();
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

                pedido.ModificarPedido();

                MostrarPedidos();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPedidos.Select();
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
            var resultado = MessageBox.Show("esta seguro de ELIMINAR defilitivamente a la pedido?",
                                                        "ELIMINAR PEDIDO",
                                                        MessageBoxButtons.OKCancel,
                                                        MessageBoxIcon.Question);
            if (resultado == DialogResult.OK )
            {
                pedido.IdPedido = Convert.ToInt32(dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["IdPedido"].Value.ToString());
                pedido.EliminarPedido();
                MostrarPedidos();
                dgvPedidos.Select();
            }
        }

        #endregion


        #region Validaciones_Nivel_Formulario

        private void txtApellido_Validated(object sender, EventArgs e)
        {
            if (this.txtNombre.Text.Length == 0)
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

        private void txtNombres_Validated(object sender, EventArgs e)
        {
            if (this.txtNombre.Text.Length == 0)
            {
                errorProvider2.Dispose();
                errorProvider1.SetError(this.txtApellido, "El campo Nombre no puede tener un valor vacio");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider1.SetError(this.txtApellido, "");
                errorProvider2.SetError(this.txtApellido, "CORRECTO");
            }
        }
        private void txtApellido_Validated_1(object sender, EventArgs e)
        {
            if (this.txtApellido.Text.Length == 0)
            {
                errorProvider2.Dispose();
                errorProvider1.SetError(this.txtApellido, "El campo Nombre no puede tener un valor vacio");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider1.SetError(this.txtApellido, "");
                errorProvider2.SetError(this.txtApellido, "CORRECTO");
            }
        }
        #endregion

        #region METODOS

        private void MostrarPedidos()
        {
            CN_Pedidos pedido = new CN_Pedidos();
            dgvPedidos.DataSource = pedido.MostrarPedido();
        }



        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                pedido.IdPedido = Convert.ToInt32(dgvPedidos.Rows[dgvPedidos.SelectedRows[0].Index].Cells["IdPedido"].Value.ToString());
            }
            else
            {
                pedido.IdPedido = 0;
            }

            pedido.NombreCliente = txtNombre.Text;
            pedido.Apellido = txtApellido.Text;
            pedido.Cantidad = txtCantidad.Text;
            pedido.InstruccionesEspeciales = txtInstrucionesEspeciales.Text;
            pedido.OpcionDePago = cbxOpcionPago.Text;

            if (cbxMenu.SelectedItem == null)
            {
                pedido.IdMenu = "0";
            }
            else
            {
                pedido.IdMenu = cbxMenu.SelectedValue.ToString();
            }
        }
        #endregion
    }
}