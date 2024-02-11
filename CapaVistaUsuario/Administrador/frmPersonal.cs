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
    public partial class frmPersonal : Form
    {
        CN_Personal personal = new CN_Personal(); // Asegúrate de tener una instancia de tu clase de negocio para Personal

        public frmPersonal()
        {
            InitializeComponent();
        }

        private void frmPersonal_Load(object sender, EventArgs e)
        {
            CV_Utiles.InicializarLabelPanel("Personal", panelPersonal);

            dgvPersonal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPersonal.ReadOnly = true;
            dgvPersonal.MultiSelect = false;
            dgvPersonal.AllowUserToAddRows = false;

            MostrarPersonal();
            dgvPersonal.Select();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
        }

        private void dgvPersonal_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPersonal.SelectedRows.Count > 0)
            {
                txtApellido.Text = dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["apellido"].Value.ToString();
                txtNombre.Text = dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["nombre"].Value.ToString();
                txtNroDoc.Text = dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["nroDoc"].Value.ToString();
                txtCuil.Text = dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["cuil"].Value.ToString();
                txtCalle.Text = dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["calle"].Value.ToString();
                txtNro.Text = dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["nro"].Value.ToString();
            }
        }

        #region BOTONES

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(false);

                personal.InsertarPersonal();
                MostrarPersonal();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPersonal.Select();
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
            txtApellido.Select();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            dgvPersonal.Select();
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
                PasarDatos(true);

                personal.ModificarPersonal();

                MostrarPersonal();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPersonal.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro de ELIMINAR definitivamente al personal?",
                                            "ELIMINAR PERSONAL",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Question);
            if (resultado == DialogResult.OK)
            {
                personal.IdPersonal = Convert.ToInt32(dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["idPersonal"].Value.ToString());
                personal.EliminarPersonal();
                MostrarPersonal();
                dgvPersonal.Select();
            }
        }

        #endregion

        #region METODOS

        private void MostrarPersonal()
        {
            dgvPersonal.DataSource = personal.MostrarPersonal();
        }

        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                personal.IdPersonal = Convert.ToInt32(dgvPersonal.Rows[dgvPersonal.SelectedRows[0].Index].Cells["idPersonal"].Value.ToString());
            }
            else
            {
                personal.IdPersonal = 0;
            }

            personal.Apellido = txtApellido.Text;
            personal.Nombre = txtNombre.Text;
            personal.NroDoc = txtNroDoc.Text;
            personal.Cuil = txtCuil.Text;
            personal.Calle = txtCalle.Text;
            personal.Nro = Convert.ToInt32(txtNro.Text);
        }

        #endregion
    }
}
