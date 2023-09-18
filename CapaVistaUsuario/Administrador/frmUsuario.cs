using System;
using System.Drawing;
using System.Windows.Forms;
using CapaLogicaNegocio; // se deve instanciar la capa inferior luego de agregarla como referencia
using CapaLogicaNegocio.Administrador;
using CapaComun;

namespace CapaVistaUsuario
{
    public partial class frmUsuario : Form
    {
        //Instancio los objetos de las clses que utilizo en el form
        CN_Usuarios User = new CN_Usuarios();
        CV_Validar_Mail ValidaCorreo = new CV_Validar_Mail();


        public frmUsuario()
        {
            InitializeComponent();
        }

        private void frmPersonas_Load(object sender, EventArgs e)
        {
            //Acomodo el dataGridView a mi necesidad
            dgvPersonas.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //Selecciona toda la fila
            dgvPersonas.ReadOnly = true; //hace que la grilla no se pueda editar
            dgvPersonas.MultiSelect = false; //desactiva la seleccion multiple
            dgvPersonas.AllowUserToAddRows = false; //desactiva  la ultima fila 

            //CN_LlenarCombos llenarCMB = new CN_LlenarCombos(cmbLocalidad, "Localidades", "IdLocalidad", "Localidades");
            //CN_LlenarCombosPersonal llenarCMB1 = new CN_LlenarCombosPersonal(cmbPersona);
            CN_LlenarCombos llenarCMB2 = new CN_LlenarCombos(cmbTipoDoc, "TipoDoc", "Id", "Tipo");

            txtNroDoc.Text = "0";
            txtNro.Text = "0";
            //cmbTipoDoc.SelectedIndex = 0;
            //cmbLocalidad.SelectedIndex = 0;


            MostrarPersonas();
            dgvPersonas.Select();
            CV_Utiles.BloquearControles(this);  //recordar que no instancio el objeto de  la clase  porque el metodo es estatico
            CV_Botonera.btnFormularios(this, btnCancelar);
        }

        private void dgvPersonas_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Este evento pertenece al DataGridView, y ocurre cualdo toma el foco o cambia de fila
            //Las siguientes lineas llenan los TextBox con los datos del DataGridView
            if (dgvPersonas.SelectedRows.Count > 0)
            {
                /*grpPersonas.Text = "Identificacion Persona Nº " + dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdPersona"].Value.ToString();
                txtApellido.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Apellido"].Value.ToString();
                txtNombres.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Nombres"].Value.ToString();
                cmbTipoDoc.SelectedValue = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdTDoc"].Value;
                txtNroDoc.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["NroDoc"].Value.ToString();
                txtTelefono.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Telefono"].Value.ToString();
                txtCorreo.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Correo"].Value.ToString();
                txtCalle.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Calle"].Value.ToString();
                txtNro.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Nro"].Value.ToString();
                txtPiso.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Piso"].Value.ToString();
                txtDto.Text = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["Dto"].Value.ToString();
                cmbLocalidad.SelectedValue = dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdLocalidad"].Value; ;*/
            }
        }

        #region BOTONES

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                //PasarDatos(false);

                //User.InsertarPersona();
                MostrarPersonas();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPersonas.Select();
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
            txtApellido.Select();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            dgvPersonas.Select();
            errorProvider1.Dispose();
            errorProvider2.Dispose();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnModificar);
            CV_Utiles.DesbloquearControles(this);
            txtApellido.Select();
        }

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            try
            {
               // PasarDatos(true);

                //User.ModificarPersona();

                MostrarPersonas();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPersonas.Select();
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
            var resultado = MessageBox.Show("esta seguro de ELIMINAR defilitivamente a la persona?",
                                                        "ELIMINAR PERSONA",
                                                        MessageBoxButtons.OKCancel,
                                                        MessageBoxIcon.Question);
            if (resultado == DialogResult.OK )
            {
                User.IdUsuario = Convert.ToInt32(dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdPersona"].Value.ToString());
                User.EliminarUsuario();
                MostrarPersonas();
                dgvPersonas.Select();
            }
        }

        #endregion


        #region Validaciones_Nivel_Formulario

        private void txtApellido_Validated(object sender, EventArgs e)
        {
            if (this.txtApellido.Text.Length == 0)
            {
                errorProvider2.Dispose();
                errorProvider1.SetError(this.txtApellido, "Debe tener un valor");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider1.SetError(this.txtApellido, "");
                errorProvider2.SetError(this.txtApellido, "CORRECTO");
            }
        }

        private void txtNombres_Validated(object sender, EventArgs e)
        {
            if (this.txtNombres.Text.Length == 0)
            {
                errorProvider2.Dispose();
                errorProvider1.SetError(this.txtNombres, "El campo Nombre no puede tener un valor vacio");
            }
            else
            {
                errorProvider1.Dispose();
                errorProvider1.SetError(this.txtNombres, "");
                errorProvider2.SetError(this.txtNombres, "CORRECTO");
            }
        }

         private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            CV_Validar_Mail val = new CV_Validar_Mail();
            val.Correo = txtCorreo.Text;
            if (val.Valid() != true)
            {
                txtCorreo.ForeColor = Color.Red;
            }
            else
            {
                txtCorreo.ForeColor = Color.Black;
            }
        }
        #endregion

        #region METODOS

        private void MostrarPersonas()
        {
            CN_Personas Pers = new CN_Personas();
            dgvPersonas.DataSource = Pers.MostrarPersona();
        }

        /*
        private void PasarDatos(bool origen)
        {
            if (origen == true)
            { 
            Pers.IdPersona = Convert.ToInt32(dgvPersonas.Rows[dgvPersonas.SelectedRows[0].Index].Cells["IdPersona"].Value.ToString());
            }
            else
            {
                Pers.IdPersona = 0;
            }
            Pers.Apellido = txtApellido.Text;
            Pers.Nombre = txtNombres.Text;
            if (cmbTipoDoc.SelectedItem == null)
            {
                Pers.TipoDoc = "0";
            }
            else
            {
                Pers.TipoDoc = cmbTipoDoc.SelectedValue.ToString();
            }
            Pers.NroDoc = txtNroDoc.Text;
            Pers.Telefono = txtTelefono.Text;
            Pers.Correo = txtCorreo.Text;
            Pers.Calle = txtCalle.Text;
            Pers.Nro = txtNro.Text;
            Pers.Piso = txtPiso.Text;
            Pers.Dto = txtDto.Text;
            if (cmbLocalidad.SelectedItem == null)
            {
                Pers.IdLocalidad = "0";
            }
            else
            {
                Pers.IdLocalidad = cmbLocalidad.SelectedValue.ToString();
            }
            if (cmbProvincia.SelectedItem == null)
            {
                Pers.IdProvincia = "0";
            }
            else
            {
                Pers.IdProvincia = cmbProvincia.SelectedValue.ToString();
            }
        }
        */
        #endregion

    
    }
}